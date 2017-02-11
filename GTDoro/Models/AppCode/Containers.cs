using GTDoro.ViewModels;
using GTDoro.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Drawing;
using GTDoro.Models.Identity;

namespace GTDoro.Models
{
    /// <summary>
    /// Pomodoro Container
    /// </summary>
    public abstract class PomodoroContainer
    {
        public abstract decimal? Effort { get; }
        public abstract int EstimateWork { get; }
        public abstract int CompletedPomodorosIfEstimate { get; }
        public abstract string PathItemName { get; }
        public abstract string ItemName { get; }
        public abstract PomodoroContainerType Type { get; }
        public abstract Color TypeColor { get; }
        public abstract int Ident { get; }
        public abstract String CssClass { get; }
        public abstract bool InheritedStatus { get; }
        public abstract bool IsSelectable { get; }
        public abstract bool IsFinished { get; }
        public abstract bool ContainsSelectedAction { get; }
        public abstract ApplicationUser Owner { get; }
        public abstract PomodoroContainer Parent { get; }
        public abstract PomodoroContainer NextSibling { get; }
        public abstract Status Status { get; set; }
        public abstract DateTime? CreationDate { get; set; }
        public abstract DateTime? EndDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDateLocal { get { return EndDate.ToUserLocalTime(Owner.TimeZoneId); } }

        [Display(Name = "Status")]
        public Status CalculatedStatus
        {
            get
            {
                if(InheritedStatus)
                {
                    return Parent.CalculatedStatus;
                }
                return Status;
            }
        }

        public bool IsActive
        {
            get { return CalculatedStatus == Status.Active; }
        }

        protected bool GroupDifferentDates { get; set;}

        public ICollection<Pomodoro> GetPomodoros(PomodoroCalculatedStatus status, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            if (!DateFrom.HasValue)
            {
                DateFrom = DateTime.MinValue;
            }
            if (!DateTo.HasValue)
            {
                DateTo = DateTime.MaxValue;
            }
            return GetPomodoros().Where(p => p.CalculatedStatus == status && 
                p.Start.HasValue).AsEnumerable().Where(p => 
                    p.StartLocal.Value >= DateFrom.Value && 
                    p.StartLocal.Value <= DateTo.Value).ToList();
        }

        public ICollection<Pomodoro> GetPomodoros(PomodoroStatus status, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            if (!DateFrom.HasValue)
            {
                DateFrom = DateTime.MinValue;
            }
            if (!DateTo.HasValue)
            {
                DateTo = DateTime.MaxValue;
            }
            return GetPomodoros().Where(p => p.Status == status &&
                p.Start.HasValue).AsEnumerable().Where(p =>
                    p.StartLocal.Value >= DateFrom.Value &&
                    p.StartLocal.Value <= DateTo.Value).ToList();
        }

        public abstract ICollection<Pomodoro> GetPomodoros();

        public Pomodoro FirstPomodoro
        {
            get
            {
                return GetPomodoros(PomodoroCalculatedStatus.Completed)
                    .OrderBy(p => p.Start).FirstOrDefault();
            }
        }

        public Pomodoro LastPomodoro
        {
            get
            {
                return GetPomodoros(PomodoroCalculatedStatus.Completed)
                    .OrderByDescending(p => p.Start).FirstOrDefault();
            }
        }
                
        public ICollection<PomodoroSet> PomodoroSets
        {
            get
            {
                ICollection<Pomodoro> pomodoros = GetPomodoros();
                return PomodoroSet.GetPomodoroSets(
                    pomodoros, PomodoroContainerType.Action, GroupDifferentDates)
                    .OrderByDescending(s => s.StartLocal).ToList();
            }
        }

        public bool HasPlanifiedWork
        {
            get
            {
                return GetPomodoros(PomodoroCalculatedStatus.Planified).Count > 0;
            }
        }

        public Pomodoro NextPlanifiedPomodoro
        {
            get
            {
                return GetPomodoros(PomodoroCalculatedStatus.Planified)
                    .Where(pm => pm.Start.HasValue)
                    .OrderBy(pm => pm.Start.Value)
                    .FirstOrDefault();
            }
        }

        public DateTime? NextDeadline
        {
            get
            {
                switch(Type)
                {
                    case PomodoroContainerType.Unspecified:
                        return null;
                    case PomodoroContainerType.Action:
                        return ((Action)this).Deadline;
                    default:
                        Action action = ((ActionContainer)this).GetActionsByCalculatedStatus(Status.Active)
                            .Where(a => a.Deadline.HasValue)
                            .OrderBy(a => a.Deadline)
                            .FirstOrDefault();
                        if (action != null)
                        {
                            return action.Deadline;
                        }
                        return null;
                }
            }
        }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal 
        { 
            get
            {
                return CreationDate.ToUserLocalTime(Owner.TimeZoneId);
            }
        }

