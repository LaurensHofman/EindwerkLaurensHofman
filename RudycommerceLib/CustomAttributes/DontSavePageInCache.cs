using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RudycommerceLib.CustomAttributes
{
    public class DontSavePageInCache : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AppendHeader("Cache-Control", "no-store");
        }
    }
}
