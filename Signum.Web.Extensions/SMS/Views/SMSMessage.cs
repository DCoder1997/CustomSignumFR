﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Caching;
    using System.Web.DynamicData;
    using System.Web.SessionState;
    using System.Web.Profile;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Xml.Linq;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Signum.Utilities;
    using Signum.Entities;
    using Signum.Web;
    using Signum.Engine;
    using Signum.Entities.SMS;
    using Signum.Web.SMS;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/SMS/Views/SMSMessage.cshtml")]
    public class _Page_SMS_Views_SMSMessage_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {


        public _Page_SMS_Views_SMSMessage_cshtml()
        {
        }
        protected System.Web.HttpApplication ApplicationInstance
        {
            get
            {
                return ((System.Web.HttpApplication)(Context.ApplicationInstance));
            }
        }
        public override void Execute()
        {




WriteLiteral("\r\n");


Write(Html.ScriptCss("~/SMS/Content/SMS.css"));

WriteLiteral("\r\n\r\n");


 using (var e = Html.TypeContext<SMSMessageDN>())
{   
    
Write(Html.ValueLine(e, s => s.MessageID, vl =>
    {
        vl.Visible = !e.Value.IsNew;
        vl.ReadOnly = true;
    }));

      
    
Write(Html.EntityLine(e, s => s.Template, vl => 
    {
        vl.Create = false;
        vl.Remove = false;
        vl.HideIfNull = true;
    }));

      
    
Write(Html.ValueLine(e, s => s.DestinationNumber, vl => vl.ReadOnly = !e.Value.IsNew));

                                                                                    
    
Write(Html.ValueLine(e, s => s.Message, vl =>
        {
            vl.ValueLineType = ValueLineType.TextArea;
            vl.ValueHtmlProps["cols"] = "30";
            vl.ValueHtmlProps["rows"] = "6";
            vl.ReadOnly = (e.Value.State != SMSMessageState.Created);
        }));

          
    if(e.Value.State == SMSMessageState.Created) {

WriteLiteral("        <div id=\"sfCharactersLeft\" data-url=\"");


                                         Write(Url.Action<SMSController>(s => s.GetDictionaries()));

WriteLiteral("\">\r\n            <p>Remaining characters: <span id=\"sfCharsLeft\"></span></p>\r\n    " +
"    </div>\r\n");



WriteLiteral("        <div>\r\n            <input type=\"button\" class=\"sf-button\" id=\"sfRemoveNoS" +
"MSChars\" value=\"Remove non valid characters\" data-url=\"");


                                                                                                                     Write(Url.Action<SMSController>(s => s.RemoveNoSMSCharacters("")));

WriteLiteral("\"/>\r\n        </div>\r\n");


    }

WriteLiteral("    <br />\r\n");


    
Write(Html.ValueLine(e, s => s.From, vl => vl.ReadOnly = (e.Value.State != SMSMessageState.Created)));

                                                                                                   

    if (e.Value.State != SMSMessageState.Created)
    {
        
   Write(Html.ValueLine(e, s => s.SendDate, vl => vl.ReadOnly = true));

                                                                     
        
   Write(Html.ValueLine(e, s => s.State, vl => vl.ReadOnly = true));

                                                                  
    }
}

WriteLiteral("\r\n");


        }
    }
}
