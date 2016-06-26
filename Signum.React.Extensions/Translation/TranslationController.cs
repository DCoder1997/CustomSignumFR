﻿using Signum.Engine;
using Signum.Engine.Authorization;
using Signum.Engine.Basics;
using Signum.Engine.Operations;
using Signum.Engine.Translation;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.React.Filters;
using Signum.Utilities;
using Signum.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

namespace Signum.React.Translation
{
    public class TranslationController : ApiController
    {
        public static IEnumerable<Assembly> AssembliesToLocalize()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.HasAttribute<DefaultAssemblyCultureAttribute>());
        }

        [Route("api/translation/state"), HttpGet]
        public List<TranslationFileStatus> GetState()
        {
            var cultures = TranslationLogic.CurrentCultureInfos(CultureInfo.GetCultureInfo("en"));

            var assemblies = AssembliesToLocalize().ToDictionary(a => a.FullName);

            var dg = DirectedGraph<Assembly>.Generate(assemblies.Values, a => a.GetReferencedAssemblies().Select(an => assemblies.TryGetC(an.FullName)).NotNull());

            var list = (from a in dg.CompilationOrderGroups().SelectMany(gr => gr.OrderBy(a => a.FullName))
                        from ci in cultures
                        select new TranslationFileStatus
                        {
                            assembly = a.GetName().Name,
                            culture = ci.Name,
                            isDefault = ci.Name == a.GetCustomAttribute<DefaultAssemblyCultureAttribute>().DefaultCulture,
                            status = CalculateStatus(a, ci)
                        }).ToList();

            return list;
        }

        private TranslatedSummaryState CalculateStatus(Assembly a, CultureInfo ci)
        {
            var fileName = LocalizedAssembly.TranslationFileName(a, ci);

            if (!System.IO.File.Exists(fileName))
                return TranslatedSummaryState.None;

            var target = DescriptionManager.GetLocalizedAssembly(a, ci);

            CultureInfo defaultCulture = CultureInfo.GetCultureInfo(a.GetCustomAttribute<DefaultAssemblyCultureAttribute>().DefaultCulture);
            var master = DescriptionManager.GetLocalizedAssembly(a, defaultCulture);

            var result = TranslationSynchronizer.GetMergeChanges(target, master, new List<LocalizedAssembly>());

            if (result.Any())
                return TranslatedSummaryState.Pending;

            return TranslatedSummaryState.Completed;
        }

        public class TranslationFileStatus
        {
            public string assembly;
            public string culture;
            public bool isDefault;
            public TranslatedSummaryState status;
        }


        [Route("api/translation/retrieve"), HttpPost]
        public AssemblyResultTS RetrieveTypes(string assembly, string culture, string filter)
        {
            Assembly ass = AssembliesToLocalize().Where(a => a.GetName().Name == assembly).SingleEx(() => "Assembly {0}".FormatWith(assembly));

            CultureInfo defaultCulture = CultureInfo.GetCultureInfo(ass.GetCustomAttribute<DefaultAssemblyCultureAttribute>().DefaultCulture);
            CultureInfo targetCulture = culture == null ? null : CultureInfo.GetCultureInfo(culture);

            var cultures = TranslationLogic.CurrentCultureInfos(defaultCulture);

            Dictionary<string, LocalizableTypeTS> types =
                (from ci in cultures
                 let la = DescriptionManager.GetLocalizedAssembly(ass, ci)
                 where la != null || ci == defaultCulture || ci == targetCulture
                 let la2 = la ?? LocalizedAssembly.ImportXml(ass, ci, forceCreate: true)
                 from t in la2.Types.Values
                 let lt = new LocalizedTypeTS
                 {
                     culture = ci.Name,
                     type = t.Type.Name,
                     gender = t.Gender?.ToString(),
                     description = t.Description,
                     pluralDescription = t.PluralDescription,

                     members = t.Members.ToDictionary(),
                 }
                 group lt by t.Type into g
                 let options = LocalizedAssembly.GetDescriptionOptions(g.Key)
                 select KVP.Create(g.Key.Name, new LocalizableTypeTS
                 {
                     type = g.Key.Name,
                     hasDescription = options.IsSet(DescriptionOptions.Description),
                     hasPluralDescription = options.IsSet(DescriptionOptions.PluralDescription),
                     hasMembers = options.IsSet(DescriptionOptions.Members),
                     hasGender = options.IsSet(DescriptionOptions.Gender),
                     cultures = g.ToDictionary(a => a.culture)
                 }))
                 .ToDictionary("types");


            if (filter.HasText())
            {
                var complete = types.Extract((k, v) => v.type.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                            v.cultures.Values.Any(lt =>
                            lt.description != null && lt.description.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                            lt.pluralDescription != null && lt.pluralDescription.Contains(filter, StringComparison.InvariantCultureIgnoreCase)));


                var filtered = types.Extract((k, v) =>
                {
                    var allMembers = v.cultures.Values.SelectMany(a => a.members.Keys).Distinct().ToList();

                    var filteredMembers = allMembers.Where(m => m.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                    v.cultures.Values.Any(lt => lt.members.TryGetC(m)?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false))
                    .ToList();

                    if (filteredMembers.Count == 0)
                        return false;

                    foreach (var item in v.cultures.Values)
                    {
                        item.members = item.members.Where(a => filteredMembers.Contains(a.Key)).ToDictionary();
                    }

                    return true;

                });

                types = complete.Concat(filtered).ToDictionary();
            }

            return new AssemblyResultTS
            {
                types = types.OrderBy(a => a.Key).ToDictionary(),
                cultures = cultures.Select(c => new CulturesTS
                {
                    name = c.Name,
                    englishName = c.EnglishName,
                    pronoms = NaturalLanguageTools.GenderDetectors.TryGetC(c.TwoLetterISOLanguageName)?.Pronoms.ToList()
                }).ToDictionary(a => a.name)
            };
        }

        public class AssemblyResultTS
        {
            public Dictionary<string, CulturesTS> cultures;
            public Dictionary<string, LocalizableTypeTS> types;
        }
        
        public class CulturesTS
        {
            public string name;
            public string englishName;
            public List<PronomInfo> pronoms; 

        }

        public class LocalizableTypeTS
        {
            public string type;
            public bool hasMembers;
            public bool hasGender;
            public bool hasDescription;
            public bool hasPluralDescription;

            public Dictionary<string, LocalizedTypeTS> cultures;
        }

        public class LocalizedTypeTS
        {
            public string type;
            public string culture;
            public string gender;
            public string description;
            public string pluralDescription;
            public Dictionary<string, string> members;
        }


        [Route("api/translation/save"), HttpPost]
        public void SaveTypes(string assembly, string culture, AssemblyResultTS result)
        {
            var currentAssembly = AssembliesToLocalize().Single(a => a.GetName().Name == assembly);

            foreach (var cultureGroup in result.types.Values.SelectMany(a => a.cultures.Values).GroupBy(lt => lt.culture).ToList())
            {
                LocalizedAssembly locAssembly = LocalizedAssembly.ImportXml(currentAssembly, CultureInfo.GetCultureInfo(cultureGroup.Key), forceCreate: true);

                var types = cultureGroup.ToDictionary(a => a.type);

                foreach (var lt in locAssembly.Types.Values)
                {
                    var ts = types.TryGetC(lt.Type.Name);

                    if (ts != null)
                    {
                        lt.Gender = ts.gender?[0];
                        lt.Description = ts.description;
                        lt.PluralDescription = ts.pluralDescription;
                        lt.Members.SetRange(ts.members);
                    }
                }

                locAssembly.ExportXml();
            }
        }

        [Route("api/translation/pluralize"), HttpPost]
        public string Pluralize(string culture, [FromBody]string text)
        {
            return NaturalLanguageTools.Pluralize(text, CultureInfo.GetCultureInfo(culture));
        }

        [Route("api/translation/gender"), HttpPost]
        public string Gender(string culture, [FromBody]string text)
        {
            return NaturalLanguageTools.GetGender(text, CultureInfo.GetCultureInfo(culture))?.ToString();
        }
    }
}
