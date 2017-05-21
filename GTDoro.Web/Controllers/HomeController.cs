using GTDoro.Core.Models.Identity;
using System.Web.Mvc;

namespace GTDoro.Controllers
{
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(Request.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Layout");
            }
            //if (TempData["RegistrationError"] != null && !string.IsNullOrWhiteSpace(TempData["RegistrationError"] as string))
            //{
            //    ModelState.AddModelError("", TempData["RegistrationError"] as string);
            //    TempData["RegistrationError"] = string.Empty;
            //}
            //delete this and the parameter for production env.

                        //return RedirectToAction("RegisterForm","Account");
            return View("~/Views/Account/RegisterForm.cshtml"/*, "~/Views/Shared/Layouts/_Landing.cshtml"*/);
        }

        public ActionResult DisplayEmail(string RegistrationLink)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(string.Empty, string.Empty);
            }

            ViewBag.RegistrationLink = RegistrationLink;
            return View("~/Views/Account/DisplayEmail.cshtml");
        }

        public ActionResult ThankYouForRegistering()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(string.Empty, string.Empty);
            }

            return View("~/Views/Account/ThankYouForRegistering.cshtml");
        }

        public ActionResult ConfirmEmail(string IsEmailConfirmation)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction(string.Empty, string.Empty);
            }

            ViewBag.IsEmailConfirmation = IsEmailConfirmation;
            return View("~/Views/Account/ConfirmEmail.cshtml");
        }
    }
}
