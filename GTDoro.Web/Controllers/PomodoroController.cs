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
using GTDoro.Core.DAL;
using PagedList;
using GTDoro.Core.ViewModels;
using GTDoro.Web.Models.ActionFilters;
using GTDoro.Web.Extensions;

namespace GTDoro.Controllers
{
    [Authorize]
    public class PomodoroController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();
                
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //// GET: /Pomodoro/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.GetPomodoroById(User, id.Value);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            db.Pomodoros.Remove(pomodoro);
            db.SaveChanges();
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }


        // GET: /Pomodoro/ChangeAction/5?parent=1
        public ActionResult ChangeAction(int? id, int? parent)
        {
            if (id == null || parent.HasValue == false)
            {
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Pomodoro pomo = db.GetPomodoroById(User, id.Value);
            if (pomo == null)
            {
                return HttpNotFound();
            }
            if(pomo.Status == PomodoroStatus.Working || pomo.Status == PomodoroStatus.Unconfirmed)
            {                
                //cannot perform change
                TempData["UpdateError"] = Settings.MSG_UNSUCCESSFUL_BECAUSE_WORKING;
                return Redirect(Request.GetReferrerUrlOrCurrent());
            }
            Action action = db.GetActionById(User, parent.Value);
            if (action == null)
            {
                return HttpNotFound();
            }

            pomo.ActionID = parent.Value;
            db.Entry(pomo).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_UPDATE;
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        #region Disabled actions
        
        // GET: /Pomodoro/
        [DisabledActionFilter]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? statusFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProjectSortParm = (sortOrder == "proj") ? "proj_desc" : "proj";
            ViewBag.TaskSortParm = (sortOrder == "task") ? "task_desc" : "task";
            ViewBag.ActionSortParm = (sortOrder == "acti") ? "acti_desc" : "acti";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date" : "";

            var pomodoros = db.GetMyPomodoros(User);

            if (statusFilter.HasValue)
            {
                pomodoros = pomodoros.Where(p => p.Status == (PomodoroStatus)statusFilter.Value);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "proj":
                    pomodoros = pomodoros.OrderBy(p => p.Action.Task.Project.Code);
                    break;
                case "proj_desc":
                    pomodoros = pomodoros.OrderByDescending(p => p.Action.Task.Project.Code);
                    break;
                case "task":
                    pomodoros = pomodoros.OrderBy(p => p.Action.Task.Code);
                    break;
                case "task_desc":
                    pomodoros = pomodoros.OrderByDescending(p => p.Action.Task.Code);
                    break;
                case "acti":
                    pomodoros = pomodoros.OrderBy(p => p.Action.Name);
                    break;
                case "acti_desc":
                    pomodoros = pomodoros.OrderByDescending(p => p.Action.Name);
                    break;
                case "date":
                    pomodoros = pomodoros.OrderBy(p => p.Start);
                    break;
                default: // Date descending
                    pomodoros = pomodoros.OrderByDescending(p => p.Start);
                    break;
            }
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(pomodoros.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Pomodoro/Details/5
        [DisabledActionFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.GetPomodoroById(User, id.Value);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            return View(pomodoro);
        }

        // GET: /Pomodoro/Create
        [DisabledActionFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Pomodoro/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Create([Bind(Include = "ID,Start,ActionID,Status")] Pomodoro pomodoro)
        {
            if (ModelState.IsValid)
            {
                //set creation date
                pomodoro.CreationDate = DateTime.UtcNow;

                db.Pomodoros.Add(pomodoro);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = pomodoro.ID });
            }

            ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name", pomodoro.ActionID);
            return View(pomodoro);
        }

        // GET: /Pomodoro/Edit/5
        [DisabledActionFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pomodoro pomodoro = db.GetPomodoroById(User, id.Value);
            if (pomodoro == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name", pomodoro.ActionID);
            return View(pomodoro);
        }

        // POST: /Pomodoro/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult Edit([Bind(Include = "ID,Start,ActionID,Status")] Pomodoro pomodoro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pomodoro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = pomodoro.ID });
            }
            ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name", pomodoro.ActionID);
            return View(pomodoro);
        }

        // POST: /Pomodoro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [DisabledActionFilter]
        public ActionResult DeleteConfirmed(int id, String UrlReferrer)
        {
            Pomodoro pomodoro = db.GetPomodoroById(User, id);
            db.Pomodoros.Remove(pomodoro);
            db.SaveChanges();
            return Redirect(UrlReferrer);
        }

        [DisabledActionFilter]
        public ActionResult AjaxHandler(JQueryDataTableParamModel param)
        {
            IEnumerable<PomodoroSet> allPomodorosSets = PomodoroSet.GetPomodoroSets(db.GetMyPomodoros(User).ToList());
            IEnumerable<PomodoroSet> filteredPomodoros = allPomodorosSets;
            //Check whether the pomodoros should be filtered by keyword
            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //Used if particulare columns are filtered 
            //var nameFilter = Convert.ToString(Request["sSearch_1"]);
            //var addressFilter = Convert.ToString(Request["sSearch_2"]);
            //var townFilter = Convert.ToString(Request["sSearch_3"]);

            //Optionally check whether the columns are searchable at all 
            //var isNameSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
            //var isAddressSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
            //var isTownSearchable = Convert.ToBoolean(Request["bSearchable_3"]);

            //    filteredCompanies = DataRepository.GetCompanies()
            //       .Where(c => isNameSearchable && c.Name.ToLower().Contains(param.sSearch.ToLower())
            //                   ||
            //                   isAddressSearchable && c.Address.ToLower().Contains(param.sSearch.ToLower())
            //                   ||
            //                   isTownSearchable && c.Town.ToLower().Contains(param.sSearch.ToLower()));
            //}
            //else
            //{
            //    filteredCompanies = allCompanies;
            //}

            //var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            //Func<Company, string> orderingFunction = (c => sortColumnIndex == 1 && isNameSortable ? c.Name :
            //                                               sortColumnIndex == 2 && isAddressSortable ? c.Address :
            //                                               sortColumnIndex == 3 && isTownSortable ? c.Town :
            //                                               "");

            //var sortDirection = Request["sSortDir_0"]; // asc or desc
            //if (sortDirection == "asc")
            //    filteredCompanies = filteredCompanies.OrderBy(orderingFunction);
            //else
            //    filteredCompanies = filteredCompanies.OrderByDescending(orderingFunction);

            var displayedPomodoroSets = filteredPomodoros.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = displayedPomodoroSets
                .Select(n => new
                {
                    Project = n.Action.Task.Project.Name,
                    Task = n.Action.Task.Name,
                    Action = n.Action.Name
                });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allPomodorosSets.Count(),
                iTotalDisplayRecords = filteredPomodoros.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}
