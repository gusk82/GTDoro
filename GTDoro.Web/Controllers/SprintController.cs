using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using GTDoro.Core.ViewModels;
using GTDoro.Core.DAL;
using PagedList;
using System.Data.Entity.Infrastructure;
using GTDoro.Web.Models.ActionFilters;
using GTDoro.Core.Models.Identity;
using GTDoro.Core.Extensions;
using System.Web.Routing;
using GTDoro.Web.Extensions;

namespace GTDoro.Controllers
{
    [Authorize]
    public class SprintController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        // GET: /Sprint/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CodeSortParm = (sortOrder == "code") ? "code_desc" : "code";
            ViewBag.NameSortParm = (sortOrder == "name") ? "name_desc" : "name";
            ViewBag.ProjectSortParm = (sortOrder == "proj") ? "proj_desc" : "proj";
            ViewBag.StartDateSortParm = (sortOrder == "star") ? "star_desc" : "star";
            ViewBag.EndDateSortParm = (sortOrder == "end") ? "end_desc" : "end";
            ViewBag.ActionsSortParm = (sortOrder == "acti") ? "acti_desc" : "acti";
            ViewBag.PomodorosSortParm = (sortOrder == "pomo") ? "pomo_desc" : "pomo";
            ViewBag.EffortSortParm = (sortOrder == "effo") ? "effo_desc" : "effo";
            ViewBag.ProgressSortParm = (sortOrder == "prog") ? "prog_desc" : "prog";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //var sprints = db.Sprints.Include(t => t.Project);
            var sprints = db.GetMySprints(User);

            //stores the current filter info for the pagination control
            RouteValueDictionary dict = new RouteValueDictionary();
            //initilised with the current sort order
            dict["CurrentSort"] = sortOrder;

            ////status filter
            //const string STATUS_PREFIX = "status-";
            //string[] statusFilter = GetArrayParamsFromRequest(Request.QueryString, STATUS_PREFIX);
            //if (statusFilter.Length > 0)
            //{
            //    sprints = sprints.ToList().AsQueryable()
            //        .Where(t => statusFilter
            //            .Any(sf => sf.ToLower() == t.CalculatedStatus.ToString().ToLower())
            //    );
            //    dict.AddFilterArrayToRouteValueDictionary(statusFilter, STATUS_PREFIX);
            //}

            ////tag filter
            //const string TAG_PREFIX = "tag-";
            //string[] tagFilter = GetArrayParamsFromRequest(Request.QueryString, TAG_PREFIX);
            //if (tagFilter.Length > 0)
            //{
            //    //OR Tag Filter
            //    sprints = sprints.ToList().AsQueryable()
            //        .Where(t => tagFilter
            //            .Any(tf => t.OwnAndInheritedTags    //all tags in the filter
            //                .Select(tc => tc.Code)          //(only tag code list)
            //                    .Contains(tf))              //contained in the item tags 
            //    );
            //    //AND Tag Filter
            //    //sprints = sprints.ToList().AsQueryable()
            //    //    .Where(t => tagFilter
            //    //        .All(tf => t.OwnAndInheritedTags    //all tags in the filter
            //    //            .Select(tc => tc.Code)          //(only tag code list)
            //    //                .Contains(tf))              //contained in the item tags 
            //    //);
            //    dict.AddFilterArrayToRouteValueDictionary(tagFilter, TAG_PREFIX);
            //}

            ////priority filter
            //const string PRIORITY_PREFIX = "priority-";
            //string[] priorityFilter = GetArrayParamsFromRequest(Request.QueryString, PRIORITY_PREFIX);
            //if (priorityFilter.Length > 0)
            //{
            //    sprints = sprints.ToList().AsQueryable()
            //        .Where(t => priorityFilter
            //            .Any(pf => pf.ToLower() == t.Priority.ToString().ToLower())
            //    );
            //    dict.AddFilterArrayToRouteValueDictionary(priorityFilter, PRIORITY_PREFIX);
            //}

            //ViewBag.IsFiltered = string.IsNullOrEmpty(Request.QueryString["filtering"]) == false ||
            //    (statusFilter.Length > 0) || (priorityFilter.Length > 0) || (tagFilter.Length > 0);
            //ViewBag.RouteFiltersForPagination = dict;

