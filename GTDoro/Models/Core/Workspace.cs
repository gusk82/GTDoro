using GTDoro.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace GTDoro.Models
{
    [NotMapped]
    public class Workspace : ProjectContainer
    {
        private ApplicationUser user;
        private List<Task> tasks;
        private List<Action> actions;
        private List<Pomodoro> pomodoros;

        public Workspace(ApplicationUser user)
        {
            this.user = user;
        }

        private void LoadUserWork()
        {
            tasks = new List<Task>();
            actions = new List<Action>();
            pomodoros = new List<Pomodoro>();
            foreach (Project p in GetProjects())
            {
                tasks.AddRange(p.GetTasks());
                actions.AddRange(p.GetActions());
                pomodoros.AddRange(p.GetPomodoros());
            }
        }
        
        public override ICollection<Project> GetProjects()
        {
            return user.Projects;
        }

        public override ICollection<Task> GetTasks()
        {
            if (tasks == null)
            {
                LoadUserWork();
            }
            return tasks;
        }

        public override ICollection<Action> GetActions()
        {
            if (actions == null)
            {
                LoadUserWork();
            }
            return actions;
        }

        public override string PathItemName
        {
            get { return "Workspace"; }
        }

        public override string ItemName
        {
            get { return "Workspace for " + user.Email; }
        }

        public override PomodoroContainerType Type
        {
            get { return PomodoroContainerType.Workspace; }
        }

        public override int Ident
        {
            get { return 0; }
        }

        public override string CssClass
        {
            get { return "gt-workspace"; }
        }

        public override bool InheritedStatus
        {
            get { return false; }
        }

        public override ICollection<Pomodoro> GetPomodoros()
        {
            if (pomodoros == null)
            {
                LoadUserWork();
            }
            return pomodoros;
        }

        public override bool IsSelectable
        {
            get
            {
                return GetProjects().Where(p => p.IsSelectable).Count() > 0;
            }
        }
        
        public override bool ContainsSelectedAction
        {
            get { return true; }
        }

        public override ApplicationUser Owner
        {
            get { return user; }
        }

        public override PomodoroContainer Parent { get { return null; } }
        public override PomodoroContainer NextSibling { get { return null; } }

        public override Status Status { get; set; }
        public override DateTime? CreationDate { get; set; }
        public override DateTime? EndDate { get; set; }

        public override Color TypeColor
        {
            get { return Color.Black; }
        }
    }
}