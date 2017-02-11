using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTDoro.Models;
using Action = GTDoro.Models.Action;
using GTDoro.ViewModels;
using GTDoro.DAL;
using GTDoro.ExtensionMethods;
using PagedList;
using System.Data.Entity.Infrastructure;
using GTDoro.Models.ActionFilters;
using GTDoro.Models.Identity;
using System.Web.Routing;

namespace GTDoro.Controllers
{
    [Authorize]
    public class TaskController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Task/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.ProjectSortParm = (sortOrder == "proj") ? "proj_desc" : "proj";
            ViewBag.CodeSortParm = (sortOrder == "code") ? "code_desc" : "code";
            ViewBag.NameSortParm = (sortOrder == "name") ? "name_desc" : "name";
            ViewBag.EndDateSortParm = (sortOrder == "date") ? "date_desc" : "date";
            ViewBag.EstimateSortParm = (sortOrder == "esti") ? "esti_desc" : "esti";
            ViewBag.StatusSortParm = (sortOrder == "stat") ? "stat_desc" : "stat";
            ViewBag.PrioritySortParm = (sortOrder == "prio") ? "prio_desc" : "prio";
            ViewBag.LastPomodoroSortParm = (sortOrder == "last") ? "last_desc" : "last";
            ViewBag.PomodorosSortParm = (sortOrder == "pomo") ? "pomo_desc" : "pomo";
            ViewBag.ProgressSortParm = (sortOrder == "prog") ? "prog_desc" : "prog";
            ViewBag.EffortSortParm = (sortOrder == "effo") ? "effo_desc" : "effo";
            ViewBag.ActionsSortParm = (sortOrder == "acti") ? "acti_desc" : "acti";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //var tasks = db.Tasks.Include(t => t.Project);
            var tasks = db.GetMyTasks(User);

            //stores the current filter info for the pagination control
            RouteValueDictionary dict = new RouteValueDictionary();
            //initilised with the current sort order
            dict["CurrentSort"] = sortOrder;

            //status filter
            const string STATUS_PREFIX = "status-";
            string[] statusFilter = GetArrayParamsFromRequest(Request.QueryString, STATUS_PREFIX);
            if (statusFilter.Length > 0)
            {
                tasks = tasks.ToList().AsQueryable()
                    .Where(t => statusFilter
                        .Any(sf => sf.ToLower() == t.CalculatedStatus.ToString().ToLower())
                );
                dict.AddFilterArrayToRouteValueDictionary(statusFilter, STATUS_PREFIX);
            }

            //tag filter
            const string TAG_PREFIX = "tag-";
            string[] tagFilter = GetArrayParamsFromRequest(Request.QueryString, TAG_PREFIX);
            if (tagFilter.Length > 0)
            {
                //OR Tag Filter
                tasks = tasks.ToList().AsQueryable()
                    .Where(t => tagFilter
                        .Any(tf => t.OwnAndInheritedTags    //all tags in the filter
                            .Select(tc => tc.Code)          //(only tag code list)
                                .Contains(tf))              //contained in the item tags 
                );
                //AND Tag Filter
                //tasks = tasks.ToList().AsQueryable()
                //    .Where(t => tagFilter
                //        .All(tf => t.OwnAndInheritedTags    //all tags in the filter
                //            .Select(tc => tc.Code)          //(only tag code list)
                //                .Contains(tf))              //contained in the item tags 
                //);
                dict.AddFilterArrayToRouteValueDictionary(tagFilter, TAG_PREFIX);
            }

            //priority filter
            const string PRIORITY_PREFIX = "priority-";
            string[] priorityFilter = GetArrayParamsFromRequest(Request.QueryString, PRIORITY_PREFIX);
            if (priorityFilter.Length > 0)
            {
                tasks = tasks.ToList().AsQueryable()
                    .Where(t => priorityFilter
                        .Any(pf => pf.ToLower() == t.Priority.ToString().ToLower())
                );
                dict.AddFilterArrayToRouteValueDictionary(priorityFilter, PRIORITY_PREFIX);
            }

            ViewBag.IsFiltered = string.IsNullOrEmpty(Request.QueryString["filtering"]) == false ||
                (statusFilter.Length > 0) || (priorityFilter.Length > 0) || (tagFilter.Length > 0);
            ViewBag.RouteFiltersForPagination = dict;

            ViewBag.StatusFilter = statusFilter;
            ViewBag.PriorityFilter = priorityFilter;
            ViewBag.TagFilter = tagFilter;
            //ViewBag.CurrentFilter = searchString;

            ViewBag.AllTags = db.GetMyTags(User).ToArray();

