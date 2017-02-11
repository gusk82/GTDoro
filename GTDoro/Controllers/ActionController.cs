using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTDoro.Models;
using GTDoro.DAL;
using PagedList;
using System.Data.Entity.Infrastructure;
using GTDoro.Exception;
using GTDoro.ExtensionMethods;
using System.Data.Entity.Core.Objects;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Models.Identity;
using Action = GTDoro.Models.Action;
using GTDoro.Models.ActionFilters;
using System.Web.Routing;

namespace GTDoro.Controllers
{
    [Authorize]
    public class ActionController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Action/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.ProjectSortParm = (sortOrder == "proj") ? "proj_desc" : "proj";
            ViewBag.TaskSortParm = (sortOrder == "task") ? "task_desc" : "task";
            ViewBag.ActionSortParm = (sortOrder == "acti") ? "acti_desc" : "acti";
            ViewBag.DateSortParm = (sortOrder == "date") ? "date_desc" : "date";
            ViewBag.EstimateSortParm = (sortOrder == "esti") ? "esti_desc" : "esti";
            ViewBag.IsPersistentSortParm = (sortOrder == "pers") ? "pers_desc" : "pers";
            ViewBag.StatusSortParm = (sortOrder == "stat") ? "stat_desc" : "stat";
            ViewBag.PrioritySortParm = (sortOrder == "prio") ? "prio_desc" : "prio";
            ViewBag.LastPomodoroSortParm = (sortOrder == "last") ? "last_desc" : "last";
            ViewBag.PomodorosSortParm = (sortOrder == "pomo") ? "pomo_desc" : "pomo";
            ViewBag.EffortSortParm = (sortOrder == "effo") ? "effo_desc" : "effo";

            var actions = db.GetMyActions(User);

            //stores the current filter info for the pagination control
            RouteValueDictionary dict = new RouteValueDictionary();
            //initilised with the current sort order
            dict["CurrentSort"] = sortOrder;

            //status filter
            const string STATUS_PREFIX = "status-";
            string[] statusFilter = GetArrayParamsFromRequest(Request.QueryString, STATUS_PREFIX);
            if (statusFilter.Length > 0)
            {
                actions = actions.ToList().AsQueryable()
                    .Where(a => statusFilter
                        .Any(sf => sf.ToLower() == a.CalculatedStatus.ToString().ToLower())
                );
                dict.AddFilterArrayToRouteValueDictionary(statusFilter, STATUS_PREFIX);
            }