        public int CompletedPomodorosCount
        {
            get
            {
                return GetPomodoros(PomodoroCalculatedStatus.Completed).Count;
            }
        }

        public string EffortInfo
        {
            get
            {
                return CompletedPomodorosIfEstimate.ToString() + " / " + EstimateWork.ToString();
            }
        }

        public EffortGroup EffortGroup
        {
            get
            {
                if (Effort.HasValue == false)
                {
                    return EffortGroup.Indeterminate;
                }
                if (Effort.Value > 150)
                {
                    return EffortGroup.VeryExceeded;
                }
                if (Effort.Value > 100)
                {
                    return EffortGroup.Exceeded;
                }
                if (Effort.Value > 0)
                {
                    return EffortGroup.InProgress;
                }
                return EffortGroup.Created;
            }
        }

        public PomodoroContainerChartsViewModel ChartsViewModel
        {
            get
            {
                PomodoroContainerChartsViewModel viewModel = new PomodoroContainerChartsViewModel();

                viewModel.WorkAmount = new MorrisLineChartViewModel
                {
                    HeaderTitle = "Work Amount",
                    Dates = GetPomodoros().ToList()
                        .Where(p => p.Start.HasValue && p.CalculatedStatus == PomodoroCalculatedStatus.Completed)
                        .Select(p => p.StartLocal.Value).AsEnumerable(),
                    Interval = DateInterval.Monthly,
                    Label = "Work units",
                    HtmlElementId = "work-amount",
                    LineColor = TypeColor
                };

                viewModel.WorkDivision = new MorrisDonutChartViewModel
                {
                    HeaderTitle = "Work Division",
                    HtmlElementId = "work-division"
                };
                if (this is ProjectContainer)
                {
                    viewModel.WorkDivision.Items = ((ProjectContainer)this).GetProjects();
                }
                else if (this is TaskContainer)
                {
                    viewModel.WorkDivision.Items = ((TaskContainer)this).GetTasks();
                }
                else if (this is ActionContainer)
                {
                    viewModel.WorkDivision.Items = ((ActionContainer)this).GetActions();
                }
                return viewModel;
            }
        }

        public CalendarViewModel CalendarViewModel
        {
            get
            {
                return GetCalendarViewModel();
            }
        }

        public CalendarViewModel GetCalendarViewModel(bool displayX = true, int? height = null)
        {
            CalendarViewModel viewModel = new CalendarViewModel();
            viewModel.WorkHistory = GetPomodoros().Where(p => p.Status == PomodoroStatus.Completed);
            viewModel.Mode = CalendarViewMode.ViewNumber;
            viewModel.ContainerType = Type;
            viewModel.DisplayX = displayX;
            viewModel.Height = height;
            return viewModel;
        }

        public Color RandomColor
        {
            get
            {
                return string.IsNullOrWhiteSpace(ItemName) ? 
                    Color.Black : ItemName.GetRandomColor();
            }
        }

        public String BoxCssClass
        {
            get
            {
                return GetBoxCssClass(Type);
            }
        }

        public static string GetBoxCssClass(PomodoroContainerType type)
        {
            switch (type)
            {
                case PomodoroContainerType.Project:
                    return "box-danger";
                case PomodoroContainerType.Task:
                    return "box-primary";
                case PomodoroContainerType.Action:
                    return "box-success";
            }
            return string.Empty;
        }

        public string StatusIconHtml { get { return CalculatedStatus.GetIconHtmlTag() + InheritedStatusIconHtml; } }

        public string InheritedStatusIconHtml
        {
            get
            {
                if(InheritedStatus)
                {
                    return "<i title=\"Inherited (item status: " + Status.GetAttributeDisplayName() 
                        + ")\" class=\"gt-status gt-inherited fa fa-long-arrow-down\"></i>";
                }
                return string.Empty;
            }
        }

        #region Tags

        private ICollection<Tag> _tags;

        /// <summary>
        /// DB Tags
        /// </summary>
        public virtual ICollection<Tag> Tags
        {
            get
            {
                return _tags ?? (_tags = new HashSet<Tag>());
            }
            set
            {
                _tags = value;
            }
        }

        /// <summary>
        /// Own tags + Inherited tags
        /// </summary>
        public IEnumerable<Tag> OwnAndInheritedTags
        {
            get
            {
                return OwnTags.Concat(InheritedTags);
            }
        }