            //ViewBag.StatusFilter = statusFilter;
            //ViewBag.PriorityFilter = priorityFilter;
            //ViewBag.TagFilter = tagFilter;
            ////ViewBag.CurrentFilter = searchString;

            ViewBag.AllTags = db.GetMyTags(User).ToArray();

            orderBySortParam(ref sprints, sortOrder);

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(sprints.ToPagedList(pageNumber, pageSize));
        }

        // POST: /Sprint/
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ListEdit(int[] selectedSprints, int? newStatus, int? newPriority)
        {
            if (selectedSprints != null && selectedSprints.Length > 0)
            {
                string[] selectedTags = GetArrayParamsFromRequest(Request.Form, "tag-");

                if (newStatus.HasValue || newPriority.HasValue || selectedTags.Length > 0)
                {
                    var sprintsToUpdate = db.GetMySprints(User)
                       .Include(t => t.Tags)
                       .Where(t => selectedSprints.ToList().Contains(t.ID)).ToList();

                    try
                    {
                        foreach (var sprint in sprintsToUpdate)
                        {
                            if (newStatus.HasValue)
                            {
                                sprint.Status = (Status)newStatus.Value;
                            }
                            if (selectedTags != null && selectedTags.Length > 0)
                            {
                                SetTagsToSprint(selectedTags, sprint);
                            }
                            db.Entry(sprint).State = EntityState.Modified;

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

        // POST: /Sprint/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditSprint(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sprintToUpdate = db.GetMySprints(User)
               .Include(t => t.Tags)
               .Where(t => t.ID == id)
               .Single();

            if (TryUpdateModel(sprintToUpdate, "",
               new string[] { "ID", "Name", "StartDate", "EndDate", "ProjectID", "InitialComment", "FinalComment", "CreationDate" }))
            {
                try
                {
                    db.Entry(sprintToUpdate).State = EntityState.Modified;
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
            return PartialView(sprintToUpdate);
        }

        // POST: /Sprint/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Sprint sprint = db.GetSprintById(User, id);
            if (sprint == null)
            {
                return HttpNotFound();
            }
            if (sprint.ContainsSelectedAction)
            {
                //not working nor pending confirmation
                if (sprint.Owner.WorkingPanelAvailable)
                {
                    //clear selected action
                    sprint.Owner.ActionID = null;
                    db.Entry(sprint.Owner).State = EntityState.Modified;
                }
                else
                {
                    //cannot perform change
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            int projectID = sprint.ProjectID;
            db.Sprints.Remove(sprint);
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_DELETE;
            //return Redirect(UrlReferrer);
            return RedirectToAction("Details", "Project", new { id = projectID });
        }

        // GET: /Sprint/ChangeStatus/5?status=1
        public ActionResult ChangeStatus(int? id, Status? status)
        {
            if (id == null || status.HasValue == false || Request.UrlReferrer == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sprint sprint = db.GetSprintById(User, id.Value);
            if (sprint == null)
            {
                return HttpNotFound();
            }
            if (status.Value != Status.Active)
            {
                //check if contains the selected action
                if (sprint.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (sprint.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        sprint.Owner.ActionID = null;
                        db.Entry(sprint.Owner).State = EntityState.Modified;
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
            if (Sprint.IsFinishedStatus(status.Value))
            {
                //if no end date
                if (sprint.EndDate.HasValue == false)
                {
                    sprint.EndDate = DateTime.UtcNow;
                }
            }
            else
            {
                //remove end date
                sprint.EndDate = null;
            }

            sprint.Status = status.Value;
            db.Entry(sprint).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        // GET: /Sprint/ChangeProject/5?parent=1
        public ActionResult ChangeProject(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Sprint sprint = db.GetSprintById(User, id.Value);
            if (sprint == null)
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
                if (sprint.ContainsSelectedAction)
                {
                    //not working nor pending confirmation
                    if (sprint.Owner.WorkingPanelAvailable)
                    {
                        //clear selected action
                        sprint.Owner.ActionID = null;
                        db.Entry(sprint.Owner).State = EntityState.Modified;
                    }
                    else
                    {
                        //cannot perform change
                        TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                        return Redirect(Request.GetReferrerUrlOrCurrent());
                    }
                }
            }
            sprint.ProjectID = parent.Value;
            db.Entry(sprint).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return RedirectToAction("Details", new { id = id.Value });
        }

        // GET: /Sprint/Edit/5
        public ActionResult Edit(int? id)
        {
            return loadView(id, Operation.EditSprint);
        }

        // GET: /Sprint/Delete/5
        public ActionResult Delete(int? id)
        {
            return loadView(id, Operation.DeleteSprint);
        }


        public ActionResult Details(int? id)
        {
            return loadView(id, Operation.SprintInfo);
        }

        
        // GET: /Sprint/AddTag/5?tagid=1
        public ActionResult AddTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sprint = db.GetSprintById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (sprint == null || tag == null)
            {
                return HttpNotFound();
            }

            if (sprint.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() == null)
            {
                sprint.Tags.Add(tag);
            }
            db.SaveChanges();

            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Sprint/RemoveTag/5?tagid=1
        public ActionResult RemoveTag(int? id, int? tagid)
        {
            if (id == null || tagid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sprint = db.GetSprintById(User, id.Value);
            var tag = db.GetTagById(User, tagid.Value);

            if (sprint == null || tag == null)
            {
                return HttpNotFound();
            }

            if (sprint.Tags.Where(t => t.ID == tag.ID).FirstOrDefault() != null)
            {
                sprint.Tags.Remove(tag);
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

        private void orderBySortParam(ref IQueryable<Sprint> sprints, string sortOrder)
        {
            switch (sortOrder)
            {
                case "code":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.Code);
                    break;
                case "code_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.Code);
                    break;
                case "name":
                    sprints = sprints.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    sprints = sprints.OrderByDescending(s => s.Name);
                    break;
                case "proj":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.Project.Code).ThenBy(s => s.Code);
                    break;
                case "proj_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.Project.Code).ThenByDescending(s => s.Code);
                    break;
                case "star":
                    sprints = sprints.OrderBy(s => s.StartDate);
                    break;
                case "star_desc":
                    sprints = sprints.OrderByDescending(s => s.StartDate);
                    break;
                case "end":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.EndDate);
                    break;
                case "end_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.EndDate);
                    break;
                case "acti":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.GetActions().Count());
                    break;
                case "acti_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.GetActions().Count());
                    break;
                case "pomo":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.CompletedPomodorosCount);
                    break;
                case "pomo_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.CompletedPomodorosCount);
                    break;
                case "prog":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.Progress.ToDecimalOrZero());
                    break;
                case "prog_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.Progress.ToDecimalOrZero());
                    break;
                case "effo":
                    sprints = sprints.ToList().AsQueryable().OrderBy(s => s.Effort.ToDecimalOrZero()).ThenBy(s => s.CompletedPomodorosCount);
                    break;
                case "effo_desc":
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.Effort.ToDecimalOrZero()).ThenByDescending(s => s.CompletedPomodorosCount);
                    break;
                default:
                    sprints = sprints.ToList().AsQueryable().OrderByDescending(s => s.StartDate.Ticks);
                    break;
            }
        }

        private void SetTagsToSprint(string[] selectedTagCodes, Sprint sprintToUpdate)
        {
            if (selectedTagCodes == null || selectedTagCodes.Length == 0)
            {
                return;
            }
            var selectedTagCodesHS = new HashSet<string>(selectedTagCodes);
            var sprintTagCodes = new HashSet<string>
                (sprintToUpdate.OwnAndInheritedTags.Select(c => c.Code));
            foreach (var tag in db.Tags)
            {
                if (selectedTagCodesHS.Contains(tag.Code) &&
                    sprintTagCodes.Contains(tag.Code) == false)
                {
                    sprintToUpdate.Tags.Add(tag);
                }
            }
        }

        private ActionResult loadView(int? id, Operation operation)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sprint = db.GetSprintById(User, id.Value);

            if (sprint == null)
            {
                return HttpNotFound();
            }

            ViewBag.Operation = operation;
            ViewBag.NotSelectedTags = sprint.GetNotSelectedTags(db.GetMyTags(User)).ToList();
            return View("Sprint", sprint);
        }

        #endregion

    }
}
