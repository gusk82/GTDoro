using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
using System.Collections.Specialized;

namespace GTDoro.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        private IPrincipal testUser;
        protected new IPrincipal User { get {  return testUser ?? base.User;  } }
        public IPrincipal TestUser 
        { 
            set { testUser = value; }
        }

        protected new HttpNotFoundResult HttpNotFound(string statusDescription = null)
        {
            return new HttpNotFoundResult(statusDescription);
        }

        protected HttpUnauthorizedResult HttpUnauthorized(string statusDescription = null)
        {
            return new HttpUnauthorizedResult(statusDescription);
        }

        protected class HttpNotFoundResult : HttpStatusCodeResult
        {
            public HttpNotFoundResult() : this(null) { }

            public HttpNotFoundResult(string statusDescription) : base(HttpStatusCode.NotFound, statusDescription) { }

        }

        protected override void HandleUnknownAction(string actionName)
        {
            if (this.GetType() != typeof(BaseController))
            {
                var errorRoute = new RouteData();
                errorRoute.Values.Add("controller", "Base");
                errorRoute.Values.Add("action", "Http404");
                errorRoute.Values.Add("url", HttpContext.Request.Url.OriginalString);

                View("~/Views/Base/Http404.cshtml").ExecuteResult(this.ControllerContext);
            }
        }

        [AllowAnonymous]
        public ActionResult Http404()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Error()
        {
            return View();
        }

        protected static string[] GetArrayParamsFromRequest(NameValueCollection queryString, string prefix)
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < queryString.Count; i++)
            {
                if (queryString.GetKey(i).StartsWith(prefix))
                {
                    if (queryString[queryString.GetKey(i)] == "1")
                    {
                        tags.Add(queryString.GetKey(i).Substring(prefix.Length));
                    }
                }
            }
            return tags.ToArray();
        }
        
        protected class HttpStatusCodeResult : ViewResult
        {
            public HttpStatusCode StatusCode { get; private set; }
            public string StatusDescription { get; private set; }

            public HttpStatusCodeResult(HttpStatusCode statusCode) : this(statusCode, null) { }

            public HttpStatusCodeResult(HttpStatusCode statusCode, string statusDescription)
            {
                this.StatusCode = statusCode;
                this.StatusDescription = statusDescription;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                context.HttpContext.Response.StatusCode = (int)this.StatusCode;
                if (this.StatusDescription != null)
                {
                    context.HttpContext.Response.StatusDescription = this.StatusDescription;
                }
                // 1. Uncomment this to use the existing Error.ascx / Error.cshtml to view as an error or
                // 2. Uncomment this and change to any custom view and set the name here or simply
                // 3. (Recommended) Let it commented and the ViewName will be the current controller view action and on your view (or layout view even better) show the @ViewBag.Message to produce an inline message that tell the Not Found or Unauthorized
                this.ViewName = "~/Views/Base/Http404.cshtml";
                this.ViewBag.Message = context.HttpContext.Response.StatusDescription;
                base.ExecuteResult(context);
            }
        }
    }
}