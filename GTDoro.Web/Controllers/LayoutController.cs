using GTDoro.Core.DAL;
using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using GTDoro.Core.Models.Identity;
using GTDoro.Core.ViewModels;
using GTDoro.Core.ViewModels.Reports;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTDoro.Web.Models.ActionFilters;
using System.Net.Mail;
using GTDoro.Web.ViewModels;
using GTDoro.Web.Models.Identity;

namespace GTDoro.Controllers
{
    [Authorize]
    public class LayoutController : BaseController
    {
        private GTDoroContext db;
        private UserManager<ApplicationUser> manager;

        public LayoutController()
        {
            db = new GTDoroContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: /Layout/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Dashboard
        public ViewResult Dashboard()
        {
            DashboardViewModel dashboard = new DashboardViewModel();

            //active projects
            dashboard.ActiveProjects = db.GetMyProjects(User).ToList().Where(p => p.IsActive);

            //active tasks
            dashboard.ActiveTasks = db.GetMyTasks(User).ToList().Where(t => t.IsActive);

            //active actions
            dashboard.ActiveActions = db.GetMyActions(User).ToList().Where(a => a.IsActive);
            
            //completed pomodoros
            dashboard.CompletedPomodoros = db.GetMyPomodoros(User).ToList().Where(p => p.Status == PomodoroStatus.Completed);
            
            //last active actions
            dashboard.LastActiveActions = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Last Active Actions",
                TableId = "last-active-actions",
                IconCssClass = "fa-play",
                ReportTypeDate = ReportTypeDate.LastPomodoro,
                RowsPerPage = 5,
                Type = PomodoroContainerType.Action,
                Items = db.GetMyPomodoros(User)
                    .Where(pm => pm.Status == PomodoroStatus.Completed && pm.Start.HasValue)
                    .Select(pm => pm.Action)
                    .Distinct().ToList()
                    .OrderByDescending(a => a.LastPomodoro.Start.Value)
                    .Take(20)
            };

            //last active tasks
            dashboard.LastActiveTasks = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Last Active Tasks",
                TableId = "last-active-tasks",
                IconCssClass = "fa-play-circle",
                ReportTypeDate = ReportTypeDate.LastPomodoro,
                RowsPerPage = 5,
                Type = PomodoroContainerType.Task,
                Items = db.GetMyPomodoros(User)
                    .Where(pm => pm.Status == PomodoroStatus.Completed && pm.Start.HasValue)
                    .Select(pm => pm.Action.Task)
                    .Distinct().ToList()
                    .OrderByDescending(t => t.LastPomodoro.Start.Value)
                    .Take(20)
            };
                            
            //upcoming deadlines (actions)
            dashboard.UpcomingDeadlines = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Upcoming Deadlines",
                TableId = "upcoming-deadlines",
                IconCssClass = "fa-clock-o",
                ReportTypeDate = ReportTypeDate.NextDeadline,
                RowsPerPage = 5,
                Type = PomodoroContainerType.Action,
                Items = db.GetMyActions(User)
                    .Where(a => a.Status == Status.Active && a.Deadline.HasValue)
                    .ToList()
                    .OrderBy(a => a.NextDeadline.Value)
                    .Take(20)
            };

            ////upcoming planified work (actions)
            //dashboard.UpcomingPlanifiedActions = new PomodoroContainerDateViewModel
            //{
            //    HeaderTitle = "Upcoming Planified Actions",
            //    TableId = "upcoming-planified-actions",
            //    IconCssClass = "fa-calendar",
            //    ReportTypeDate = ReportTypeDate.NextPlanifiedPomodoro,
            //    RowsPerPage = 5,
            //    Items = db.GetMyPomodoros(User).ToList()
            //        .Where(pm => pm.CalculatedStatus == PomodoroCalculatedStatus.Planified && pm.Start.HasValue)
            //        .Select(pm => pm.Action)
            //        .Distinct().ToList()
            //        .OrderBy(a => a.NextPlanifiedPomodoro.Start.Value)
            //        .Take(20)
            //};

