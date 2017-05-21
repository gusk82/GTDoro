using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GTDoro.Web.Models.ActionFilters
{
    public class AdminActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.Name != Settings.ADMIN_USERNAME)
            {
                filterContext.Result = new RedirectResult(Settings.GetError404Path(filterContext.HttpContext.Request.FilePath));
                return; 
            }
        }
    }

    public class DisabledActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectResult(Settings.GetError404Path(filterContext.HttpContext.Request.FilePath));
            return;
        }
    }
}