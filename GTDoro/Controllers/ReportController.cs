using GTDoro.DAL;
using GTDoro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using GTDoro.ViewModels.Reports;
using GTDoro.ViewModels;
using System.Globalization;

namespace GTDoro.Controllers
{
    public class ReportController : BaseController
    {
        private GTDoroContext db = new GTDoroContext();

        //
        // GET: /Reports/
        public ActionResult Index()
        {
            //return RedirectToAction("WorkHistory");
            return View();
        }

        public ViewResult WorkHistory(string start, string end, int? statusFilter)
        {
            DateTime dtStart = DateTime.Today.AddDays(-29);
            if (!string.IsNullOrWhiteSpace(start))
            {
                DateTime.TryParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStart);
            }
            ViewBag.start = dtStart.ToString("dd/MM/yyyy");

            DateTime dtEnd = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(start))
            {
                DateTime.TryParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEnd);
            }
            ViewBag.end = dtEnd.ToString("dd/MM/yyyy");

            var pomodoros = db.GetMyPomodoros(User).ToList().Where(
                p => p.Start.HasValue && p.StartLocal.Value.Date >= dtStart && p.StartLocal.Value.Date <= dtEnd);

            if (statusFilter.HasValue)
            {
                pomodoros = pomodoros.Where(p => p.Status == (PomodoroStatus)statusFilter.Value);
            }

            ViewBag.reportType = new SelectList(new string[] { "Working history" });

            WorkHistoryViewModel model = new WorkHistoryViewModel();
            model.WorkHistory = pomodoros.ToList();
            model.ReportType = ReportType.WorkHistory;
            model.Start = dtStart;
            model.End = dtEnd;
            return View(model);
        }
	}
}