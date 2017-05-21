using GTDoro.Core.DAL;
using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using GTDoro.Core.Models.Identity;
using GTDoro.Core.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTDoro.Web.Models.ActionFilters;

namespace GTDoro.Controllers
{
    [Authorize]
    public class TestController : BaseController
    {
        private GTDoroContext db;
        private UserManager<ApplicationUser> manager;

        #region Disabled actions

        public TestController()
        {
            db = new GTDoroContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        //
        // GET: /Test/
        [DisabledActionFilter]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost, ActionName("TestPanels")]
        [DisabledActionFilter]
        public ActionResult Start()
        {
            Pomodoro pomodoro = new Pomodoro();
            pomodoro.Start = DateTime.UtcNow;
            pomodoro.Status = PomodoroStatus.Working;
            pomodoro.ActionID = Int32.Parse(Request.Form["ActionID"]);
            db.Entry(pomodoro).State = EntityState.Modified;
            db.Pomodoros.Add(pomodoro);
            db.SaveChanges();
            return RedirectToAction("TestPanels");
        }
        
        [DisabledActionFilter]
        public ViewResult Accordion()
        {
            IEnumerable<Project> projects = db.Projects
                   .Include(p => p.Tasks.Select(t => t.Actions));
            return View(projects);
        }

        #endregion
    }
}