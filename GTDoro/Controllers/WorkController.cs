using GTDoro.DAL;
using GTDoro.Models;
using Action = GTDoro.Models.Action;
using GTDoro.Models.Identity;
using GTDoro.ViewModels;
using GTDoro.ExtensionMethods;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTDoro.Models.ActionFilters;

namespace GTDoro.Controllers
{
    [Authorize]
    public class WorkController : BaseController
    {
        private GTDoroContext db;
        private UserManager<ApplicationUser> manager;

        public WorkController()
        {
            db = new GTDoroContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        //
        // GET: /Work/Start
        public RedirectResult Start()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser.WorkingPanelAvailable)
            {
                if (currentUser.ActionID.HasValue && currentUser.ActionID.Value > 0)
                {
                    if (currentUser.Action.IsSelectable)
                    {
                        Pomodoro pomodoro = new Pomodoro();
                        pomodoro.Start = DateTime.UtcNow;
                        pomodoro.Status = PomodoroStatus.Working;
                        pomodoro.ActionID = currentUser.ActionID.Value;
                        db.Entry(pomodoro).State = EntityState.Modified;
                        db.Pomodoros.Add(pomodoro);
                    }
                    else
                    {
                        currentUser.ActionID = null;
                        db.Entry(currentUser).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }
        
        public PartialViewResult Working()
        {
            WorkingPanelViewModel viewModel = new WorkingPanelViewModel();
            //(first) pomodoro with status = working(if exists)
            viewModel.pomodoro = db.GetMyPomodoros(User)
                .Where(p => p.Start.HasValue)
                .OrderByDescending(p => p.Start)
                .FirstOrDefault(p => p.Status == PomodoroStatus.Working);
            
            if (viewModel.pomodoro != null)
            {
                //target time reached -> pomodoro is completed
                if (viewModel.TargetTime <= DateTime.UtcNow)
                {
                    viewModel.pomodoro.Status = PomodoroStatus.Unconfirmed;
                    db.Entry(viewModel.pomodoro).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            //not exists
            if (viewModel.pomodoro == null)
            {
                //(first) pomodoro with status = unconfirmed(if exists)
                viewModel.pomodoro = db.GetMyPomodoros(User)
                    .Where(p => p.Start.HasValue)
                    .OrderByDescending(p => p.Start)
                    .FirstOrDefault(p => p.Status == PomodoroStatus.Unconfirmed);
            }
            
            if (viewModel.pomodoro != null){
                viewModel.action = viewModel.pomodoro.Action;
            }
            else
            {
                var currentUser = manager.FindById(User.Identity.GetUserId());
                if (currentUser.ActionID.HasValue && currentUser.ActionID.Value > 0)
                {
                    viewModel.action = db.GetActionById(User, currentUser.ActionID.Value);
                }
            }
            if (viewModel.action != null)
            {
                ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name", viewModel.action);
            }
            else
            {
                ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name");
            }

            return PartialView("Panels/_WorkingPanel", viewModel);
        }

        // GET: /Work/Confirm/5
        public RedirectResult Confirm(int id)
        {
            Pomodoro pomodoro = db.GetPomodoroById(User, id);
            if (pomodoro.Status == PomodoroStatus.Unconfirmed)
            {
                pomodoro.Status = PomodoroStatus.Completed;
                db.SaveChanges();
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Work/CancelUnconfirmed/5
        public RedirectResult CancelUnconfirmed(int id)
        {
            Pomodoro pomodoro = db.GetPomodoroById(User, id);
            if (pomodoro.Status == PomodoroStatus.Unconfirmed)
            {
                pomodoro.Status = PomodoroStatus.Cancelled;
                db.SaveChanges();
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Work/CancelWorking/5
        public RedirectResult CancelWorking()
        {
            //(first) pomodoro with status = working(if exists)
            Pomodoro pomodoro = db.GetMyPomodoros(User).FirstOrDefault(p => p.Status == PomodoroStatus.Working);
            if (pomodoro != null)
            {
                pomodoro.Status = PomodoroStatus.Cancelled;
                db.SaveChanges();
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        // GET: /Work/RestartWork
        public RedirectResult RestartWork()
        {
            //(first) pomodoro with status = working(if exists)
            Pomodoro pomodoro = db.GetMyPomodoros(User).FirstOrDefault(p => p.Status == PomodoroStatus.Working);
            if (pomodoro != null)
            {
                pomodoro.Start = DateTime.UtcNow;
                db.SaveChanges();
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        public RedirectResult AutoSelect()
        {            
            ApplicationUser currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser.ActionID.HasValue == false && currentUser.WorkingPanelAvailable)
            {
                Action autoSelectedAction = null;
                autoSelectedAction = db.GetMyActions(User).ToList()
                    .Where(a => a.IsSelectable)
                    .OrderByDescending(a => a.Priority)
                    .ThenBy(a => a.Deadline ?? DateTime.MaxValue)
                    .ThenByDescending(a => a.LastPomodoro != null ? a.LastPomodoro.Start.Value : DateTime.MinValue)
                    .ThenByDescending(a => a.CreationDate ?? DateTime.MinValue)
                    .FirstOrDefault();
                if(autoSelectedAction != null)
                {
                    currentUser.ActionID = autoSelectedAction.ID;
                    db.Entry(currentUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect(Request.GetReferrerUrlOrCurrent());
        }

        #region Disabled actions

        //
        // GET: /Work/Accordion
        [DisabledActionFilter]
        public ViewResult Accordion()
        {
            IEnumerable<Project> projects = db.Projects
                   .Include(p => p.Tasks.Select(t => t.Actions));
            return View(projects);
        }

        [DisabledActionFilter]
        public ViewResult PendingConfirmation()
        {
            ViewBag.ActionID = new SelectList(db.GetMyActions(User), "ID", "Name");
            return View(db.GetMyPomodoros(User).Where(p => p.Status == PomodoroStatus.Unconfirmed).Include(p => p.Action));
        }

        #endregion
    }
}