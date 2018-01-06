using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTDoro.Core.Models;
using GTDoro.Core.DAL;
using GTDoro.Core.Models.Identity;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Web.Models.ActionFilters;
using System.Data.Entity.Infrastructure;
using System.Web.Routing;
using GTDoro.Core.Extensions;
using GTDoro.Web.Extensions;

namespace GTDoro.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Project/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CodeSortParm = (sortOrder == "code") ? "code_desc" : "code";
            ViewBag.NameSortParm = (sortOrder == "name") ? "name_desc" : "name";
            ViewBag.LastPomodoroSortParm = (sortOrder == "last") ? "last_desc" : "last";
            ViewBag.PomodorosSortParm = (sortOrder == "pomo") ? "pomo_desc" : "pomo";
            ViewBag.ProgressSortParm = (sortOrder == "prog") ? "prog_desc" : "prog";
            ViewBag.EffortSortParm = (sortOrder == "effo") ? "effo_desc" : "effo";
            ViewBag.StatusSortParm = (sortOrder == "stat") ? "stat_desc" : "stat";
            ViewBag.CreationDateSortParm = (sortOrder == "crea") ? "crea_desc" : "crea";
            ViewBag.TasksSortParm = (sortOrder == "task") ? "task_desc" : "task";
            ViewBag.ActionsSortParm = (sortOrder == "acti") ? "acti_desc" : "acti";

            var projects = db.GetMyProjects(User);

            //stores the current filter info for the pagination control
            RouteValueDictionary dict = new RouteValueDictionary();
            //initilised with the current sort order
            dict["CurrentSort"] = sortOrder;

            if (Request != null) //avoid null reference exceptions when testing
            {
                //status filter
                const string STATUS_PREFIX = "status-";
                string[] statusFilter = GetArrayParamsFromRequest(Request.QueryString, STATUS_PREFIX);
                if (statusFilter.Length > 0)
                {
                    projects = projects.ToList().AsQueryable()
                        .Where(p => statusFilter
                            .Any(sf => sf.ToLower() == p.CalculatedStatus.ToString().ToLower())
                    );
                    dict.AddFilterArrayToRouteValueDictionary(statusFilter, STATUS_PREFIX);
                }

                //tag filter
                const string TAG_PREFIX = "tag-";
                string[] tagFilter = GetArrayParamsFromRequest(Request.QueryString, TAG_PREFIX);
                if (tagFilter.Length > 0)
                {
                    projects = projects.ToList().AsQueryable()
                        .Where(p => tagFilter
                            .Any(tf => p.OwnAndInheritedTags    //all tags in the filter
                                .Select(tc => tc.Code)          //(only tag code list)
                                    .Contains(tf))              //contained in the item tags 
                    );
                    //AND Tag Filter
                    //projects = projects.ToList().AsQueryable()
                    //    .Where(p => tagFilter
                    //        .All(tf => p.OwnAndInheritedTags    //all tags in the filter
                    //            .Select(tc => tc.Code)          //(only tag code list)
                    //                .Contains(tf))              //contained in the item tags 
                    //);
                    dict.AddFilterArrayToRouteValueDictionary(tagFilter, TAG_PREFIX);
                }
                                
                ViewBag.IsFiltered = string.IsNullOrEmpty(Request.QueryString["filtering"]) == false ||
                    (statusFilter.Length > 0) || (tagFilter.Length > 0);
                ViewBag.RouteFiltersForPagination = dict;

                ViewBag.StatusFilter = statusFilter;
                ViewBag.TagFilter = tagFilter;
            }
            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}
            //ViewBag.CurrentFilter = searchString;

            ViewBag.AllTags = db.GetMyTags(User).ToArray();

            switch (sortOrder)
            {
                case "code":
                    projects = projects.OrderBy(p => p.Code);
                    break;
                case "code_desc":
                    projects = projects.OrderByDescending(p => p.Code);
                    break;
                case "name":
                    projects = projects.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(p => p.Name);
                    break;
                case "last":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "last_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "pomo":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.CompletedPomodorosCount);
                    break;
                case "pomo_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.CompletedPomodorosCount);
                    break;
                case "prog":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.Progress.ToDecimalOrZero());
                    break;
                case "prog_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.Progress.ToDecimalOrZero());
                    break;
                case "effo":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.Effort.ToDecimalOrZero()).ThenBy(p => p.CompletedPomodorosCount);
                    break;
                case "effo_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.Effort.ToDecimalOrZero()).ThenByDescending(p => p.CompletedPomodorosCount);
                    break;
                case "stat":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.CalculatedStatus);
                    break;
                case "stat_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.CalculatedStatus);
                    break;
                case "crea":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.CreationDateLocal.ToTicksOrZero());
                    break;
                case "crea_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.CreationDateLocal.ToTicksOrZero());
                    break;
                case "task":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.Tasks.Count());
                    break;
                case "task_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.Tasks.Count());
                    break;
                case "acti":
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.GetActions().Count());
                    break;
                case "acti_desc":
                    projects = projects.ToList().AsQueryable().OrderByDescending(p => p.GetActions().Count());
                    break;
                default:
                    projects = projects.ToList().AsQueryable().OrderBy(p => p.Status).ThenByDescending(p => p.LastPomodoro.ToDateTicksOrZero());
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));
        }

        // POST: /Project/
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit(int[] selectedProjects, int? newStatus)
        {
            if (selectedProjects != null && selectedProjects.Length > 0)
            {
                string[] selectedTags = GetArrayParamsFromRequest(Request.Form, "tag-");

                if (newStatus.HasValue || selectedTags.Length > 0)
                {
                    var projectsToUpdate = db.GetMyProjects(User)
                       .Include(t => t.Tags)
                       .Where(p => selectedProjects.ToList().Contains(p.ID)).ToList();

                    try
                    {
                        foreach (var project in projectsToUpdate)
                        {
                            if (newStatus.HasValue)
                            {
                                project.Status = (Status)newStatus.Value;
                            }
                            if (selectedTags != null && selectedTags.Length > 0)
                            {
                                SetTagsToProject(selectedTags, project);
                            }
                            db.Entry(project).State = EntityState.Modified;

                        }
                        TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;

                        db.SaveChanges();
                    }
                    catch (RetryLimitExceededException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // POST: /Project/CreateTask/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(Include = "ID,Code,Name,Description,EndDate,ProjectID,Status,Priority")] Task newTask)
        {
            if (ModelState.IsValid)
            {
                //set creation date
                newTask.CreationDate = DateTime.UtcNow;
                db.Tasks.Add(newTask);

                //delete associated collected thing
                int collectedThingID = 0;
                if (Request != null)
                {
                    Int32.TryParse(Request.Params["collectedThingID"], out collectedThingID);
                    if (collectedThingID > 0)
                    {
                        CollectedThing collectedThing = db.GetCollectedThingById(User, collectedThingID);
                        if (collectedThing != null)
                        {
                            db.CollectedThings.Remove(collectedThing);
                        }
                    }
                }

                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_CREATE;
                if (Request != null && Url != null) //avoid null reference exceptions when testing
                {
                    string button = Request.Form["submitButton"];
                    if (button == "1")
                    {
                        return JavaScript("window.location = '" + Url.Action("CreateTask", new { id = newTask.ProjectID }) + "'");
                    }
                    return JavaScript("window.location = '" + Url.Action("Details", "Task", new { id = newTask.ID }) + "'");
                }
            }
            //load project for the view
            newTask.Project = db.GetProjectById(User, newTask.ProjectID);
            ViewBag.NewTask = newTask;
            return PartialView(newTask.Project);
            //return RedirectToAction("Details", new { id = newTask.ProjectID, op = "create" });
        }


        // POST: /Project/CreateTask/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSprint([Bind(Include = "ID,Name,StartDate,EndDate,ProjectID,InitialComment,FinalComment,CreationDate")] Sprint newSprint)
        {
            if (ModelState.IsValid)
            {
                //set creation date
                newSprint.CreationDate = DateTime.UtcNow;
                db.Sprints.Add(newSprint);
                
                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_CREATE;
                if (Request != null && Url != null) //avoid null reference exceptions when testing
                {
                    string button = Request.Form["submitButton"];
                    if (button == "1")
                    {
                        return JavaScript("window.location = '" + Url.Action("CreateSprint", new { id = newSprint.ProjectID }) + "'");
                    }
                    return JavaScript("window.location = '" + Url.Action("Details", "Sprint", new { id = newSprint.ID }) + "'");
                }
            }
            //load project for the view
            newSprint.Project = db.GetProjectById(User, newSprint.ProjectID);
            ViewBag.NewSprint = newSprint;
            return PartialView(newSprint.Project);
            //return RedirectToAction("Details", new { id = newTask.ProjectID, op = "create" });
        }

        // POST: /Project/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Code,Name,Description,Status,EndDate,CreationDate")] Project Project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Project).State = EntityState.Modified;
                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
                return JavaScript("window.location = '" + Url.Action("Details", new { id = Project.ID }) + "'");
            }
            Project = db.GetProjectById(User, Project.ID);
            return PartialView("Edit", Project);
        }
        
        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Project project = db.GetProjectById(User, id);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.ContainsSelectedAction)
            {
                //not working nor pending confirmation
                if (project.Owner.WorkingPanelAvailable)
                {
                    //clear selected action
                    project.Owner.ActionID = null;
                    db.Entry(project.Owner).State = EntityState.Modified;
                }
                else
                {
                    //cannot perform change
                    return RedirectToAction("Delete", new { id = id });
                }
            }     

            db.Projects.Remove(project);
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_DELETE;
            return Redirect("/Project");
        }
        
        // GET: /Project/ChangeStatus/5?status=1
        public ActionResult ChangeStatus(int? id, Status? status)
        {
            if (id == null || status.HasValue == false || Request.UrlReferrer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (status.Value != Status.Active)
            {
                //check if contains the selected action
                if (project.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (project.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        project.Owner.ActionID = null;
                        db.Entry(project.Owner).State = EntityState.Modified;
                    }
                    else
                    {
                        //cannot perform change
                        TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                        return Redirect(Request.UrlReferrer.AbsoluteUri);
                    }
                }
            }
            //update end date
            if (Project.IsFinishedStatus(status.Value))
            {
                //if no end date
                if (project.EndDate.HasValue == false)
                {
                    project.EndDate = DateTime.UtcNow;
                }
            }
            else
            {
                //remove end date
                project.EndDate = null;
            }     

            project.Status = status.Value;
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        // GET: /Project/Details/5
        public ActionResult Details(int? id)
        {
            return loadView(id, Operation.ProjectInfo);
        }

        // GET: /Project/Edit/5
        public ActionResult Edit(int? id)
        {
            return loadView(id, Operation.EditProject);
        }

        // GET: /Project/CreateTask/5
        public ActionResult CreateTask(int? id, int? ct, string text)
        {
            ActionResult result = loadView(id, Operation.CreateTask);

            if ((result is HttpStatusCodeResult || result is HttpNotFoundResult) == false)
            {
                Task newTask = (Task)ViewBag.NewTask;

                newTask.ProjectID = id.Value;
                if (ct.HasValue)
                {
                    CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                    if (collectedthing != null)
                    {
                        newTask.SetCode(collectedthing.Name);
                        newTask.SetName(collectedthing.Name);
                        ViewBag.collectedThingID = ct.Value;
                    }
                }
                else if (string.IsNullOrWhiteSpace(text) == false)
                {
                    newTask.SetCode(text);
                    newTask.SetName(text);
                }
            }
            return result;
        }


        // GET: /Project/CreateSprint/5
        public ActionResult CreateSprint(int? id)
        {
            ActionResult result = loadView(id, Operation.CreateTask);

            if ((result is HttpStatusCodeResult || result is HttpNotFoundResult) == false)
            {
                Sprint newSprint = (Sprint)ViewBag.NewSprint;

                newSprint.ProjectID = id.Value;
            }
            return result;
        }

        // GET: /Project/Delete/5
        public ActionResult Delete(int? id)
        {
            return loadView(id, Operation.DeleteProject);
        }

        // GET: /Project/AddTag/5?tagid=1
        public ActionResult AddTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = db.GetProjectById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (project == null || tag == null)
            {
                return HttpNotFound();
            }

            if (project.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() == null)
            {
                project.Tags.Add(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Project/RemoveTag/5?tagid=1
        public ActionResult RemoveTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = db.GetProjectById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (project == null || tag == null)
            {
                return HttpNotFound();
            }

            if (project.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() != null)
            {
                project.Tags.Remove(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Private methods

        private void SetTagsToProject(string[] selectedTagCodes, Project projectToUpdate)
        {
            if (selectedTagCodes == null || selectedTagCodes.Length == 0)
            {
                return;
            }
            var selectedTagCodesHS = new HashSet<string>(selectedTagCodes);
            var projectTagCodes = new HashSet<string>
                (projectToUpdate.OwnAndInheritedTags.Select(c => c.Code));
            foreach (var tag in db.Tags)
            {
                if (selectedTagCodesHS.Contains(tag.Code) &&
                    projectTagCodes.Contains(tag.Code) == false)
                {
                    projectToUpdate.Tags.Add(tag);
                }
            }
        }

        private ActionResult loadView(int? id, Operation operation)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            Task newTask = new Task();
            newTask.ProjectID = id.Value;
            ViewBag.NewTask = newTask;

            Sprint newSprint = new Sprint();
            newSprint.ProjectID = id.Value;
            newSprint.StartDate = DateTime.UtcNow.ToLocalTime().Date;
            ViewBag.NewSprint = newSprint;

            ViewBag.Operation = operation;
            ViewBag.NotSelectedTags = project.GetNotSelectedTags(db.GetMyTags(User)).ToList();
            return View("Project", project);
        }
        
        #endregion

        #region Disabled actions

        // GET: /Project/Edit/5
        [DisabledActionFilter]
        public ActionResult EditOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project Project = db.GetProjectById(User, id.Value);
            if (Project == null)
            {
                return HttpNotFound();
            }
            return View(Project);
        }

        // GET: /Project/Details/5
        [DisabledActionFilter]
        public ActionResult DetailsOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project Project = db.GetProjectById(User, id.Value);
            if (Project == null)
            {
                return HttpNotFound();
            }
            return View(Project);
        }

        // GET: /Project/CreateTask/5
        [DisabledActionFilter]
        public ActionResult CreateTaskOld(int? id, int? ct, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }

            Task newTask = new Task();
            newTask.ProjectID = id.Value;
            if (ct.HasValue)
            {
                CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                if (collectedthing != null)
                {
                    newTask.SetCode(collectedthing.Name);
                    newTask.Name = collectedthing.Name;
                    ViewBag.collectedThingID = ct.Value;
                }
            }
            else if (string.IsNullOrWhiteSpace(text) == false)
            {
                newTask.SetCode(text);
                newTask.Name = text;
            }
            ViewBag.NewTask = newTask;
            return View(project);
        }
        
        // GET: /Project/Delete/5
        [DisabledActionFilter]
        public ActionResult DeleteOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.ContainsSelectedAction)
            {
                //not working nor pending confirmation
                if (project.Owner.WorkingPanelAvailable == false)
                {
                    //cannot perform change
                    TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                    return RedirectToAction("Details", new { id = id.Value });
                }
            }
            return View(project);
        }

        // GET: /Project/Charts/5
        [DisabledActionFilter]
        public ActionResult Charts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View("Containers/_WorkCharts", project);
        }

        // GET: /Project/TaskList/5
        [DisabledActionFilter]
        public ActionResult TaskList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_TaskContainer", project);
        }

        // GET: /Project/WorkHistory/5
        [DisabledActionFilter]
        public ActionResult WorkHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.GetProjectById(User, id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_PomodoroSetsList", project);
        }

        #endregion
    }
}