            orderBySortParam(ref tasks, sortOrder);

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(tasks.ToPagedList(pageNumber, pageSize));
        }

        // POST: /Task/
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit(int[] selectedTasks, int? newStatus, int? newPriority)
        {
            if (selectedTasks != null && selectedTasks.Length > 0)
            {
                string[] selectedTags = GetArrayParamsFromRequest(Request.Form, "tag-");

                if (newStatus.HasValue || newPriority.HasValue || selectedTags.Length > 0)
                {
                    var tasksToUpdate = db.GetMyTasks(User)
                       .Include(t => t.Tags)
                       .Where(t => selectedTasks.ToList().Contains(t.ID)).ToList();

                    try
                    {
                        foreach (var task in tasksToUpdate)
                        {
                            if (newStatus.HasValue)
                            {
                                task.Status = (Status)newStatus.Value;
                            }
                            if (newPriority.HasValue)
                            {
                                task.Priority = (LevelExtended)newPriority.Value;
                            }
                            if (selectedTags != null && selectedTags.Length > 0)
                            {
                                SetTagsToTask(selectedTags, task);
                            }
                            db.Entry(task).State = EntityState.Modified;

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

        // POST: /Task/CreateAction/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAction([Bind(Include = "ID,Name,Description,EndDate,Estimate,TaskID,Deadline,Status,IsPersistent,Priority")] Action newAction) //var name cannot be action
        {
            if (ModelState.IsValid)
            {
                db.Actions.Add(newAction);
                //set creation date
                newAction.CreationDate = DateTime.UtcNow;

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
                        return JavaScript("window.location = '" + Url.Action("CreateAction", new { id = newAction.TaskID }) + "'");
                    }
                    return JavaScript("window.location = '" + Url.Action("Details", "Action", new { id = newAction.ID }) + "'");
                }
            }
            //load task for the view
            newAction.Task = db.GetTaskById(User, newAction.TaskID);
            ViewBag.NewAction = newAction;
            return PartialView(newAction.Task);
        }

        // POST: /Task/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var taskToUpdate = db.GetMyTasks(User)
               .Include(t => t.Tags)
               .Where(t => t.ID == id)
               .Single();

            if (TryUpdateModel(taskToUpdate, "",
               new string[] { "ID", "Code", "Name", "Description", "EndDate", "ProjectID", "Status", "Priority", "CreationDate" }))
            {
                try
                {
                    db.Entry(taskToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
                    return JavaScript("window.location = '" + Url.Action("Details", new { id = id }) + "'");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return PartialView(taskToUpdate);
        }

        // POST: /Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Task task = db.GetTaskById(User, id);
            if (task == null)
            {
                return HttpNotFound();
            }
            if (task.ContainsSelectedAction)
            {
                //not working nor pending confirmation
                if (task.Owner.WorkingPanelAvailable)
                {
                    //clear selected action
                    task.Owner.ActionID = null;
                    db.Entry(task.Owner).State = EntityState.Modified;
                }
                else
                {
                    //cannot perform change
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            int projectID = task.ProjectID;
            db.Tasks.Remove(task);
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_DELETE;
            //return Redirect(UrlReferrer);
            return RedirectToAction("Details", "Project", new { id = projectID });
        }

        // GET: /Task/ChangeStatus/5?status=1
        public ActionResult ChangeStatus(int? id, Status? status)
        {
            if (id == null || status.HasValue == false || Request.UrlReferrer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            if (status.Value != Status.Active)
            {
                //check if contains the selected action
                if (task.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (task.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        task.Owner.ActionID = null;
                        db.Entry(task.Owner).State = EntityState.Modified;
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
            if (Task.IsFinishedStatus(status.Value))
            {
                //if no end date
                if (task.EndDate.HasValue == false)
                {
                    task.EndDate = DateTime.UtcNow;
                }
            }
            else
            {
                //remove end date
                task.EndDate = null;
            }

            task.Status = status.Value;
            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        // GET: /Task/ChangeProject/5?parent=1
        public ActionResult ChangeProject(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            Project project = db.GetProjectById(User, parent.Value);
            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.Status != Status.Active)
            {
                //check if contains the selected action
                if (task.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (task.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        task.Owner.ActionID = null;
                        db.Entry(task.Owner).State = EntityState.Modified;
                    }
                    else
                    {
                        //cannot perform change
                        TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                        return Redirect(Request.GetReferrerUrlOrCurrent());
                    }
                }
            }
            task.ProjectID = parent.Value;
            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return RedirectToAction("Details", new { id = id.Value });
        }

        // GET: /Task/Edit/5
        public ActionResult Edit(int? id)
        {
            return loadView(id, Operation.EditTask);
        }

        // GET: /Task/Delete/5
        public ActionResult Delete(int? id)
        {
            return loadView(id, Operation.DeleteTask);
        }


        public ActionResult Details(int? id)
        {
            return loadView(id, Operation.TaskInfo);
        }


        // GET: /Task/CreateAction/5
        public ActionResult CreateAction(string op, int? id, int? ct, string text)
        {
            ActionResult result = loadView(id, Operation.CreateAction);
            if ((result is HttpStatusCodeResult || result is HttpNotFoundResult) == false)
            {
                Action newAction = (Action)ViewBag.NewAction;

                newAction.TaskID = id.Value;
                if (ct.HasValue)
                {
                    CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                    if (collectedthing != null)
                    {
                        newAction.SetName(collectedthing.Name);
                        ViewBag.collectedThingID = ct.Value;
                    }
                }
                else if (string.IsNullOrWhiteSpace(text) == false)
                {
                    newAction.SetName(text);
                }
            }
            return result;
        }


        // GET: /Task/AddTag/5?tagid=1
        public ActionResult AddTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = db.GetTaskById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (task == null || tag == null)
            {
                return HttpNotFound();
            }

            if (task.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() == null)
            {
                task.Tags.Add(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Task/RemoveTag/5?tagid=1
        public ActionResult RemoveTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = db.GetTaskById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (task == null || tag == null)
            {
                return HttpNotFound();
            }

            if (task.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() != null)
            {
                task.Tags.Remove(tag);
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

        private void orderBySortParam(ref IQueryable<Task> tasks, string sortOrder)
        {
            switch (sortOrder)
            {
                case "proj":
                    tasks = tasks.OrderBy(t => t.Project.Code).ThenBy(t => t.Code);
                    break;
                case "proj_desc":
                    tasks = tasks.OrderByDescending(t => t.Project.Code).ThenByDescending(t => t.Code);
                    break;
                case "code":
                    tasks = tasks.OrderBy(t => t.Code);
                    break;
                case "code_desc":
                    tasks = tasks.OrderByDescending(t => t.Code);
                    break;
                case "name":
                    tasks = tasks.OrderBy(t => t.Name);
                    break;
                case "name_desc":
                    tasks = tasks.OrderByDescending(t => t.Name);
                    break;
                case "date":
                    tasks = tasks.OrderBy(t => t.EndDate);
                    break;
                case "dest_desc":
                    tasks = tasks.OrderByDescending(t => t.EndDate);
                    break;
                case "stat":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.CalculatedStatus);
                    break;
                case "stat_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.CalculatedStatus);
                    break;
                case "prio":
                    tasks = tasks.OrderBy(t => t.Priority);
                    break;
                case "prio_desc":
                    tasks = tasks.OrderByDescending(t => t.Priority);
                    break;
                case "last":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "last_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "pomo":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.CompletedPomodorosCount);
                    break;
                case "pomo_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.CompletedPomodorosCount);
                    break;
                case "prog":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.Progress.ToDecimalOrZero());
                    break;
                case "prog_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.Progress.ToDecimalOrZero());
                    break;
                case "effo":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.Effort.ToDecimalOrZero()).ThenBy(t => t.CompletedPomodorosCount);
                    break;
                case "effo_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.Effort.ToDecimalOrZero()).ThenByDescending(t => t.CompletedPomodorosCount);
                    break;
                case "acti":
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.Actions.Count());
                    break;
                case "acti_desc":
                    tasks = tasks.ToList().AsQueryable().OrderByDescending(t => t.Actions.Count());
                    break;
                default:
                    tasks = tasks.ToList().AsQueryable().OrderBy(t => t.Status).ThenByDescending(t => t.LastPomodoro.ToDateTicksOrZero());
                    break;
            }
        }

        private void SetTagsToTask(string[] selectedTagCodes, Task taskToUpdate)
        {
            if (selectedTagCodes == null || selectedTagCodes.Length == 0)
            {
                return;
            }
            var selectedTagCodesHS = new HashSet<string>(selectedTagCodes);
            var taskTagCodes = new HashSet<string>
                (taskToUpdate.OwnAndInheritedTags.Select(c => c.Code));
            foreach (var tag in db.Tags)
            {
                if (selectedTagCodesHS.Contains(tag.Code) &&
                    taskTagCodes.Contains(tag.Code) == false)
                {
                    taskToUpdate.Tags.Add(tag);
                }
            }
        }

        private ActionResult loadView(int? id, Operation operation)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var task = db.GetTaskById(User, id.Value);

            if (task == null)
            {
                return HttpNotFound();
            }
            Action newAction = new Action();
            newAction.TaskID = id.Value;
            ViewBag.NewAction = newAction;

            ViewBag.Operation = operation;
            ViewBag.NotSelectedTags = task.GetNotSelectedTags(db.GetMyTags(User)).ToList();
            return View("Task", task);
        }

        #region Tag set private methods (not in use)

        //private void PopulateAssignedTagData(Task task = null)
        //{
        //    var allTags = db.GetMyTags(User);
        //    HashSet<int> taskTags = null;
        //    if (task != null)
        //    {
        //        taskTags = new HashSet<int>(task.Tags.Select(c => c.ID));
        //    }
        //    var viewModel = new List<AssignedTagData>();
        //    foreach (var tag in allTags)
        //    {
        //        viewModel.Add(new AssignedTagData
        //        {
        //            TagID = tag.ID,
        //            Title = tag.Name,
        //            Assigned = (taskTags != null) ? taskTags.Contains(tag.ID) : false
        //        });
        //    }
        //    ViewBag.Tags = viewModel;
        //}

        //private void UpdateTaskTags(string[] selectedTags, Task taskToUpdate)
        //{
        //    if (selectedTags == null)
        //    {
        //        taskToUpdate.Tags = new List<Tag>();
        //        return;
        //    }

        //    var selectedTagsHS = new HashSet<string>(selectedTags);
        //    var taskTags = new HashSet<int>
        //        (taskToUpdate.Tags.Select(c => c.ID));
        //    foreach (var tag in db.Tags)
        //    {
        //        if (selectedTagsHS.Contains(tag.ID.ToString()))
        //        {
        //            if (!taskTags.Contains(tag.ID))
        //            {
        //                taskToUpdate.Tags.Add(tag);
        //            }
        //        }
        //        else
        //        {
        //            if (taskTags.Contains(tag.ID))
        //            {
        //                taskToUpdate.Tags.Remove(tag);
        //            }
        //        }
        //    }
        //}

        #endregion

        #endregion

        #region Disabled actions

        // GET: /Task/Details/5
        [DisabledActionFilter]
        public ActionResult DetailsOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: /Task/CreateAction/5
        [DisabledActionFilter]
        public ActionResult CreateActionOld(int? id, int? ct, string text)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }

            Action newAction = new Action();
            newAction.TaskID = id.Value;
            if (ct.HasValue)
            {
                CollectedThing collectedthing = db.GetCollectedThingById(User, ct.Value);
                if (collectedthing != null)
                {
                    newAction.SetName(collectedthing.Name);
                    ViewBag.collectedThingID = ct.Value;
                }
            }
            else if (string.IsNullOrWhiteSpace(text) == false)
            {
                newAction.SetName(text);
            }
            ViewBag.NewAction = newAction;
            return View(task);
        }

        // GET: /Task/Edit/5
        [DisabledActionFilter]
        public ActionResult EditOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Task task = db.GetMyTasks(User)
            //    .Include(t => t.Tags)
            //    .Where(t => t.ID == id)
            //    .Single();
            var task = db.GetTaskById(User, id.Value);

            //PopulateAssignedTagData(task);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: /Task/Delete/5
        [DisabledActionFilter]
        public ActionResult DeleteOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            if (task.ContainsSelectedAction)
            {
                //not working nor pending confirmation
                if (task.Owner.WorkingPanelAvailable == false)
                {
                    //cannot perform change
                    TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                    return RedirectToAction("Details", new { id = id.Value });
                }
            }
            return View(task);
        }

        // GET: /Task/Create
        [DisabledActionFilter]
        public ActionResult Create(int? projectId, int? collectedThingID, string name)
        {
            ViewBag.ProjectID = new SelectList(db.GetMyProjects(User), "ID", "Code", projectId);
            ViewBag.Name = name;
            return View();
        }

        // POST: /Task/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description,EndDate,ProjectID,Status,Priority")] Task task)
        {
            int collectedThingID = 0;
            Int32.TryParse(Request.Params["collectedThingID"], out collectedThingID);

            if (ModelState.IsValid)
            {
                //set creation date
                task.CreationDate = DateTime.UtcNow;
                //create task
                db.Tasks.Add(task);
                if (collectedThingID > 0)
                {
                    //delete associated collected thing
                    CollectedThing collectedThing = db.GetCollectedThingById(User, collectedThingID);
                    db.CollectedThings.Remove(collectedThing);
                }

                db.SaveChanges();

                return RedirectToAction("TaskList", "Project", new { id = task.ProjectID });
            }

            ViewBag.ProjectID = new SelectList(db.GetMyProjects(User), "ID", "Code", task.ProjectID);
            return View(task);
        }

        // GET: /Task/Charts/5
        [DisabledActionFilter]
        public ActionResult Charts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }

            return View("Containers/_WorkCharts", task);
        }

        // GET: /Task/ActionList/5
        [DisabledActionFilter]
        public ActionResult ActionList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_ActionContainer", task);
        }

        // GET: /Task/WorkHistory/5
        [DisabledActionFilter]
        public ActionResult WorkHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.GetTaskById(User, id.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_PomodoroSetsList", task);
        }

        #endregion
    }
}
