using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/Fundamentos
        public ActionResult Fundamentos()
        {
            return View();
        }

        //
        // GET: /Help/Manual
        public ActionResult Manual()
        {
            return View();
        }

        //
        // GET: /Help/Fundamentos
        public ActionResult Basics()
        {
            return View();
        }

        //
        // GET: /Help/ManualBasico
        public ActionResult ManualBasico()
        {
            return View();
        }
	}
}