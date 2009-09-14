﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Signum.Engine;
using Signum.Engine.Maps;
using Signum.Utilities;
using Signum.Entities;
using Signum.Utilities.DataStructures;
using System.Reflection;
using Signum.Utilities.Reflection;
using System.Collections.Specialized;
using Signum.Web.Properties;
using Signum.Entities.Properties;
using Signum.Entities.Reflection;
using Signum.Entities.DynamicQuery;
using Signum.Engine.DynamicQuery;
using System.Configuration;

namespace Signum.Web
{
    public delegate bool IsViewableEvent(Type type, bool admin);

    public static class Navigator
    {
        private static Func<Type, int, string> viewRoute;
        public static Func<Type, int, string> ViewRoute 
        { 
            get 
            {
                return viewRoute ??
                       (viewRoute = (Type type, int id) => "View/{0}/{1}".Formato(Navigator.TypesToURLNames[type], id));
            }
            set 
            {
                viewRoute = value;
            }
        }

        private static Func<object, string> findRoute;
        public static Func<object, string> FindRoute
        {
            get
            {
                return findRoute ??
                       (findRoute = (object queryName) => "Find/{0}".Formato(Manager.QuerySettings[queryName].UrlName));
            }
            set
            {
                findRoute = value;
            }
        }

        public static NavigationManager Manager;

        public static void Start()
        {
            Manager.Start();
        }
        
        public static Type ResolveType(string typeName)
        {
            return Manager.ResolveType(typeName);
        }

        public static Type ResolveTypeFromUrlName(string typeUrlName)
        {
            return Manager.ResolveTypeFromUrlName(typeUrlName); 
        }

        public static object ResolveQueryFromUrlName(string queryUrlName)
        {
            return Manager.ResolveQueryFromUrlName(queryUrlName);
        }

        public static object ResolveQueryFromToStr(string queryNameToStr)
        {
            return Manager.ResolveQueryFromToStr(queryNameToStr);
        }

        public static ViewResult View(this Controller controller, object obj)
        {
            return Manager.View(controller, obj, null); 
        }

        public static ViewResult View(this Controller controller, object obj, string partialViewName)
        {
            return Manager.View(controller, obj, partialViewName);
        }

        public static PartialViewResult PopupView<T>(this Controller controller, T entity, string prefix)
        {
            return Manager.PopupView(controller, entity, prefix, null);
        }

        public static PartialViewResult PopupView<T>(this Controller controller, T entity, string prefix, string partialViewName)
        {
            return Manager.PopupView(controller, entity, prefix, partialViewName);
        }

        public static PartialViewResult PartialView<T>(this Controller controller, T entity, string prefix)
        {
            return Manager.PartialView(controller, entity, prefix, null);
        }

        public static PartialViewResult PartialView<T>(this Controller controller, T entity, string prefix, string partialViewName)
        {
            return Manager.PartialView(controller, entity, prefix, partialViewName);
        }

        public static ViewResult Find(Controller controller, object queryName)
        {
            return Find(controller, new FindOptions(queryName));
        }

        public static ViewResult Find(Controller controller, FindOptions findOptions)
        {
            return Manager.Find(controller, findOptions);
        }

        public static PartialViewResult PartialFind(Controller controller, FindOptions findOptions, string prefix, string prefixEnd)
        {
            return Manager.PartialFind(controller, findOptions, prefix, prefixEnd);
        }

        public static PartialViewResult Search(Controller controller, object queryName, List<Filter> filters, int? resultsLimit, bool? allowMultiple, string prefix)
        {
            return Manager.Search(controller, queryName, filters, resultsLimit, allowMultiple, prefix);
        }

        internal static List<Filter> ExtractFilters(HttpContextBase httpContext, object queryName)
        {
            return Manager.ExtractFilters(httpContext, queryName);
        }

        internal static List<FilterOptions> ExtractFilterOptions(HttpContextBase httpContext, object queryName)
        {
            return Manager.ExtractFilterOptions(httpContext, queryName);
        }

        public static SortedList<string, object> ToSortedList(NameValueCollection form, string prefixFilter, string prefixToIgnore)
        {
            SortedList<string, object> formValues = new SortedList<string, object>(form.Count);
            foreach (string key in form.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key != "prefixToIgnore" && (string.IsNullOrEmpty(prefixFilter) || key.StartsWith(prefixFilter)))
                {
                    if (string.IsNullOrEmpty(prefixToIgnore) || !key.StartsWith(prefixToIgnore))
                        formValues.Add(key, form[key]);
                }
            }
            
