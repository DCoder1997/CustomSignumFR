﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Help.Views
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    
    #line 9 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Engine.Basics;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Engine.Help;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 7 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Entities.Basics;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Entities.Help;
    
    #line default
    #line hidden
    
    #line 1 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Entities.Reflection;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 5 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Web.Extensions;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Help\Views\ViewEntity.cshtml"
    using Signum.Web.Help;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Help/Views/ViewEntity.cshtml")]
    public partial class ViewEntity : System.Web.Mvc.WebViewPage<dynamic>
    {

#line 20 "..\..\Help\Views\ViewEntity.cshtml"
public System.Web.WebPages.HelperResult WriteProperty(Node<Tuple<PropertyHelp, TypeElementContext<PropertyRouteHelpDN>>> node, string entityName)
{
#line default
#line hidden
return new System.Web.WebPages.HelperResult(__razor_helper_writer => {

#line 21 "..\..\Help\Views\ViewEntity.cshtml"
 
    PropertyHelp ph = node.Value.Item1;
    using (var ctx = node.Value.Item2)
    {
    

#line default
#line hidden

#line 25 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, Html.HiddenRuntimeInfo(ctx));


#line default
#line hidden

#line 25 "..\..\Help\Views\ViewEntity.cshtml"
                                
    


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <dt");

WriteAttributeTo(__razor_helper_writer, "id", Tuple.Create(" id=\"", 679), Tuple.Create("\"", 722)

#line 27 "..\..\Help\Views\ViewEntity.cshtml"
, Tuple.Create(Tuple.Create("", 684), Tuple.Create<System.Object, System.Int32>(HelpUrls.IdProperty(ph.PropertyRoute)

#line default
#line hidden
, 684), false)
);

WriteLiteralTo(__razor_helper_writer, ">");


#line 27 "..\..\Help\Views\ViewEntity.cshtml"
                      WriteTo(__razor_helper_writer, ph.PropertyInfo.NiceName());


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, " <code");

WriteLiteralTo(__razor_helper_writer, " class=\'shortcut\'");

WriteLiteralTo(__razor_helper_writer, ">[p:");


#line 27 "..\..\Help\Views\ViewEntity.cshtml"
                                                                            WriteTo(__razor_helper_writer, entityName);


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, ".");


#line 27 "..\..\Help\Views\ViewEntity.cshtml"
                                                                                         WriteTo(__razor_helper_writer, ph.PropertyRoute.PropertyString());


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "]</code></dt>\r\n");

WriteLiteralTo(__razor_helper_writer, "    <dd>\r\n        <span");

WriteLiteralTo(__razor_helper_writer, " class=\"info\"");

WriteLiteralTo(__razor_helper_writer, ">\r\n");

WriteLiteralTo(__razor_helper_writer, "            ");


#line 30 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, Html.WikiParse(ph.Info, HelpWiki.DefaultWikiSettings));


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\r\n        </span>\r\n");

WriteLiteralTo(__razor_helper_writer, "        ");


#line 32 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, Html.Hidden(ctx.SubContextPrefix(a => a.Property), ctx.Value.Property.Path));


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\r\n");

WriteLiteralTo(__razor_helper_writer, "        ");


#line 33 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, Html.TextArea(ctx.SubContextPrefix(a => a.Description), ph.UserDescription, new { @class = "editable" }));


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "\r\n        <span");

WriteLiteralTo(__razor_helper_writer, " class=\"wiki\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 34 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, Html.WikiParse(ph.UserDescription, HelpWiki.DefaultWikiSettings));


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</span>\r\n    </dd>\r\n");


#line 36 "..\..\Help\Views\ViewEntity.cshtml"
        if (node.Children.Count > 0)
        {


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    <h4");

WriteLiteralTo(__razor_helper_writer, " class=\"embedded\"");

WriteLiteralTo(__razor_helper_writer, ">");


#line 38 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, ph.PropertyInfo.NiceName());


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "</h4>\r\n");

WriteLiteralTo(__razor_helper_writer, "    <dl");

WriteLiteralTo(__razor_helper_writer, " class=\"embedded\"");

WriteLiteralTo(__razor_helper_writer, ">\r\n");


#line 40 "..\..\Help\Views\ViewEntity.cshtml"
        

#line default
#line hidden

#line 40 "..\..\Help\Views\ViewEntity.cshtml"
         foreach (var v in node.Children)
        {
            

#line default
#line hidden

#line 42 "..\..\Help\Views\ViewEntity.cshtml"
WriteTo(__razor_helper_writer, WriteProperty(v, entityName));


#line default
#line hidden

#line 42 "..\..\Help\Views\ViewEntity.cshtml"
                                         ;
        }


#line default
#line hidden
WriteLiteralTo(__razor_helper_writer, "    </dl>\r\n");


#line 45 "..\..\Help\Views\ViewEntity.cshtml"
        }
    }