            //last active actions
            dashboard.LastCreatedActions = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Last Created Actions",
                TableId = "last-created-actions",
                IconCssClass = "fa-plus-circle",
                ReportTypeDate = ReportTypeDate.LastCreationDate,
                RowsPerPage = 5,
                Type = PomodoroContainerType.Action,
                Items = db.GetMyActions(User)
                    .Where(a => a.CreationDate.HasValue)
                    .ToList()
                    .OrderByDescending(a => a.CreationDateLocal.Value)
                    .Take(20)
            };

            //work amount (line-chart)
            dashboard.WorkAmountChartViewModel = new MorrisLineChartViewModel
            {
                HeaderTitle = "Work Amount",
                Dates = db.GetMyPomodoros(User).ToList()
                    .Where(p => p.Start.HasValue && p.CalculatedStatus == LoggableItemCalculatedStatus.Completed)
                    .Select(p => p.StartLocal.Value).AsEnumerable(),
                Interval = DateInterval.Monthly,
                Label = "Work units",
                HtmlElementId = "work-amount"
            };

            dashboard.WorkDivisionViewModel = new MorrisDonutChartViewModel
            {
                HeaderTitle = "Work Division",
                Items = db.GetMyProjects(User)
                    .AsEnumerable()
                    .OrderByDescending(p => p.CompletedPomodorosCount),
                HtmlElementId = "work-division"
            };

            dashboard.MyWorkspace = db.GetMyWorkspace(User);
            //dashboard.WorkDivisionViewModels = new List<MorrisDonutChartViewModel> { 
            //    //work project division (donut chart)
            //    new MorrisDonutChartViewModel
            //    {
            //        HeaderTitle = "Projects",
            //        Items = db.GetMyProjects(User),
            //        HtmlElementId = "project-chart"
            //    },
            //    //work task division (donut chart)
            //     new MorrisDonutChartViewModel
            //    {
            //        HeaderTitle = "Tasks",
            //        Items = db.GetMyTasks(User),
            //        HtmlElementId = "task-chart",
            //        UseFullPathItemName = true
            //    }
            //};

            return View(dashboard);
        }

        // GET: /Calendar
        public ActionResult Calendar()
        {
            CalendarViewModel calendar = new CalendarViewModel();
            calendar.WorkHistory = db.GetMyPomodoros(User).Where(p => p.Status == PomodoroStatus.Completed)
                .Include(p => p.Action)
                .Include(p=> p.Action.Task);
            calendar.PlanifiedWork = db.GetMyPomodoros(User).Where(p => p.Status == PomodoroStatus.Planified);
            calendar.ActiveActions = db.GetMyActions(User).Where(a => a.Status == Status.Active);

            return View(calendar);
        }

        // GET: /Review
        public ViewResult Review()
        {
            ReviewViewModel review = new ReviewViewModel();

            //collected thins
            review.CollectedThings = new CollectedThingDateViewModel
            {
                HeaderTitle = "Collected Things",
                TableId = "collected-things",
                IconCssClass = "fa-plus-circle",
                RowsPerPage = 7,
                Items = db.GetMyCollectedThings(User).ToList()
                    .Take(21).ToList()
            };     

            //inactive actions
            TimeSpan InactiveTimespanLimit = new TimeSpan(30, 0, 0, 0, 0); //30 days
            review.InactiveActions = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Inactive Actions",
                TableId = "inactive-actions",
                IconCssClass = "fa-stop",
                ReportTypeDate = ReportTypeDate.LastPomodoro,
                RowsPerPage = 7,
                Type = PomodoroContainerType.Action,
                Items = db.GetMyActions(User).ToList()
                    .Where(a => a.IsActive && 
                        a.LastPomodoro != null && a.LastPomodoro.Start.HasValue &&
                        (DateTime.Today.Date.Subtract(a.LastPomodoro.StartLocal.Value) > InactiveTimespanLimit))
                    .Take(21).ToList()
            };

            //expired actions
            review.ExpiredActions = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Expired Actions",
                TableId = "expired-actions",
                IconCssClass = "fa-calendar-o",
                ReportTypeDate = ReportTypeDate.NextDeadline,
                RowsPerPage = 3,
                Type = PomodoroContainerType.Action,
                Items = db.GetMyActions(User).ToList()
                    .Where(a => a.IsExpired && a.IsActive).OrderBy(a => a.Deadline).ToList()
                    .Take(15).ToList()
            };     

            //tasks with no active actions
            review.TaskWithNoActiveActions = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Task With No Active Actions",
                TableId = "tasks-no-active-actions",
                IconCssClass = "fa-dot-circle-o",
                ReportTypeDate = ReportTypeDate.LastPomodoro,
                RowsPerPage = 3,
                Type = PomodoroContainerType.Task,
                Items = db.GetMyTasks(User)
                    .Where(t => t.Status == Status.Active && t.Project.Status == Status.Active && !db.Actions
                        .Any(a => a.TaskID == t.ID && a.Status == Status.Active)).ToList()
                        .OrderByDescending(t => (t.LastPomodoro != null ? t.LastPomodoro.Start : DateTime.MinValue))
                        .Take(15).ToList()
            };

            //projects with no active tasks
            review.ProjectsWithNoActiveTasks = new PomodoroContainerDateViewModel
            {
                HeaderTitle = "Project With No Active Tasks",
                TableId = "projects-no-active-tasks",
                IconCssClass = "fa-bullseye",
                ReportTypeDate = ReportTypeDate.LastPomodoro,
                RowsPerPage = 3,
                Type = PomodoroContainerType.Project,
                Items = db.GetMyProjects(User)
                    .Where(p => p.Status == Status.Active && !db.Tasks
                        .Any(t => t.ProjectID == p.ID && t.Status == Status.Active)).ToList()
                        .OrderByDescending(p => (p.LastPomodoro != null ? p.LastPomodoro.Start : DateTime.MinValue))
                        .Take(15).ToList()
            };

            return View(review);
        }

        //
        // GET: /Reports
        public ActionResult Reports()
        {
            return RouteToReport(ReportType.WorkHistory);
        }
        // POST: /Reports
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reports(ReportType reportType, string start, string end)
        {
            return RouteToReport(reportType, start, end);
        }

        private RedirectToRouteResult RouteToReport(ReportType reportType, string start = "", string end = "")
        {
            switch(reportType)
            {
                //case ReportType.WorkHistory:
                default:
                    return RedirectToAction("WorkHistory", "Report", new { start = start, end = end });
            }
        }
        
        // GET: /Help
        public ActionResult Help()
        {
            return RedirectToAction("Basics", "Help");
        }

        // GET: /Admin
        [AdminActionFilter]
        public ActionResult Admin()
        {
            AdminViewModel viewModel = new AdminViewModel();
            viewModel.Users = db.Users.AsEnumerable();
            viewModel.MessageHtmlContent = db.GetParameterValueByCode(ParameterType.RegMessageContent);
                        
            //registered users
            viewModel.RegisteredUsersChartViewModel = new MorrisLineChartViewModel
            {
                HeaderTitle = "Registered Users",
                Dates = db.Users.Where(u => u.CreationDate.HasValue).AsEnumerable().Select(u => u.CreationDateLocal.Value).AsEnumerable(),
                Interval = DateInterval.Monthly,
                Label = "Users",
                HtmlElementId = "registered-users"
            };

            return View(viewModel);
        }
        
        // POST: /Admin
        [AdminActionFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Admin(string MessageHtmlContent, string submit)
        {
            switch(submit)
            {
                case "SaveRegMessage":
                    db.SetParameterValue(ParameterType.RegMessageContent, MessageHtmlContent);
                    break;
                case "SendMessage":
                    using (SmtpClient client = new SmtpClient())
                    {
                        MailMessage message = new MailMessage("admin@gtdoro.com", "users@gtdoro.com");
                        message.Subject = "Message from GTDoro.com";
                        message.Body = MessageHtmlContent;
                        message.IsBodyHtml = true;
                        foreach (ApplicationUser u in db.Users)
                        {
                            message.Bcc.Add(u.UserName);
                        }
                        client.Send(message);
                        TempData["UpdateInfo"] = Settings.MSG_SUCCESSFUL_OPERATION;
                    }
                    break;
            }

            return RedirectToAction("Admin");
        }

        public ActionResult MyAccount()
        {
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser appUser = manager.FindById(User.Identity.GetUserId());
            SettingsViewModel viewModel = new SettingsViewModel()
            {
                ChangePasswordViewModel = new ChangePasswordViewModel(),
                TimeZoneId = appUser.TimeZoneId
            };
            return View(viewModel);
        }
        
        [HttpPost, ActionName("MyAccount")]
        public ActionResult ChangeSettings(string TimeZoneId)
        {
            return View();
        }

        #region Disabled actions



        #endregion

    }
}