        /// <summary>
        /// DB Tags excluding inherited tags
        /// </summary>
        public IEnumerable<Tag> OwnTags
        {
            get
            {
                var inheritedTagIDs = InheritedTags.Select(t => t.ID);
                return Tags.Where(t => !inheritedTagIDs.Contains(t.ID));
            }
        }

        /// <summary>
        /// All tags from parents
        /// </summary>
        public IEnumerable<Tag> InheritedTags
        {
            get
            {
                return Parent != null ?
                    Parent.OwnTags.Concat(Parent.InheritedTags) :
                    new List<Tag>();
            }
        }

        /// <summary>
        /// All available user tags excluding own and inherited tags
        /// </summary>
        public IEnumerable<Tag> GetNotSelectedTags(IQueryable<Tag> availableTags)
        {
            var ownTagIDs = OwnTags.Select(t => t.ID);
            var inheritedTagIDs = InheritedTags.Select(t => t.ID);
            return availableTags.Where(t => !ownTagIDs.Contains(t.ID) && !inheritedTagIDs.Contains(t.ID));
        }

        #endregion

    }
        
    /// <summary>
    /// Action Container
    /// </summary>
    public abstract class ActionContainer : PomodoroContainer
    {
        public ICollection<Action> GetActionsByCalculatedStatus(Status status)
        {
            return GetActions().Where(a => a.CalculatedStatus == status).ToList();
        }

        public ICollection<Action> GetActionsByStatus(Status status)
        {
            return GetActions().Where(a => a.Status == status).ToList();
        }

        public abstract ICollection<Action> GetActions();

        public decimal? Progress
        {
            get
            {
                ICollection<Action> completed = GetActionsByCalculatedStatus(Status.Completed);
                ICollection<Action> active = GetActionsByCalculatedStatus(Status.Active);

                if(completed.Count == 0 && active.Count == 0)
                {
                    return null;
                }
                return 100M * ((decimal)completed.Count / (decimal)(completed.Count + active.Count));
            }
        }

        public string ProgressInfo
        {
            get
            {
                ICollection<Action> completed = GetActionsByCalculatedStatus(Status.Completed);
                ICollection<Action> active = GetActionsByCalculatedStatus(Status.Active);

                return completed.Count.ToString() + " / " + (completed.Count + active.Count).ToString();
            }
        }


        public ProgressGroup ProgressGroup
        {
            get
            {
                if (Progress.HasValue == false || Progress.Value == 0)
                {
                    return ProgressGroup.Created;
                }
                if (Progress.Value >= 100)
                {
                    return ProgressGroup.Completed;
                }
                return ProgressGroup.InProgress;
            }
        }

        public override decimal? Effort
        {
            get
            {
                if (EstimateWork > 0)
                {
                    return 100M * ((decimal)CompletedPomodorosIfEstimate / (decimal)EstimateWork);
                }
                return null;
            }
        }

        public override int EstimateWork
        {
            get
            {
                //return GetActions()
                //    .Where(a => a.Estimate.HasValue && a.Estimate.Value > 0)
                //    .Sum(a => a.Estimate.Value);
                return GetActions().Sum(a => a.EstimateWork);
            }
        }

        public override int CompletedPomodorosIfEstimate
        {
            get
            {
                return GetActions()
                    .Where(a => a.Estimate.HasValue)
                    .Sum(a => a.CompletedPomodorosCount);
            }
        }
        
        public override bool ContainsSelectedAction
        {
            get
            {
                if (Owner != null)
                {
                    return GetActions().Where(a => a.ID == Owner.ActionID).Count() > 0;
                }
                return false;
            }
        }

        public static bool IsFinishedStatus(Status status)
        {
            return status == Status.Completed || status == Status.Cancelled;
        }

        public override bool IsFinished
        {
            get { return IsFinishedStatus(Status); }
        }
    }

    /// <summary>
    /// Task Container
    /// </summary>
    public abstract class TaskContainer : ActionContainer
    {

        public ICollection<Task> GetTasksByCalculatedStatus(Status status)
        {
            return GetTasks().Where(t => t.CalculatedStatus == status).ToList();
        }

        public ICollection<Task> GetTasksByStatus(Status status)
        {
            return GetTasks().Where(t => t.Status == status).ToList();
        }

        public abstract ICollection<Task> GetTasks();

    }

    /// <summary>
    /// Project Container
    /// </summary>
    public abstract class ProjectContainer : TaskContainer
    {

        public ICollection<Project> GetProjectByCalculatedStatus(Status status)
        {
            return GetProjects().Where(p => p.CalculatedStatus == status).ToList();
        }

        public ICollection<Project> GetProjectsByStatus(Status status)
        {
            return GetProjects().Where(p => p.Status == status).ToList();
        }

        public abstract ICollection<Project> GetProjects();

    }
    
}