#line default
#line hidden
});

#line 47 "..\..\Help\Views\ViewEntity.cshtml"
}
#line default
#line hidden

        public ViewEntity()
        {
        }
        public override void Execute()
        {
DefineSection("head", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 12 "..\..\Help\Views\ViewEntity.cshtml"
Write(Html.ScriptCss("~/help/Content/help.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

});

WriteLiteral("\r\n");

            
            #line 15 "..\..\Help\Views\ViewEntity.cshtml"
   
    EntityHelp eh = (EntityHelp)Model;
    ViewBag.Title = eh.Type.NiceName();

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"col-md-9\"");

WriteLiteral(">\r\n");

            
            #line 52 "..\..\Help\Views\ViewEntity.cshtml"
        
            
            #line default
            #line hidden
            
            #line 52 "..\..\Help\Views\ViewEntity.cshtml"
         using (TypeContext<EntityHelpDN> ec = new TypeContext<EntityHelpDN>(eh.Entity.Value, null))
        {
            var name = Navigator.ResolveWebTypeName(eh.Type);
            ec.FormGroupStyle = FormGroupStyle.None;


            using (Html.BeginForm((HelpController hc) => hc.SaveEntity(), new { id = "form-save" }))
            {
            
            
            #line default
            #line hidden
            
            #line 60 "..\..\Help\Views\ViewEntity.cshtml"
       Write(Html.HiddenRuntimeInfo(ec));

            
            #line default
            #line hidden
            
            #line 60 "..\..\Help\Views\ViewEntity.cshtml"
                                       
            
            
            #line default
            #line hidden
            
            #line 61 "..\..\Help\Views\ViewEntity.cshtml"
       Write(Html.HiddenRuntimeInfo(ec, e => e.Culture));

            
            #line default
            #line hidden
            
            #line 61 "..\..\Help\Views\ViewEntity.cshtml"
                                                       
            
            
            #line default
            #line hidden
            
            #line 62 "..\..\Help\Views\ViewEntity.cshtml"
       Write(Html.HiddenRuntimeInfo(ec, e => e.Type));

            
            #line default
            #line hidden
            
            #line 62 "..\..\Help\Views\ViewEntity.cshtml"
                                                    
                if (!Navigator.IsReadOnly(typeof(EntityHelpDN)))
                {
                    Html.RenderPartial(HelpClient.Buttons, new ViewDataDictionary
                    {
                        { "options",  Database.Query<EntityHelpDN>()
                        .Where(e => e.Type == ec.Value.Type && e.Culture != ec.Value.Culture)
                        .Select(e => new { e.Culture })
                        .ToList()
                        .Select(e => KVP.Create(e.Culture.ToCultureInfo(), this.Url.Action((HelpController a) => a.TraslateEntity(e.Culture.Name))))
                        .ToDictionary() }
                    });
                }

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"edit-container\"");

WriteLiteral(">\r\n                <h1");

WriteAttribute("title", Tuple.Create(" title=\"", 2854), Tuple.Create("\"", 2880)
            
            #line 76 "..\..\Help\Views\ViewEntity.cshtml"
, Tuple.Create(Tuple.Create("", 2862), Tuple.Create<System.Object, System.Int32>(eh.Type.Namespace
            
            #line default
            #line hidden
, 2862), false)
);

WriteLiteral(">");

            
            #line 76 "..\..\Help\Views\ViewEntity.cshtml"
                                          Write(eh.Type.NiceName());

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n                <code");

WriteLiteral(" class=\'shortcut\'");

WriteLiteral(">[e:");

            
            #line 77 "..\..\Help\Views\ViewEntity.cshtml"
                                     Write(name);

            
            #line default
            #line hidden
WriteLiteral("]</code>\r\n                <span");

WriteLiteral(" class=\"info\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 79 "..\..\Help\Views\ViewEntity.cshtml"
               Write(Html.WikiParse(eh.Info, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </span>\r\n");

WriteLiteral("                ");

            
            #line 81 "..\..\Help\Views\ViewEntity.cshtml"
           Write(Html.TextArea(ec.SubContextPrefix(a => a.Description), eh.Description, 5, 80, new { @class = "editable" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                <span");

WriteLiteral(" class=\"wiki\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 83 "..\..\Help\Views\ViewEntity.cshtml"
               Write(Html.WikiParse(eh.Description, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </span>\r\n            </div>\r\n");

WriteLiteral("            <div");

WriteLiteral(" id=\"entityContent\"");

WriteLiteral(" class=\"help_left\"");

WriteLiteral(">\r\n");

            
            #line 87 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 87 "..\..\Help\Views\ViewEntity.cshtml"
                  
                var allowedProps = eh.Properties.Where(a => a.Value.IsAllowed() == null).ToDictionary();
                
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 91 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 91 "..\..\Help\Views\ViewEntity.cshtml"
                 if (allowedProps.Any())
                {

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" id=\"properties\"");

WriteLiteral(">\r\n                        <h2");

WriteLiteral(" class=\"greyTitle\"");

WriteLiteral(">");

            
            #line 94 "..\..\Help\Views\ViewEntity.cshtml"
                                         Write(Html.PropertyNiceName(() => eh.Entity.Value.Properties));

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n");

            
            #line 95 "..\..\Help\Views\ViewEntity.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 95 "..\..\Help\Views\ViewEntity.cshtml"
                           
                    var elementContexts = ec.TypeElementContext(e => e.Properties).ToDictionary(a => a.Value.Property.Path);

                    var tuplesDic = allowedProps.Values.Select(p => Tuple.Create(p, elementContexts.GetOrThrow(p.PropertyRoute.PropertyString()))).ToDictionary(a => a.Item1.PropertyRoute);

                    var roots = TreeHelper.ToTreeC(tuplesDic.Values, kvp =>
                    {
                        var parent = kvp.Item1.PropertyRoute.Parent;
                        if (parent.PropertyRouteType == PropertyRouteType.Root || parent.PropertyRouteType == PropertyRouteType.Mixin)
                            return null;

                        if (parent.PropertyRouteType == PropertyRouteType.MListItems)
                            parent = parent.Parent;

                        return tuplesDic.GetOrThrow(parent);
                    });
                        
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                        <dl");

WriteLiteral(" class=\"dl-horizontal\"");

WriteLiteral(">\r\n");

            
            #line 114 "..\..\Help\Views\ViewEntity.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 114 "..\..\Help\Views\ViewEntity.cshtml"
                             foreach (var node in roots)
                            {
                                
            
            #line default
            #line hidden
            
            #line 116 "..\..\Help\Views\ViewEntity.cshtml"
                           Write(WriteProperty(node, name));

            
            #line default
            #line hidden
            
            #line 116 "..\..\Help\Views\ViewEntity.cshtml"
                                                          ;
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </dl>\r\n                    </div>\r\n");

            
            #line 120 "..\..\Help\Views\ViewEntity.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 122 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 122 "..\..\Help\Views\ViewEntity.cshtml"
                  
                var allowedOperations = eh.Operations.Where(a => a.Value.IsAllowed() == null).ToDictionary();
                
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 126 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 126 "..\..\Help\Views\ViewEntity.cshtml"
                 if (allowedOperations.Any())
                {
                    var operations = ec.TypeElementContext(e => e.Operations).ToDictionary(a => a.Value.Operation);
                         

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" id=\"operations\"");

WriteLiteral(">\r\n                        <h2");

WriteLiteral(" class=\"greyTitle\"");

WriteLiteral(">");

            
            #line 131 "..\..\Help\Views\ViewEntity.cshtml"
                                         Write(Html.PropertyNiceName(() => eh.Entity.Value.Operations));

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n                        <dl");

WriteLiteral(" class=\"dl-horizontal\"");

WriteLiteral(">\r\n");

            
            #line 133 "..\..\Help\Views\ViewEntity.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 133 "..\..\Help\Views\ViewEntity.cshtml"
                             foreach (var op in allowedOperations)
                            {
                                using (TypeElementContext<OperationHelpDN> ctx = operations.GetOrThrow(op.Key))
                                {

            
            #line default
            #line hidden
WriteLiteral("                                <dt");

WriteAttribute("id", Tuple.Create(" id=\"", 5927), Tuple.Create("\"", 5961)
            
            #line 137 "..\..\Help\Views\ViewEntity.cshtml"
, Tuple.Create(Tuple.Create("", 5932), Tuple.Create<System.Object, System.Int32>(HelpUrls.IdOperation(op.Key)
            
            #line default
            #line hidden
, 5932), false)
);

WriteLiteral(">");

            
            #line 137 "..\..\Help\Views\ViewEntity.cshtml"
                                                                  Write(op.Key.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("<code");

WriteLiteral(" class=\'shortcut\'");

WriteLiteral(">[o:");

            
            #line 137 "..\..\Help\Views\ViewEntity.cshtml"
                                                                                                                  Write(op.Key.Key);

            
            #line default
            #line hidden
WriteLiteral("]</code></dt>\r\n");

WriteLiteral("                                <dd>\r\n                                    <span");

WriteLiteral(" class=\"info\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

            
            #line 140 "..\..\Help\Views\ViewEntity.cshtml"
                                   Write(Html.WikiParse(op.Value.Info, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </span>\r\n");

WriteLiteral("                                    ");

            
            #line 142 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.HiddenRuntimeInfo(ctx));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 143 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.HiddenRuntimeInfo(ctx, e => e.Culture));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 144 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.HiddenRuntimeInfo(ctx, a => a.Operation));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 145 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.TextArea(ctx.SubContextPrefix(a => a.Description), op.Value.UserDescription, new { @class = "editable" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    <span");

WriteLiteral(" class=\"wiki\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

            
            #line 147 "..\..\Help\Views\ViewEntity.cshtml"
                                   Write(Html.WikiParse(op.Value.UserDescription, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </span>\r\n                                </" +
"dd>\r\n");

            
            #line 150 "..\..\Help\Views\ViewEntity.cshtml"
                                }
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </dl>\r\n                    </div>\r\n");

            
            #line 154 "..\..\Help\Views\ViewEntity.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 156 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 156 "..\..\Help\Views\ViewEntity.cshtml"
                  
                var allowedQueries = eh.Queries.Where(a => a.Value.IsAllowed() == null).ToDictionary();
                
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 160 "..\..\Help\Views\ViewEntity.cshtml"
                
            
            #line default
            #line hidden
            
            #line 160 "..\..\Help\Views\ViewEntity.cshtml"
                 if (allowedQueries.Any())
                {
                    var queries = ec.TypeElementContext(e => e.Queries).ToDictionary(a => a.Value.Query);
                        

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" id=\"queries\"");

WriteLiteral(">\r\n                        <h2");

WriteLiteral(" class=\"greyTitle\"");

WriteLiteral(">");

            
            #line 165 "..\..\Help\Views\ViewEntity.cshtml"
                                         Write(typeof(QueryDN).NicePluralName());

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n");

            
            #line 166 "..\..\Help\Views\ViewEntity.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 166 "..\..\Help\Views\ViewEntity.cshtml"
                         foreach (var mq in allowedQueries)
                        {
                            using (TypeElementContext<QueryHelpDN> qctx = queries.GetOrThrow(QueryLogic.GetQuery(mq.Key)))
                            {
                                        
                            
            
            #line default
            #line hidden
            
            #line 171 "..\..\Help\Views\ViewEntity.cshtml"
                       Write(Html.HiddenRuntimeInfo(qctx));

            
            #line default
            #line hidden
            
            #line 171 "..\..\Help\Views\ViewEntity.cshtml"
                                                          
                            
            
            #line default
            #line hidden
            
            #line 172 "..\..\Help\Views\ViewEntity.cshtml"
                       Write(Html.HiddenRuntimeInfo(qctx, e => e.Culture));

            
            #line default
            #line hidden
            
            #line 172 "..\..\Help\Views\ViewEntity.cshtml"
                                                                                                            
                            
            
            #line default
            #line hidden
            
            #line 173 "..\..\Help\Views\ViewEntity.cshtml"
                       Write(Html.HiddenRuntimeInfo(qctx, a => a.Query));

            
            #line default
            #line hidden
            
            #line 173 "..\..\Help\Views\ViewEntity.cshtml"
                                                                                                           

            
            #line default
            #line hidden
WriteLiteral("                            <h3");

WriteAttribute("id", Tuple.Create(" id=\"", 8123), Tuple.Create("\"", 8153)
            
            #line 174 "..\..\Help\Views\ViewEntity.cshtml"
, Tuple.Create(Tuple.Create("", 8128), Tuple.Create<System.Object, System.Int32>(HelpUrls.IdQuery(mq.Key)
            
            #line default
            #line hidden
, 8128), false)
);

WriteLiteral(">");

            
            #line 174 "..\..\Help\Views\ViewEntity.cshtml"
                                                          Write(QueryUtils.GetNiceName(mq.Key));

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");

WriteLiteral("                            <div");

WriteLiteral(" class=\"queryName\"");

WriteLiteral(">\r\n                                <code");

WriteLiteral(" class=\'shortcut\'");

WriteLiteral(">[q:");

            
            #line 176 "..\..\Help\Views\ViewEntity.cshtml"
                                                     Write(QueryUtils.GetQueryUniqueKey(mq.Key));

            
            #line default
            #line hidden
WriteLiteral("]</code>\r\n                                <span");

WriteLiteral(" class=\"info\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 178 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.WikiParse(mq.Value.Info, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </span>\r\n");

WriteLiteral("                                ");

            
            #line 180 "..\..\Help\Views\ViewEntity.cshtml"
                           Write(Html.TextArea(qctx.SubContextPrefix(a => a.Description), mq.Value.UserDescription, new { @class = "editable" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                <span");

WriteLiteral(" class=\"wiki\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 182 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.WikiParse(mq.Value.UserDescription, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </span>\r\n                            </div>\r\n");

            
            #line 185 "..\..\Help\Views\ViewEntity.cshtml"

                                var allowedColumn = mq.Value.Columns.Where(a => a.Value.IsAllowed() == null);                               
                                if (allowedColumn.Any())
                                {
                                    var columns = qctx.TypeElementContext(a => a.Columns).ToDictionary(a => a.Value.ColumnName);
                                    

            
            #line default
            #line hidden
WriteLiteral("                            <dl");

WriteLiteral(" class=\"dl-horizontal columns\"");

WriteLiteral(">\r\n");

            
            #line 192 "..\..\Help\Views\ViewEntity.cshtml"
                                
            
            #line default
            #line hidden
            
            #line 192 "..\..\Help\Views\ViewEntity.cshtml"
                                 foreach (var qc in allowedColumn)
                                {
                                    using (var ctx = columns.GetOrThrow(qc.Value.Column.Name))
                                    {
                                    
            
            #line default
            #line hidden
            
            #line 196 "..\..\Help\Views\ViewEntity.cshtml"
                               Write(Html.HiddenRuntimeInfo(ctx));

            
            #line default
            #line hidden
            
            #line 196 "..\..\Help\Views\ViewEntity.cshtml"
                                                                     

            
            #line default
            #line hidden
WriteLiteral("                                    <dt>");

            
            #line 197 "..\..\Help\Views\ViewEntity.cshtml"
                                   Write(qc.Value.NiceName);

            
            #line default
            #line hidden
WriteLiteral("</dt>\r\n");

WriteLiteral("                                    <dd>\r\n                                       " +
" <span");

WriteLiteral(" class=\"info\"");

WriteLiteral(">\r\n");

WriteLiteral("                                            ");

            
            #line 200 "..\..\Help\Views\ViewEntity.cshtml"
                                       Write(Html.WikiParse(qc.Value.Info, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </span>\r\n");

WriteLiteral("                                        ");

            
            #line 202 "..\..\Help\Views\ViewEntity.cshtml"
                                   Write(Html.Hidden(ctx.SubContextPrefix(a => a.ColumnName), ctx.Value.ColumnName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                        ");

            
            #line 203 "..\..\Help\Views\ViewEntity.cshtml"
                                   Write(Html.TextArea(ctx.SubContextPrefix(a => a.Description), ctx.Value.Description, new { @class = "editable" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        <span");

WriteLiteral(" class=\"wiki\"");

WriteLiteral(">\r\n");

WriteLiteral("                                            ");

            
            #line 205 "..\..\Help\Views\ViewEntity.cshtml"
                                       Write(Html.WikiParse(qc.Value.UserDescription, HelpWiki.DefaultWikiSettings));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </span>\r\n                              " +
"      </dd>\r\n");

            
            #line 208 "..\..\Help\Views\ViewEntity.cshtml"
                                    }
                                }

            
            #line default
            #line hidden
WriteLiteral("                            </dl>\r\n");

            
            #line 211 "..\..\Help\Views\ViewEntity.cshtml"
                                }
                            }

                        }

            
            #line default
            #line hidden
WriteLiteral("                    </div>\r\n");

            
            #line 216 "..\..\Help\Views\ViewEntity.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n");

            
            #line 218 "..\..\Help\Views\ViewEntity.cshtml"
            }
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n\r\n    <div");

WriteLiteral(" class=\"col-md-3\"");

WriteLiteral(">\r\n");

            
            #line 223 "..\..\Help\Views\ViewEntity.cshtml"
        
            
            #line default
            #line hidden
            
            #line 223 "..\..\Help\Views\ViewEntity.cshtml"
           Html.RenderPartial(HelpClient.MiniMenu, new ViewDataDictionary { { "type", eh.Type } });
            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
