using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RudycommerceLib.CustomAttributes
{
    /// <summary>
    /// Allows a page to not be saved in the cache. This will result that if people revisit the page for example by using the back button, 
    /// it will be forced to reload and for example call the HttpGet method in the controller.
    /// </summary>
    public class DontSavePageInCache : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AppendHeader("Cache-Control", "no-store");
        }
    }
}