            //tag filter
            const string TAG_PREFIX = "tag-";
            string[] tagFilter = GetArrayParamsFromRequest(Request.QueryString, TAG_PREFIX);
            if (tagFilter.Length > 0)
            {
                actions = actions.ToList().AsQueryable()
                    .Where(a => tagFilter
                        .All(tf => a.OwnAndInheritedTags    //all tags in the filter
                            .Select(tc => tc.Code)          //(only tag code list)
                                .Contains(tf))              //contained in the item tags 
                );
                //AND Tag Filter
                //actions = actions.ToList().AsQueryable()
                //    .Where(a => tagFilter
                //        .All(tf => a.OwnAndInheritedTags    //all tags in the filter
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
                actions = actions.ToList().AsQueryable()
                    .Where(a => priorityFilter
                        .Any(pf => pf.ToLower() == a.Priority.ToString().ToLower())
                );
                dict.AddFilterArrayToRouteValueDictionary(priorityFilter, PRIORITY_PREFIX);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.IsFiltered = string.IsNullOrEmpty(Request.QueryString["filtering"]) == false ||
                (statusFilter.Length > 0) || (priorityFilter.Length > 0) || (tagFilter.Length > 0);
            ViewBag.RouteFiltersForPagination = dict;

            ViewBag.StatusFilter = statusFilter;
            ViewBag.PriorityFilter = priorityFilter;
            ViewBag.TagFilter = tagFilter;
            //ViewBag.CurrentFilter = searchString;

            ViewBag.AllTags = db.GetMyTags(User).ToArray();

            switch (sortOrder)
            {
                case "proj":
                    actions = actions.OrderBy(a => a.Task.Project.Code).ThenBy(a => a.Task.Code).ThenBy(a => a.Name);
                    break;
                case "proj_desc":
                    actions = actions.OrderByDescending(a => a.Task.Project.Code).ThenByDescending(a => a.Task.Code).ThenByDescending(a => a.Name);
                    break;
                case "task":
                    actions = actions.OrderBy(a => a.Task.Code).ThenBy(a => a.Name);
                    break;
                case "task_desc":
                    actions = actions.OrderByDescending(a => a.Task.Code).ThenByDescending(a => a.Name);
                    break;
                case "acti":
                    actions = actions.OrderBy(a => a.Name);
                    break;
                case "acti_desc":
                    actions = actions.OrderByDescending(a => a.Name);
                    break;
                case "esti":
                    actions = actions.OrderBy(a => a.Estimate);
                    break;
                case "esti_desc":
                    actions = actions.OrderByDescending(a => a.Estimate);
                    break;
                case "pers":
                    actions = actions.ToList().AsQueryable().OrderBy(a => a.IsPersistent);
                    break;
                case "pers_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(a => a.IsPersistent);
                    break;
                case "last":
                    actions = actions.ToList().AsQueryable().OrderBy(t => t.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "last_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(t => t.LastPomodoro.ToDateTicksOrZero());
                    break;
                case "pomo":
                    actions = actions.ToList().AsQueryable().OrderBy(a => a.CompletedPomodorosCount);
                    break;
                case "pomo_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(a => a.CompletedPomodorosCount);
                    break;
                case "date":
                    actions = actions.ToList().AsQueryable().OrderBy(a => a.DeadlineOrEndDate.ToTicksOrZero());
                    break;
                case "date_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(a => a.DeadlineOrEndDate.ToTicksOrZero());
                    break;               
                case "stat":
                    actions = actions.ToList().AsQueryable().OrderBy(a => a.CalculatedStatus);
                    break;
                case "stat_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(a => a.CalculatedStatus);
                    break;
                case "prio":
                    actions = actions.OrderBy(a => a.Priority);
                    break;
                case "prio_desc":
                    actions = actions.OrderByDescending(a => a.Priority);
                    break;
                case "effo":
                    actions = actions.ToList().AsQueryable().OrderBy(t => t.Effort.ToDecimalOrZero()).ThenBy(t => t.CompletedPomodorosCount);
                    break;
                case "effo_desc":
                    actions = actions.ToList().AsQueryable().OrderByDescending(t => t.Effort.ToDecimalOrZero()).ThenByDescending(t => t.CompletedPomodorosCount);
                    break;
                default:
                    actions = actions.ToList().AsQueryable().OrderBy(a => a.Status).ThenByDescending(a => a.LastPomodoro.ToDateTicksOrZero());
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(actions.ToPagedList(pageNumber, pageSize));
        }

        // POST: /Action/
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit(int[] selectedActions, int? newStatus, int? newPriority)
        {
            if (selectedActions != null && selectedActions.Length > 0)
            {
                string[] selectedTags = GetArrayParamsFromRequest(Request.Form, "tag-");

                if (newStatus.HasValue || newPriority.HasValue || selectedTags.Length > 0)
                {
                    var actionsToUpdate = db.GetMyActions(User)
                       .Include(t => t.Tags)
                       .Where(a => selectedActions.ToList().Contains(a.ID)).ToList();

                    try
                    {
                        foreach (var action in actionsToUpdate)
                        {
                            if (newStatus.HasValue)
                            {
                                action.Status = (Status)newStatus.Value;
                            }
                            if (newPriority.HasValue)
                            {
                                action.Priority = (LevelExtended)newPriority.Value;
                            }
                            if (selectedTags != null && selectedTags.Length > 0)
                            {
                                SetTagsToAction(selectedTags, action);
                            }
                            db.Entry(action).State = EntityState.Modified;
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


        // GET: /Action/Details/5
        public ActionResult LightDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            return View(action);
        }

        // POST: /Action/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,EndDate,Estimate,TaskID,Deadline,Status,IsPersistent,Priority,CreationDate")] Action newAction) //var name cannot be action
        {
            if (ModelState.IsValid)
            {
                db.Entry(newAction).State = EntityState.Modified;
                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
                return JavaScript("window.location = '" + Url.Action("Details", new { id = newAction.ID }) + "'");
            }
            newAction = db.GetActionById(User, newAction.ID);
            return PartialView(newAction);
        }

        // POST: /Action/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Action action = db.GetActionById(User, id);
            if (action == null)
            {
                return HttpNotFound();
            }
            //check if it's the selected action
            if (action.ContainsSelectedAction)
            {
                //neither working nor pending confirmation
                if (action.Owner.WorkingPanelAvailable)
                {
                    //clear selected action
                    action.Owner.ActionID = null;
                    db.Entry(action.Owner).State = EntityState.Modified;
                }
                else
                {
                    //cannot perform change
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            int taskID = action.TaskID;
            db.Actions.Remove(action);
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_DELETE;
            return RedirectToAction("Details", "Task", new { id = taskID });
        }
        
        // GET: /Action/Select/5
        public ActionResult Select(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser.WorkingPanelAvailable && action.IsSelectable)
            {
                currentUser.ActionID = id.Value;
                db.Entry(currentUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
            }

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // POST: /Action/Select/5
        [HttpPost, ActionName("Select")]
        public ActionResult SelectPost(int? ActionID)
        {
            return RedirectToAction("Select", new { id = ActionID });
        }

        #region Private Properties Time Select List

        private List<object> timesSelectList
        {
            get
            {
                //populate time drop down lists
                List<object> times = new List<object>();
                int counter = 0;
                TimeSpan TimeFrom = new TimeSpan(0);

                while (TimeFrom != new TimeSpan(24, 0, 0))
                {
                    times.Add(new { period = counter++, time = TimeFrom.ToString(@"hh\:mm") });
                    TimeFrom = TimeFrom.Add(new TimeSpan(0, Settings.POMOCYCLE, 0));
                }
                return times;
            }
        }


        private SelectList startTimeSelectList
        {
            get
            {
                return new SelectList(timesSelectList, "period", "time",
                    Settings.DefaultPlanifyTime.ToPomoLengthPeriod());
            }
        }

        private SelectList endTimeSelectList
        {
            get
            {
                return new SelectList(timesSelectList, "period", "time", 
                    Settings.DefaultPlanifyTime.ToPomoLengthPeriod() + Settings.DEFAULT_WORK_LENGTH);
            }
        }

        #endregion


        private void checkPlanification(Action action, DateTime dateFrom, int startTimePeriod, int endTimePeriod)
        {
            DateTime start = dateFrom.AddMinutes(startTimePeriod * Settings.POMOCYCLE);
            DateTime end = dateFrom.AddMinutes(endTimePeriod * Settings.POMOCYCLE);

            if (start.AddMinutes(Settings.POMOCYCLE) > end)
            {
                throw new CalendarException(CalendarWarning.NonPositiveInterval, "Planification must have a minimum of one pomodoro");
            }
            if (start < DateTime.UtcNow.ToUserLocalTime(action.Owner.TimeZoneId))
            {
                throw new CalendarException(CalendarWarning.PastTime, "Planification datetime must be in the future");
            }
            if (action == null || action.Status != Status.Active)
            {
                throw new CalendarException(CalendarWarning.InvalidAction, "Invalid or inactive action (ID: " + action.ID + ")");
            }
            List<Pomodoro> conflictingPomodoros = db.GetMyPomodoros(User).Where(p => p.Start.HasValue).AsEnumerable().Where(p =>
                ((p.StartLocal.Value <= start && DbFunctions.AddMinutes(p.StartLocal.Value, Settings.POMOCYCLE) > start) ||
                    (p.StartLocal.Value < end && DbFunctions.AddMinutes(p.StartLocal.Value, Settings.POMOCYCLE) >= end))).ToList();
            if (conflictingPomodoros.Count > 0)
            {
                throw new ConflictCalendarException(CalendarWarning.Conflict, conflictingPomodoros);
            }
        }


        // GET: /Action/ChangeStatus/5?status=1
        public ActionResult ChangeStatus(int? id, Status? status)
        {
            if (id == null || status.HasValue == false || Request.UrlReferrer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            if (status.Value != Status.Active)
            {
                //check if it's the selected action
                if (action.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (action.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        action.Owner.ActionID = null;
                        db.Entry(action.Owner).State = EntityState.Modified;
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
            if(Action.IsFinishedStatus(status.Value))
            {
                //if no end date
                if(action.EndDate.HasValue == false)
                {
                    action.EndDate = DateTime.UtcNow;
                }
            }
            else
            {
                //remove end date
                action.EndDate = null;
            }     

            action.Status = status.Value;
            db.Entry(action).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        // GET: /Action/ChangeTask/5?parent=1
        public ActionResult ChangeTask(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            };
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            Task task = db.GetTaskById(User, parent.Value);
            if (task == null)
            {
                return HttpNotFound();
            }
            if (task.Status != Status.Active)
            {
                //check if contains the selected action
                if (action.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (action.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        action.Owner.ActionID = null;
                        db.Entry(action.Owner).State = EntityState.Modified;
                    }
                    else
                    {
                        //cannot perform change
                        TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                        return Redirect(Request.GetReferrerUrlOrCurrent());
                    }
                }
            }
            action.TaskID = parent.Value;
            db.Entry(action).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return RedirectToAction("Details", new { id = id.Value });
        }

        // GET: /Action/Details/5
        public ActionResult Details(int? id)
        {
            return loadView(id, Operation.ActionInfo);
        }

        // GET: /Action/Edit/5
        public ActionResult Edit(int? id)
        {
            return loadView(id, Operation.EditAction);
        }

        // GET: /Action/Delete/5
        public ActionResult Delete(int? id)
        {
            return loadView(id, Operation.DeleteAction);
        }


        // GET: /Action/AddTag/5?tagid=1
        public ActionResult AddTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var action = db.GetActionById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (action == null || tag == null)
            {
                return HttpNotFound();
            }

            if (action.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() == null)
            {
                action.Tags.Add(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Action/RemoveTag/5?tagid=1
        public ActionResult RemoveTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var action = db.GetActionById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (action == null || tag == null)
            {
                return HttpNotFound();
            }

            if (action.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() != null)
            {
                action.Tags.Remove(tag);
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

        private void SetTagsToAction(string[] selectedTagCodes, Action actionToUpdate)
        {
            if (selectedTagCodes == null || selectedTagCodes.Length == 0)
            {
                return;
            }
            var selectedTagCodesHS = new HashSet<string>(selectedTagCodes);
            var actionTagCodes = new HashSet<string>
                (actionToUpdate.OwnAndInheritedTags.Select(c => c.Code));
            foreach (var tag in db.Tags)
            {
                if (selectedTagCodesHS.Contains(tag.Code) &&
                    actionTagCodes.Contains(tag.Code) == false)
                {
                    actionToUpdate.Tags.Add(tag);
                }
            }
        }

        private ActionResult loadView(int? id, Operation operation)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            ViewBag.Operation = operation;
            ViewBag.NotSelectedTags = action.GetNotSelectedTags(db.GetMyTags(User)).ToList();
            return View("Action", action);
        }

        #endregion

        #region Disabled actions

        // GET: /Action/Details/5
        [DisabledActionFilter]
        public ActionResult DetailsOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            return View(action);
        }

        // GET: /Action/Edit/5
        [DisabledActionFilter]
        public ActionResult EditOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var actionToUpdate = db.GetActionById(User, id.Value);
            if (actionToUpdate == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", actionToUpdate.TaskID);
            return View(actionToUpdate);
        }

        // GET: /Action/Delete/5
        [DisabledActionFilter]
        public ActionResult DeleteOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }

            if (action.ContainsSelectedAction)
            {
                //working or pending confirmation
                if (action.Owner.WorkingPanelAvailable == false)
                {
                    TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                    return RedirectToAction("Details", new { id = id.Value });
                }
            }
            return View(action);
        }

        // GET: /Action/Create
        [DisabledActionFilter]
        public ActionResult Create(int? taskId, int? collectedThingID, string name)
        {
            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", taskId);
            ViewBag.Name = name;
            return View();
        }

        // POST: /Action/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include = "ID,Name,Description,EndDate,Estimate,TaskID,Deadline,Status")] Action newAction) //var name cannot be action
        {
            int collectedThingID = 0;
            if (Request != null)
            {
                Int32.TryParse(Request.Params["collectedThingID"], out collectedThingID);
            }

            newAction.Task = db.GetTaskById(User, newAction.TaskID);
            if (ModelState.IsValid)
            {
                //create action
                db.Actions.Add(newAction);
                //set creation date
                newAction.CreationDate = DateTime.UtcNow;
                if (collectedThingID > 0)
                {
                    //delete associated collected thing
                    CollectedThing collectedThing = db.GetCollectedThingById(User, collectedThingID);
                    db.CollectedThings.Remove(collectedThing);
                }

                db.SaveChanges();
                return RedirectToAction("ActionList", "Task", new { id = newAction.TaskID });
            }

            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", newAction.TaskID);
            return View(newAction);
        }

        // GET: /Action/Charts/5
        [DisabledActionFilter]
        public ActionResult Charts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }

            return View("Containers/_WorkCharts", action);
        }

        // GET: /Action/WorkHistory/5
        [DisabledActionFilter]
        public ActionResult WorkHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.GetActionById(User, id.Value);
            if (action == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_PomodoroSetsList", action);
        }

        // GET: /Action/Work/5
        [DisabledActionFilter]
        public ActionResult Work(int? id)
        {
            //(first) pomodoro with status = working(if exists)
            Pomodoro pomodoro = db.GetMyPomodoros(User).FirstOrDefault(p => p.Status == PomodoroStatus.Working);
            if (pomodoro != null)
            {
                if (id.HasValue && id.Value == pomodoro.ActionID)
                {
                    return RedirectToAction("Working", "Pomodoro");
                }
                else
                {
                    return RedirectToAction("CancelWorking", "Pomodoro", new { id = id });
                }
            }

            Action action = null;
            //ToDo: Check conflicts
            if (id.HasValue)
            {
                action = db.Actions.Find(id);
                if (action != null)
                {
                    UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var currentUser = manager.FindById(User.Identity.GetUserId());
                    currentUser.ActionID = id.Value;
                    db.Entry(currentUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View(action);
        }

        // POST: /Action/Work/5
        [HttpPost]
        [DisabledActionFilter]
        public ActionResult Work()
        {
            Pomodoro pomodoro = new Pomodoro();
            pomodoro.Start = DateTime.UtcNow;
            pomodoro.Status = PomodoroStatus.Working;
            pomodoro.ActionID = Int32.Parse(Request.Form["ActionID"]);
            db.Entry(pomodoro).State = EntityState.Modified;
            db.Pomodoros.Add(pomodoro);
            db.SaveChanges();
            return RedirectToAction("Working", "Pomodoro");
        }

        // GET: /Action/Planify/5
        [DisabledActionFilter]
        public ActionResult Planify(int? id)
        {
            Action action = null;
            if (id.HasValue)
            {
                action = db.Actions.Find(id.Value);
            }

            ViewBag.dateFrom = Settings.DefaultPlanifyDateTime.ToString("dd/MM/yyyy");
            ViewBag.startTimePeriod = startTimeSelectList;
            ViewBag.endTimePeriod = endTimeSelectList;

            return View(action);
        }

        // POST: /Action/Planify/5
        [HttpPost]
        [DisabledActionFilter]
        public ActionResult Planify(int ActionID, DateTime dateFrom, int startTimePeriod, int endTimePeriod, string submitButton)
        {
            Action action = db.Actions.Find(ActionID);
            if (submitButton == "Check")
            {
                try
                {
                    checkPlanification(action, dateFrom, startTimePeriod, endTimePeriod);
                }
                catch (CalendarException cex)
                {
                    ModelState.AddModelError(string.Empty, cex.Message);
                }
            }
            if (ModelState.IsValid)
            {
                bool save = false;
                DateTime start = dateFrom.AddMinutes(startTimePeriod * Settings.POMOCYCLE);
                DateTime end = dateFrom.AddMinutes(endTimePeriod * Settings.POMOCYCLE);
                DateTime current = start;

                while (current < end)
                {
                    save = true;
                    var p = new Pomodoro();
                    p.ActionID = ActionID;
                    p.Status = PomodoroStatus.Planified;
                    p.Start = current;
                    db.Pomodoros.Add(p);
                    current = current.AddMinutes(Settings.POMOCYCLE);
                }
                if (save)
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.dateFrom = Settings.DefaultPlanifyDateTime.ToString("dd/MM/yyyy");
            ViewBag.startTimePeriod = startTimeSelectList;
            ViewBag.endTimePeriod = endTimeSelectList;
            return View(action);
        }

        #endregion
    }
}