            return formValues;
        }

        public static SortedList<string, object> ToSortedList(NameValueCollection form, string prefixToIgnore)
        {
            SortedList<string, object> formValues = new SortedList<string, object>(form.Count);
            foreach (string key in form.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key != "prefixToIgnore")
                {
                    if (string.IsNullOrEmpty(prefixToIgnore) || !key.StartsWith(prefixToIgnore))
                        formValues.Add(key, form[key]);
                }
            }

            return formValues;
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, string prefixToIgnore, ref T obj)
        {
            SortedList<string, object> formValues = ToSortedList(controller.Request.Form, prefixToIgnore);

            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, null);
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, string prefixToIgnore, ref T obj, out List<string> fullIntegrityErrors)
        {
            SortedList<string, object> formValues = ToSortedList(controller.Request.Form, prefixToIgnore);

            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, null, out fullIntegrityErrors);
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, string prefix, string prefixToIgnore, ref T obj)
        {
            SortedList<string, object> formValues = ToSortedList(controller.Request.Form, prefix, prefixToIgnore);

            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, prefix);
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, SortedList<string, object> formValues, ref T obj)
        {
            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, null);
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, SortedList<string, object> formValues, ref T obj, string prefix)
            where T:Modifiable
        {
            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, prefix);
        }

        public static Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, SortedList<string, object> formValues, ref T obj, string prefix, out List<string> fullIntegrityErrors)
            where T:Modifiable
        {
            return Manager.ApplyChangesAndValidate(controller, formValues, ref obj, prefix, out fullIntegrityErrors);
        }

        public static ModifiableEntity ExtractEntity(Controller controller, NameValueCollection form)
        {
            return Manager.ExtractEntity(controller, form, null);
        }

        public static ModifiableEntity ExtractEntity(Controller controller, NameValueCollection form, string prefix)
        {
            return Manager.ExtractEntity(controller, form, prefix);
        }

        public static object CreateInstance(Controller controller, Type type)
        {
            lock (Constructor.ConstructorManager)
            {
                return Constructor.Construct(type, controller);
            }
        }

        public static ModifiableEntity CreateInstance(Type type)
        {
            lock (Constructor.ConstructorManager)
            {
                return Constructor.ConstructStrict(type);
            }
        }

        static Dictionary<string, Type> nameToType;
        public static Dictionary<string, Type> NameToType
        {
            get { return nameToType.ThrowIfNullC("Names to Types dictionary not initialized"); }
            internal set { nameToType = value; }
        }

        public static Dictionary<Type, string> TypesToURLNames
        {
            get { return Manager.TypesToURLNames; }
        }

        internal static void ConfigureEntityBase(EntityBase el, Type entityType, bool admin)
        {
            if (Manager.EntitySettings.ContainsKey(entityType))
            {
                el.Create = Navigator.IsCreable(entityType, admin);
                el.View = Navigator.IsViewable(entityType, admin);
                el.Find = Navigator.IsFindable(entityType);
            }
        }

        public static bool IsViewable(Type type, bool admin)
        {
            return Manager.IsViewable(type, admin);
        }
        public static bool IsReadOnly(Type type, bool admin)
        {
            return Manager.IsReadOnly(type, admin);
        }
        public static bool IsCreable(Type type, bool admin)
        {
            return Manager.IsCreable(type, admin);
        }
        public static bool IsFindable(object queryName)
        {
            return Manager.IsFindable(queryName);
        }
    }
    
    public class NavigationManager
    {
        public Dictionary<Type, EntitySettings> EntitySettings;
        public Dictionary<object, QuerySettings> QuerySettings;
        public DynamicQueryManager Queries { get; set; }
        
        public string NormalPageUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.NormalPage.aspx";
        public string PopupControlUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.PopupControl.ascx";
        public string OKCancelPopulUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.OKCancelPopup.ascx";
        public string SearchPopupControlUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.SearchPopupControl.ascx";
        public string SearchWindowUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.SearchWindow.aspx";
        public string SearchControlUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.SearchControl.ascx";
        public string SearchResultsUrl = "~/Plugin/Signum.Web.dll/Signum.Web.Views.SearchResults.ascx";
        
        protected internal Dictionary<string, Type> URLNamesToTypes { get; private set; }
        protected internal Dictionary<Type, string> TypesToURLNames { get; private set; }
        protected internal Dictionary<string, object> UrlQueryNames { get; private set; }

        public event Func<Type, bool> GlobalIsCreable;
        public event Func<Type, bool> GlobalIsViewable;
        public event Func<Type, bool> GlobalIsReadOnly;
        public event Func<object, bool> GlobalIsFindable;


        //public void AddEntitySettings(Type type, string ViewPrefix)
        //{
        //    if (!type.Name.EndsWith("DN")) throw new ApplicationException("This method is only valid for xxxDN entities");
        //    EntitySettings.Add(type, new EntitySettings(false) { PartialViewName = ViewPrefix + type.Name.RemoveRight(2) + "IU.ascx" });
        //}

        public void Start()
        {
            URLNamesToTypes = EntitySettings.ToDictionary(
                kvp => kvp.Value.UrlName ?? (kvp.Key.Name.EndsWith("DN") ? kvp.Key.Name.Substring(0, kvp.Key.Name.Length - 2) : kvp.Key.Name), 
                kvp => kvp.Key);
            TypesToURLNames = URLNamesToTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            Navigator.NameToType = EntitySettings.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Key);

            if (Queries != null)
            {
                if (QuerySettings == null)
                    QuerySettings = new Dictionary<object, QuerySettings>();
                foreach (object o in Queries.GetQueryNames())
                {
                    if (!QuerySettings.ContainsKey(o))
                        QuerySettings.Add(o, new QuerySettings() { Top = 5 });
                    if (!QuerySettings[o].UrlName.HasText())
                        QuerySettings[o].UrlName = GetQueryName(o);
                }

                UrlQueryNames = QuerySettings.ToDictionary(kvp => kvp.Value.UrlName ?? GetQueryName(kvp.Key), kvp => kvp.Key);
            }
        }

        HashSet<string> loadedModules = new HashSet<string>();

        public bool NotDefined<T>()
        {
            return NotDefined(typeof(T).FullName);
        }

        public bool NotDefined(string moduleName)
        {
            return loadedModules.Add(moduleName);
        }

        protected internal virtual string GetQueryName(object queryName)
        {
            return (queryName is Type) ? (TypesToURLNames.TryGetC<Type, string>((Type)queryName) ?? ((Type)queryName).Name) :
                   (queryName is Enum) ? EnumExtensions.NiceToString((Enum)queryName) :
                   queryName.ToString();
        }

        protected internal virtual ViewResult View(Controller controller, object obj, string partialViewName)
        {
            EntitySettings es = Navigator.Manager.EntitySettings.TryGetC(obj.GetType()).ThrowIfNullC("No hay una vista asociada al tipo: " + obj.GetType());

            controller.ViewData[ViewDataKeys.MainControlUrl] = partialViewName ?? es.PartialViewName;
            IdentifiableEntity entity = (IdentifiableEntity)obj;
            controller.ViewData.Model = entity;
            if (controller.ViewData.Keys.Count(s => s==ViewDataKeys.PageTitle)==0)
                controller.ViewData[ViewDataKeys.PageTitle] = entity.ToStr;

            return new ViewResult()
            {
                ViewName = NormalPageUrl,
                MasterName = null,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual PartialViewResult PopupView<T>(Controller controller, T entity, string prefix, string partialViewName)
        {
            string url = partialViewName ??
                Navigator.Manager.EntitySettings.TryGetC(entity.GetType()).ThrowIfNullC("No hay una vista asociada al tipo: " + entity.GetType()).PartialViewName;

            controller.ViewData[ViewDataKeys.MainControlUrl] = url;
            controller.ViewData[ViewDataKeys.PopupPrefix] = prefix;

            controller.ViewData.Model = entity;
            
            return new PartialViewResult
            {
                ViewName = PopupControlUrl,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual PartialViewResult PartialView<T>(Controller controller, T entity, string prefix, string partialViewName)
        {
            Type cleanType = entity != null ? entity.GetType() : typeof(T);
            string url = partialViewName ??
                Navigator.Manager.EntitySettings.TryGetC(cleanType).ThrowIfNullC("No hay una vista asociada al tipo: " + cleanType).PartialViewName;

            controller.ViewData[ViewDataKeys.PopupPrefix] = prefix;
            controller.ViewData.Model = entity;

            if (controller.ViewData.ContainsKey(ViewDataKeys.EmbeddedControl))
                controller.Response.Write("<input type='hidden' id='{0}' name='{0}' value='' />".Formato(prefix + TypeContext.Separator + EntityBaseKeys.IsNew)); 

            return new PartialViewResult
            {
                ViewName = url,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual ViewResult Find(Controller controller, FindOptions findOptions)
        {
            QueryDescription queryDescription = Queries.QueryDescription(findOptions.QueryName);

            Type entitiesType = Reflector.ExtractLazy(queryDescription.Columns.Single(a => a.IsEntity).Type);

            List<Column> columns = queryDescription.Columns.Where(a => a.Filterable).ToList();

            foreach (FilterOptions opt in findOptions.FilterOptions)
            {
                opt.Column = queryDescription.Columns.Where(c => c.Name == opt.ColumnName)
                    .Single("Filter Column {0} not found or found more than once in query description".Formato(opt.ColumnName));
            }

            //controller.ViewData[ViewDataKeys.SearchResourcesRoute] = ConfigurationManager.AppSettings[ViewDataKeys.SearchResourcesRoute] ?? "../../";
            controller.ViewData[ViewDataKeys.MainControlUrl] = SearchControlUrl;
            controller.ViewData[ViewDataKeys.FilterColumns] = columns;
            controller.ViewData[ViewDataKeys.FindOptions] = findOptions;
            controller.ViewData[ViewDataKeys.Top] = QuerySettings.TryGetC(findOptions.QueryName).ThrowIfNullC("QuerySettings not present for QueryName {0}".Formato(findOptions.QueryName.ToString())).Top;
            if (controller.ViewData.Keys.Count(s => s == ViewDataKeys.PageTitle) == 0)
                controller.ViewData[ViewDataKeys.PageTitle] = SearchTitle(findOptions.QueryName);
            controller.ViewData[ViewDataKeys.EntityTypeName] = entitiesType.Name;
            controller.ViewData[ViewDataKeys.EntityType] = entitiesType;
            controller.ViewData[ViewDataKeys.Create] =
                (findOptions.Create.HasValue) ?
                    findOptions.Create.Value :
                    Navigator.IsCreable(entitiesType, true); ;

            return new ViewResult()
            {
                ViewName = SearchWindowUrl,
                MasterName = null,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual PartialViewResult PartialFind(Controller controller, FindOptions findOptions, string prefix, string prefixEnd)
        {
            QueryDescription queryDescription = Queries.QueryDescription(findOptions.QueryName);

            Type entitiesType = Reflector.ExtractLazy(queryDescription.Columns.Single(a => a.IsEntity).Type);

            List<Column> columns = queryDescription.Columns.Where(a => a.Filterable).ToList();

            //controller.ViewData[ViewDataKeys.ResourcesRoute] = ConfigurationManager.AppSettings[ViewDataKeys.ResourcesRoute] ?? "../../";
            controller.ViewData[ViewDataKeys.MainControlUrl] = SearchControlUrl;
            controller.ViewData[ViewDataKeys.PopupPrefix] = prefix;
            controller.ViewData[ViewDataKeys.PopupSufix] = prefixEnd ?? "";

            controller.ViewData[ViewDataKeys.FilterColumns] = columns;
            controller.ViewData[ViewDataKeys.FindOptions] = findOptions;
            controller.ViewData[ViewDataKeys.Top] = QuerySettings.TryGetC(findOptions.QueryName).ThrowIfNullC("QuerySettings not present for QueryName {0}".Formato(findOptions.QueryName.ToString())).Top;
            //controller.ViewData[ViewDataKeys.QuerySettings] = QuerySettings.TryGetC(findOptions.QueryName).ThrowIfNullC("QuerySettings not present for QueryName {0}".Formato(findOptions.QueryName.ToString()));
            if (controller.ViewData.Keys.Count(s => s == ViewDataKeys.PageTitle) == 0)
                controller.ViewData[ViewDataKeys.PageTitle] = SearchTitle(findOptions.QueryName);
            controller.ViewData[ViewDataKeys.EntityTypeName] = entitiesType.Name;
            controller.ViewData[ViewDataKeys.EntityType] = entitiesType;
            controller.ViewData[ViewDataKeys.Create] =
                (findOptions.Create.HasValue) ?
                    findOptions.Create.Value :
                    Navigator.IsCreable(entitiesType, true);

            return new PartialViewResult
            {
                ViewName = SearchPopupControlUrl,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual string SearchTitle(object queryName)
        {
            if (QuerySettings != null)
            {
                QuerySettings qs = QuerySettings.TryGetC(queryName);
                if (qs != null && qs.Title != null)
                    return qs.Title;
            }

            return GetQueryName(queryName);
        }

        protected internal virtual PartialViewResult Search(Controller controller, object queryName, List<Filter> filters, int? resultsLimit, bool? allowMultiple, string prefix)
        {
            QueryResult queryResult = Queries.ExecuteQuery(queryName, filters, resultsLimit);

            //controller.ViewData[ViewDataKeys.ResourcesRoute] = ConfigurationManager.AppSettings[ViewDataKeys.ResourcesRoute] ?? "../../";
            controller.ViewData[ViewDataKeys.Results] = queryResult;
            controller.ViewData[ViewDataKeys.AllowMultiple] = allowMultiple;
            controller.ViewData[ViewDataKeys.PopupPrefix] = prefix;

            if (queryResult != null && queryResult.Data != null && queryResult.Data.Length > 0 && queryResult.VisibleColums.Count > 0)
            {
                int entityColumnIndex = queryResult.Columns.IndexOf(c => c.IsEntity);
                controller.ViewData[ViewDataKeys.EntityColumnIndex] = entityColumnIndex;
            }

            QuerySettings settings = QuerySettings[queryName];
            controller.ViewData[ViewDataKeys.Formatters] = queryResult.Columns.Select(c =>settings.GetFormatter(c)).ToList();

            return new PartialViewResult
            {
                ViewName = SearchResultsUrl,
                ViewData = controller.ViewData,
                TempData = controller.TempData
            };
        }

        protected internal virtual List<Filter> ExtractFilters(HttpContextBase httpContext, object queryName)
        {
            List<Filter> result = new List<Filter>();

            QueryDescription queryDescription = Queries.QueryDescription(queryName);

            int index = 0;
            string name;
            object value;
            string operation;
            Type type;
            NameValueCollection parameters = httpContext.Request.Params;
            var names = parameters.AllKeys.Where(k => k.StartsWith("name"));
            foreach(string nameKey in names)
            {
                if (!int.TryParse(nameKey.RemoveLeft(4), out index))
                    continue;

                name = parameters[nameKey];
                value = parameters["val" + index.ToString()];
                operation = parameters["sel" + index.ToString()];
                type = queryDescription.Columns
                           .SingleOrDefault(c => c.Name == name)
                           .ThrowIfNullC("Invalid filter, column \"{0}\" not found".Formato(name))
                           .Type;

                if (type == typeof(bool))
                {
                    string[] vals = ((string)value).Split(',');
                    value = (vals[0] == "true") ? true : false;
                }

                if (typeof(Lazy).IsAssignableFrom(type))
                {
                    int intValue;
                    if (value!=null && int.TryParse(value.ToString(), out intValue))
                        value = Lazy.Create(Reflector.ExtractLazy(type), intValue);
                    else
                        value = null;
                }
                FilterOperation filterOperation = ((FilterOperation[])Enum.GetValues(typeof(FilterOperation))).SingleOrDefault(op => op.ToString() == operation);

                result.Add(new Filter
                {
                    Name = name, 
                    Type = type,
                    Operation = filterOperation,
                    Value = value,
                });
            }
            return result;
        }

        protected internal virtual List<FilterOptions> ExtractFilterOptions(HttpContextBase httpContext, object queryName)
        {
            List<FilterOptions> result = new List<FilterOptions>();

            QueryDescription queryDescription = Queries.QueryDescription(queryName);

            int index = 0;
            string name;
            object value;
            string operation;
            bool frozen;
            Type type;
            NameValueCollection parameters = httpContext.Request.Params;
            var names = parameters.AllKeys.Where(k => k.StartsWith("name"));
            foreach (string nameKey in names)
            {
                if (!int.TryParse(nameKey.RemoveLeft(4), out index))
                    continue;

                name = parameters[nameKey];
                value = parameters["val" + index.ToString()];
                operation = parameters["sel" + index.ToString()];
                type = queryDescription.Columns
                           .SingleOrDefault(c => c.Name == name)
                           .ThrowIfNullC("Invalid filter, column \"{0}\" not found".Formato(name))
                           .Type;
                
                if (type == typeof(bool))
                {
                    string[] vals = ((string)value).Split(',');
                    value = (vals[0] == "true") ? true : false;
                }
                if (typeof(Lazy).IsAssignableFrom(type))
                {
                    int intValue;
                    if (value != null && int.TryParse(value.ToString(), out intValue))
                        value = Lazy.Create(Reflector.ExtractLazy(type), intValue);
                    else
                        value = null;
                }
                FilterOperation filterOperation = ((FilterOperation[])Enum.GetValues(typeof(FilterOperation))).SingleOrDefault(op => op.ToString() == operation);

                frozen = parameters.AllKeys.Any(k => k == "frozen" + index.ToString());

                result.Add(new FilterOptions
                {
                    ColumnName = name,
                    Operation = filterOperation,
                    Frozen = frozen,
                    Value = value,
                });
            }
            return result;
        }

        protected internal virtual Type ResolveTypeFromUrlName(string typeUrlName)
        {
            return URLNamesToTypes
                .TryGetC(typeUrlName)
                .ThrowIfNullC("No hay un tipo asociado al nombre: " + typeUrlName);
        }

        protected internal virtual object ResolveQueryFromUrlName(string queryUrlName)
        {
            if (UrlQueryNames.ContainsKey(queryUrlName)) 
                return UrlQueryNames[queryUrlName];

            //If it's the name of a Type
            if (Navigator.NameToType.ContainsKey(queryUrlName))
            { 
                string urlName= TypesToURLNames[Navigator.NameToType[queryUrlName]];
                if (!string.IsNullOrEmpty(urlName))
                    return UrlQueryNames[urlName].ThrowIfNullC("No hay una query asociado al nombre: " + queryUrlName);
            }

            throw new ArgumentException("No hay una query asociado al nombre: " + queryUrlName);
        }

        protected internal virtual object ResolveQueryFromToStr(string queryNameToStr)
        {
            return Queries.GetQueryNames()
                .SingleOrDefault(qn => qn.ToString() == queryNameToStr)
                .ThrowIfNullC("No hay una query asociado al nombre: " + queryNameToStr);
        }

        protected internal virtual Type ResolveType(string typeName)
        {
            Type type = Navigator.NameToType.TryGetC(typeName);
            if (type == null)
                throw new ArgumentException(Resource.Type0NotFoundInTheSchema.Formato(typeName));
            return type;
        }

        protected internal virtual Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, SortedList<string, object> formValues, ref T obj, string prefix)
        {
            Modification modification = GenerateModification(formValues, (Modifiable)(object)obj, prefix ?? "");

            obj = (T)modification.ApplyChanges(controller, obj);

            return GenerateErrors((Modifiable)(object)obj, modification);
        }

        protected internal virtual Dictionary<string, List<string>> ApplyChangesAndValidate<T>(Controller controller, SortedList<string, object> formValues, ref T obj, string prefix, out List<string> fullIntegrityErrors)
        {
            Modification modification = GenerateModification(formValues, (Modifiable)(object)obj, prefix ?? "");

            obj = (T)modification.ApplyChanges(controller, obj);

            return GenerateErrors((Modifiable)(object)obj, modification, out fullIntegrityErrors);
        }

        protected internal virtual Dictionary<string, List<string>> GenerateErrors(Modifiable obj, Modification modification)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            modification.Validate(obj, errors);

            Dictionary<Modifiable, string> dicGlobalErrors = obj.FullIntegrityCheckDictionary();
            //Split each error in one entry in the HashTable:
            var globalErrors = dicGlobalErrors.SelectMany(a => a.Value.Lines()).ToList();
            //eliminar de globalErrors los que ya hemos metido en el diccionario
            errors.SelectMany(a => a.Value)
                  .ForEach(e => globalErrors.Remove(e));
            //meter el resto en el diccionario
            if (globalErrors.Count > 0)
                errors.Add(ViewDataKeys.GlobalErrors, globalErrors.ToList());

            return errors;
        }

        protected internal virtual Dictionary<string, List<string>> GenerateErrors(Modifiable obj, Modification modification, out List<string> fullIntegrityErrors)
        {
            fullIntegrityErrors = null;

            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            modification.Validate(obj, errors);

            Dictionary<Modifiable, string> dicGlobalErrors = obj.FullIntegrityCheckDictionary();
            //Split each error in one entry in the HashTable:
            var globalErrors = dicGlobalErrors.SelectMany(a => a.Value.Lines()).ToList();
            //eliminar de globalErrors los que ya hemos metido en el diccionario
            errors.SelectMany(a => a.Value)
                  .ForEach(e => globalErrors.Remove(e));
            //meter el resto en el diccionario
            fullIntegrityErrors = globalErrors.ToList();

            return errors;
        }

        public virtual Modification GenerateModification(SortedList<string, object> formValues, Modifiable obj, string prefix)
        {
            MinMax<int> interval = Modification.FindSubInterval(formValues, prefix);

            return Modification.Create(obj.GetType(), formValues, interval, prefix);
        }

        protected internal virtual ModifiableEntity ExtractEntity(Controller controller, NameValueCollection form, string prefix)
        {
            string typeName = form[(prefix ?? "") + TypeContext.Separator + TypeContext.RuntimeType];
            string id = form[(prefix ?? "") + TypeContext.Separator + TypeContext.Id];

            Type type = Navigator.NameToType.GetOrThrow(typeName, Resource.Type0NotFoundInTheSchema);

            if (!string.IsNullOrEmpty(id))
                return Database.Retrieve(type, int.Parse(id));
            else
                return (ModifiableEntity)Constructor.Construct(type, controller);
        }

        protected internal virtual bool IsViewable(Type type, bool admin)
        {
            if (GlobalIsViewable != null && !GlobalIsViewable(type))
                return false;

            return EntitySettings[type].IsViewable(admin);
        }

        protected internal virtual bool IsReadOnly(Type type, bool admin)
        {
            if (GlobalIsReadOnly != null && !GlobalIsReadOnly(type))
                return false;

            return EntitySettings[type].IsReadOnly(admin);
        }

        protected internal virtual bool IsCreable(Type type, bool admin)
        {
            if (GlobalIsCreable != null && !GlobalIsCreable(type))
                return false;

            return EntitySettings[type].IsCreable(admin);
        }

        protected internal virtual bool IsFindable(object queryName)
        {
            if (GlobalIsFindable != null && !GlobalIsFindable(queryName))
                return false;

            if (QuerySettings == null)
                return false;

            return QuerySettings.ContainsKey(queryName);
        }

        public virtual bool ShowOkSave(Type type, bool admin)
        {
            EntitySettings es = EntitySettings.TryGetC(type);
            if (es != null && es.ShowOkSave != null)
                return es.ShowOkSave(admin);

            return true;
        }

        public virtual bool ShowSearchOkButton(object queryName, bool admin)
        {
            QuerySettings qs = QuerySettings.TryGetC(queryName);
            if (qs != null && qs.ShowOkButton != null)
                return qs.ShowOkButton(admin);

            return true;
        }
    }

    public static class ModelStateExtensions
    { 
        public static void FromDictionary(this ModelStateDictionary modelState, Dictionary<string, List<string>> errors, NameValueCollection form)
        {
            if (errors != null)
                errors.ForEach(p => p.Value.ForEach(v => modelState.AddModelError(p.Key, v, form[p.Key])));
        }

        public static string ToJsonData(this ModelStateDictionary modelState)
        {
            return modelState.ToJSonObjectBig(
                key => key.Quote(),
                value => value.Errors.ToJSonArray(me => me.ErrorMessage.Quote()));
        }

        //http://www.crankingoutcode.com/2009/02/01/IssuesWithAddModelErrorSetModelValueWithMVCRC1.aspx
        //Necesary to set model value if you add a model error, otherwise some htmlhelpers throw exception
        public static void AddModelError(this ModelStateDictionary modelState, string key, string errorMessage, string attemptedValue)
        {
            modelState.AddModelError(key, errorMessage);
            modelState.SetModelValue(key, new ValueProviderResult(attemptedValue, attemptedValue, null));
        }

    }
}
