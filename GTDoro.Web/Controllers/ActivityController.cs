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
using PagedList;
using System.Data.Entity.Infrastructure;
using GTDoro.Core.Exception;
using System.Data.Entity.Core.Objects;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Core.Models.Identity;
using Activity = GTDoro.Core.Models.Activity;
using GTDoro.Web.Models.ActionFilters;
using System.Web.Routing;
using GTDoro.Core.Extensions;
using GTDoro.Web.Extensions;

namespace GTDoro.Controllers
{
    [Authorize]
    public class ActivityController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Activity/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.ProjectSortParm = (sortOrder == "proj") ? "proj_desc" : "proj";
            ViewBag.TaskSortParm = (sortOrder == "task") ? "task_desc" : "task";
            ViewBag.ActivitySortParm = (sortOrder == "acti") ? "acti_desc" : "acti";
            ViewBag.DateSortParm = (sortOrder == "date") ? "date_desc" : "date";
            ViewBag.EstimateSortParm = (sortOrder == "esti") ? "esti_desc" : "esti";
            ViewBag.IsPersistentSortParm = (sortOrder == "pers") ? "pers_desc" : "pers";
            ViewBag.StatusSortParm = (sortOrder == "stat") ? "stat_desc" : "stat";
            ViewBag.PrioritySortParm = (sortOrder == "prio") ? "prio_desc" : "prio";
            ViewBag.LastTimePeriodSortParm = (sortOrder == "last") ? "last_desc" : "last";
            ViewBag.TimePeriodsSortParm = (sortOrder == "pomo") ? "pomo_desc" : "pomo";
            ViewBag.EffortSortParm = (sortOrder == "effo") ? "effo_desc" : "effo";

            var activities = db.GetMyActivities(User);

            //stores the current filter info for the pagination control
            RouteValueDictionary dict = new RouteValueDictionary();
            //initilised with the current sort order
            dict["CurrentSort"] = sortOrder;

            //status filter
            const string STATUS_PREFIX = "status-";
            string[] statusFilter = GetArrayParamsFromRequest(Request.QueryString, STATUS_PREFIX);
            if (statusFilter.Length > 0)
            {
                activities = activities.ToList().AsQueryable()
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
                activities = activities.ToList().AsQueryable()
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
                activities = activities.ToList().AsQueryable()
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
                    activities = activities.OrderBy(a => a.Task.Project.Code).ThenBy(a => a.Task.Code).ThenBy(a => a.Name);
                    break;
                case "proj_desc":
                    activities = activities.OrderByDescending(a => a.Task.Project.Code).ThenByDescending(a => a.Task.Code).ThenByDescending(a => a.Name);
                    break;
                case "task":
                    activities = activities.OrderBy(a => a.Task.Code).ThenBy(a => a.Name);
                    break;
                case "task_desc":
                    activities = activities.OrderByDescending(a => a.Task.Code).ThenByDescending(a => a.Name);
                    break;
                case "acti":
                    activities = activities.OrderBy(a => a.Name);
                    break;
                case "acti_desc":
                    activities = activities.OrderByDescending(a => a.Name);
                    break;
                case "esti":
                    activities = activities.OrderBy(a => a.Estimate);
                    break;
                case "esti_desc":
                    activities = activities.OrderByDescending(a => a.Estimate);
                    break;
                case "pers":
                    activities = activities.ToList().AsQueryable().OrderBy(a => a.IsPersistent);
                    break;
                case "pers_desc":
                    activities = activities.ToList().AsQueryable().OrderByDescending(a => a.IsPersistent);
                    break;
                case "last":
                    activities = activities.ToList().AsQueryable().OrderBy(t => t.LastTimePeriod.ToDateTicksOrZero());
                    break;
                case "last_desc":
                    activities = activities.ToList().AsQueryable().OrderByDescending(t => t.LastTimePeriod.ToDateTicksOrZero());
                    break;
                //case "pomo":
                //    activities = activities.ToList().AsQueryable().OrderBy(a => a.CompletedTimePeriodsCount);
                //    break;
                //case "pomo_desc":
                //    activities = activities.ToList().AsQueryable().OrderByDescending(a => a.CompletedTimePeriodsCount);
                //    break;
                case "date":
                    activities = activities.ToList().AsQueryable().OrderBy(a => a.DeadlineOrEndDate.ToTicksOrZero());
                    break;
                case "date_desc":
                    activities = activities.ToList().AsQueryable().OrderByDescending(a => a.DeadlineOrEndDate.ToTicksOrZero());
                    break;
                case "stat":
                    activities = activities.ToList().AsQueryable().OrderBy(a => a.CalculatedStatus);
                    break;
                case "stat_desc":
                    activities = activities.ToList().AsQueryable().OrderByDescending(a => a.CalculatedStatus);
                    break;
                case "prio":
                    activities = activities.OrderBy(a => a.Priority);
                    break;
                case "prio_desc":
                    activities = activities.OrderByDescending(a => a.Priority);
                    break;
                //case "effo":
                //    activities = activities.ToList().AsQueryable().OrderBy(t => t.Effort.ToDecimalOrZero()).ThenBy(t => t.CompletedTimePeriodsCount);
                //    break;
                //case "effo_desc":
                //    activities = activities.ToList().AsQueryable().OrderByDescending(t => t.Effort.ToDecimalOrZero()).ThenByDescending(t => t.CompletedTimePeriodsCount);
                //    break;
                default:
                    activities = activities.ToList().AsQueryable().OrderBy(a => a.Status).ThenByDescending(a => a.LastTimePeriod.ToDateTicksOrZero());
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(activities.ToPagedList(pageNumber, pageSize));
        }

