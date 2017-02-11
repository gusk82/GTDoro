using GTDoro.DAL;
using GTDoro.Models;
using GTDoro.Models.ActionFilters;
using GTDoro.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.Controllers
{
    [Authorize]
    public class WorkspaceController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();
        
        // POST: /Project/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject([Bind(Include = "ID,Code,Name,Description,Status,EndDate")] Project newProject)
        {
            int collectedThingID = 0;
            if (Request != null)
            {
                Int32.TryParse(Request.Params["collectedThingID"], out collectedThingID);
            }
            //set owner
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser appUser = manager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                //set owner
                newProject.User = appUser;

                //set creation date
                newProject.CreationDate = DateTime.UtcNow;

                //create project
                db.Projects.Add(newProject);
                if (collectedThingID > 0)
                {
                    //delete associated collected thing
                    CollectedThing collectedThing = db.GetCollectedThingById(User, collectedThingID);
                    if (collectedThing != null)
                    {
                        db.CollectedThings.Remove(collectedThing);
                    }
                }

                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_CREATE;
                 
                if (Request != null && Url != null) //avoid null reference exceptions when testing
                {
                    string button = Request.Form["submitButton"];
                    if (button == "1")
                    {
                        return JavaScript("window.location = '" + Url.Action("Index") + "'");
                    }
                    return JavaScript("window.location = '" + Url.Action("Details", "Project", new { id = newProject.ID }) + "'");
                }
            }

            ViewBag.NewProject = newProject;
            return PartialView(new Workspace(appUser));
        }

        public ActionResult Index(int? ct, string text)
        {
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser user = manager.FindById(User.Identity.GetUserId());

            Project newProject = new Project();
            newProject.User = user;
            if (ct.HasValue)
            {
                CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                if (collectedthing != null)
                {
                    newProject.SetCode(collectedthing.Name);
                    newProject.SetName(collectedthing.Name);
                    ViewBag.collectedThingID = ct.Value;
                }
            }
            else if (string.IsNullOrWhiteSpace(text) == false)
            {
                newProject.SetCode(text);
                newProject.SetName(text);
            }
            ViewBag.NewProject = newProject;

            return View("Workspace", new Workspace(user));
        }
        
        public ActionResult CreateProject(int? ct, string text)
        {
            return RedirectToAction("Index", new { ct = ct, text = text });
        }
        #region Disabled Actions

        // GET: /Project/Create
        [DisabledActionFilter]
        public ActionResult CreateProjectOld(int? ct, string text)
        {
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser user = manager.FindById(User.Identity.GetUserId());

            Project newProject = new Project();
            newProject.User = user;
            if (ct.HasValue)
            {
                CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                if (collectedthing != null)
                {
                    newProject.SetCode(collectedthing.Name);
                    newProject.Name = collectedthing.Name;
                    ViewBag.collectedThingID = ct.Value;
                }
            }
            else if (string.IsNullOrWhiteSpace(text) == false)
            {
                newProject.SetCode(text);
                newProject.Name = text;
            }
            ViewBag.NewProject = newProject;

            return PartialView("~/Views/Workspace/CreateProject.cshtml", new Workspace(user));
        }
        #endregion
    }
}