        // POST: /Activity/
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit(int[] selectedActivities, int? newStatus, int? newPriority)
        {
            if (selectedActivities != null && selectedActivities.Length > 0)
            {
                string[] selectedTags = GetArrayParamsFromRequest(Request.Form, "tag-");

                if (newStatus.HasValue || newPriority.HasValue || selectedTags.Length > 0)
                {
                    var activitiesToUpdate = db.GetMyActivities(User)
                       .Include(t => t.Tags)
                       .Where(a => selectedActivities.ToList().Contains(a.ID)).ToList();

                    try
                    {
                        foreach (var activity in activitiesToUpdate)
                        {
                            if (newStatus.HasValue)
                            {
                                activity.Status = (Status)newStatus.Value;
                            }
                            if (newPriority.HasValue)
                            {
                                activity.Priority = (LevelExtended)newPriority.Value;
                            }
                            if (selectedTags != null && selectedTags.Length > 0)
                            {
                                SetTagsToActivity(selectedTags, activity);
                            }
                            db.Entry(activity).State = EntityState.Modified;
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


        // GET: /Activity/Details/5
        public ActionResult LightDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: /Activity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,EndDate,Estimate,TaskID,Deadline,Status,IsPersistent,Priority,CreationDate")] Activity newActivity) //var name cannot be activity
        {
            if (ModelState.IsValid)
            {
                db.Entry(newActivity).State = EntityState.Modified;
                db.SaveChanges();

                TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
                return JavaScript("window.location = '" + Url.Action("Details", new { id = newActivity.ID }) + "'");
            }
            newActivity = db.GetActivityById(User, newActivity.ID);
            return PartialView(newActivity);
        }

        // POST: /Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Activity activity = db.GetActivityById(User, id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            //check if it's the selected activity
            if (activity.ContainsSelectedActivity)
            {
                //neither working nor pending confirmation
                if (activity.Owner.WorkingPanelAvailable)
                {
                    //clear selected activity
                    activity.Owner.ActivityID = null;
                    db.Entry(activity.Owner).State = EntityState.Modified;
                }
                else
                {
                    //cannot perform change
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            int taskID = activity.TaskID;
            db.Activities.Remove(activity);
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_DELETE;
            return RedirectToAction("Details", "Task", new { id = taskID });
        }

        // GET: /Activity/Select/5
        public ActionResult Select(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser.WorkingPanelAvailable && activity.IsSelectable)
            {
                currentUser.ActivityID = id.Value;
                db.Entry(currentUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
            }

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // POST: /Activity/Select/5
        [HttpPost, ActionName("Select")]
        public ActionResult SelectPost(int? ActivityID)
        {
            return RedirectToAction("Select", new { id = ActivityID });
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
        

        // GET: /Activity/ChangeStatus/5?status=1
        public ActionResult ChangeStatus(int? id, Status? status)
        {
            if (id == null || status.HasValue == false || Request.UrlReferrer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            if (status.Value != Status.Active)
            {
                //check if it's the selected activity
                if (activity.ContainsSelectedActivity)
                {
                    //not working nor pending confirmation
                    if (activity.Owner.WorkingPanelAvailable)
                    {
                        //clear selected activity
                        activity.Owner.ActivityID = null;
                        db.Entry(activity.Owner).State = EntityState.Modified;
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
            if (Activity.IsFinishedStatus(status.Value))
            {
                //if no end date
                if (activity.EndDate.HasValue == false)
                {
                    activity.EndDate = DateTime.UtcNow;
                }
            }
            else
            {
                //remove end date
                activity.EndDate = null;
            }

            activity.Status = status.Value;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        // GET: /Activity/ChangeTask/5?parent=1
        public ActionResult ChangeTask(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            };
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
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
                //check if contains the selected activity
                if (activity.ContainsSelectedActivity)
                {
                    //not working nor pending confirmation
                    if (activity.Owner.WorkingPanelAvailable)
                    {
                        //clear selected activity
                        activity.Owner.ActivityID = null;
                        db.Entry(activity.Owner).State = EntityState.Modified;
                    }
                    else
                    {
                        //cannot perform change
                        TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                        return Redirect(Request.GetReferrerUrlOrCurrent());
                    }
                }
            }
            activity.TaskID = parent.Value;
            db.Entry(activity).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return RedirectToAction("Details", new { id = id.Value });
        }

        // GET: /Activity/Details/5
        public ActionResult Details(int? id)
        {
            return loadView(id, Operation.ActivityInfo);
        }

        // GET: /Activity/Edit/5
        public ActionResult Edit(int? id)
        {
            return loadView(id, Operation.EditActivity);
        }

        // GET: /Activity/Delete/5
        public ActionResult Delete(int? id)
        {
            return loadView(id, Operation.DeleteActivity);
        }


        // GET: /Activity/AddTag/5?tagid=1
        public ActionResult AddTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activity = db.GetActivityById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (activity == null || tag == null)
            {
                return HttpNotFound();
            }

            if (activity.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() == null)
            {
                activity.Tags.Add(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Activity/RemoveTag/5?tagid=1
        public ActionResult RemoveTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activity = db.GetActivityById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (activity == null || tag == null)
            {
                return HttpNotFound();
            }

            if (activity.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() != null)
            {
                activity.Tags.Remove(tag);
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

        private void SetTagsToActivity(string[] selectedTagCodes, Activity activityToUpdate)
        {
            if (selectedTagCodes == null || selectedTagCodes.Length == 0)
            {
                return;
            }
            var selectedTagCodesHS = new HashSet<string>(selectedTagCodes);
            var activityTagCodes = new HashSet<string>
                (activityToUpdate.OwnAndInheritedTags.Select(c => c.Code));
            foreach (var tag in db.Tags)
            {
                if (selectedTagCodesHS.Contains(tag.Code) &&
                    activityTagCodes.Contains(tag.Code) == false)
                {
                    activityToUpdate.Tags.Add(tag);
                }
            }
        }

        private ActionResult loadView(int? id, Operation operation)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.Operation = operation;
            ViewBag.NotSelectedTags = activity.GetNotSelectedTags(db.GetMyTags(User)).ToList();
            return View("Activity", activity);
        }

        #endregion

        #region Disabled activities

        // GET: /Activity/Details/5
        [DisabledActionFilter]
        public ActionResult DetailsOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: /Activity/Edit/5
        [DisabledActionFilter]
        public ActionResult EditOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activityToUpdate = db.GetActivityById(User, id.Value);
            if (activityToUpdate == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", activityToUpdate.TaskID);
            return View(activityToUpdate);
        }

        // GET: /Activity/Delete/5
        [DisabledActionFilter]
        public ActionResult DeleteOld(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }

            if (activity.ContainsSelectedActivity)
            {
                //working or pending confirmation
                if (activity.Owner.WorkingPanelAvailable == false)
                {
                    TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                    return RedirectToAction("Details", new { id = id.Value });
                }
            }
            return View(activity);
        }

        // GET: /Activity/Create
        [DisabledActionFilter]
        public ActionResult Create(int? taskId, int? collectedThingID, string name)
        {
            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", taskId);
            ViewBag.Name = name;
            return View();
        }

        // POST: /Activity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include = "ID,Name,Description,EndDate,Estimate,TaskID,Deadline,Status")] Activity newActivity) //var name cannot be activity
        {
            int collectedThingID = 0;
            if (Request != null)
            {
                Int32.TryParse(Request.Params["collectedThingID"], out collectedThingID);
            }

            newActivity.Task = db.GetTaskById(User, newActivity.TaskID);
            if (ModelState.IsValid)
            {
                //create activity
                db.Activities.Add(newActivity);
                //set creation date
                newActivity.CreationDate = DateTime.UtcNow;
                if (collectedThingID > 0)
                {
                    //delete associated collected thing
                    CollectedThing collectedThing = db.GetCollectedThingById(User, collectedThingID);
                    db.CollectedThings.Remove(collectedThing);
                }

                db.SaveChanges();
                return RedirectToAction("ActivityList", "Task", new { id = newActivity.TaskID });
            }

            ViewBag.TaskID = new SelectList(db.GetMyTasks(User), "ID", "Code", newActivity.TaskID);
            return View(newActivity);
        }

        // GET: /Activity/Charts/5
        [DisabledActionFilter]
        public ActionResult Charts(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }

            return View("Containers/_WorkCharts", activity);
        }

        // GET: /Activity/WorkHistory/5
        [DisabledActionFilter]
        public ActionResult WorkHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.GetActivityById(User, id.Value);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View("Containers/_TimePeriodSetsList", activity);
        }

        // GET: /Activity/Work/5
        [DisabledActionFilter]
        public ActionResult Work(int? id)
        {
            //(first) pomodoro with status = working(if exists)
            TimePeriod pomodoro = db.GetMyTimePeriods(User).FirstOrDefault(p => p.Status == TimePeriodStatus.Working);
            if (pomodoro != null)
            {
                if (id.HasValue && id.Value == pomodoro.ActivityID)
                {
                    return RedirectToAction("Working", "TimePeriod");
                }
                else
                {
                    return RedirectToAction("CancelWorking", "TimePeriod", new { id = id });
                }
            }

            Activity activity = null;
            //ToDo: Check conflicts
            if (id.HasValue)
            {
                activity = db.Activities.Find(id);
                if (activity != null)
                {
                    UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var currentUser = manager.FindById(User.Identity.GetUserId());
                    currentUser.ActivityID = id.Value;
                    db.Entry(currentUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return View(activity);
        }

        // POST: /Activity/Work/5
        [HttpPost]
        [DisabledActionFilter]
        public ActionResult Work()
        {
            TimePeriod timePeriod = new TimePeriod();
            timePeriod.Start = DateTime.UtcNow;
            timePeriod.Status = TimePeriodStatus.Working;
            timePeriod.ActivityID = Int32.Parse(Request.Form["ActivityID"]);
            db.Entry(timePeriod).State = EntityState.Modified;
            db.TimePeriods.Add(timePeriod);
            db.SaveChanges();
            return RedirectToAction("Working", "TimePeriod");
        }

        // GET: /Activity/Planify/5
        [DisabledActionFilter]
        public ActionResult Planify(int? id)
        {
            Activity activity = null;
            if (id.HasValue)
            {
                activity = db.Activities.Find(id.Value);
            }

            ViewBag.dateFrom = Settings.DefaultPlanifyDateTime.ToString("dd/MM/yyyy");
            ViewBag.startTimePeriod = startTimeSelectList;
            ViewBag.endTimePeriod = endTimeSelectList;

            return View(activity);
        }

        

        #endregion
    }
}
