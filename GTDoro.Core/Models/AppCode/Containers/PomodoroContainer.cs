using GTDoro.Core.Extensions;
using GTDoro.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Pomodoro Container
    /// </summary>
    public abstract class PomodoroContainer : LoggableItemContainer
    {
        public abstract int EstimateWork { get; }
        public abstract int CompletedPomodorosIfEstimate { get; }
        public abstract PomodoroContainerType Type { get; }
        public abstract bool ContainsSelectedAction { get; }
        public abstract PomodoroContainer NextSibling { get; }

        public abstract ICollection<Pomodoro> GetPomodoros();

        public Pomodoro FirstPomodoro
        {
            get
            {
                return GetPomodoros(LoggableItemCalculatedStatus.Completed)
                    .OrderBy(p => p.Start).FirstOrDefault();
            }
        }

        public Pomodoro LastPomodoro
        {
            get
            {
                return GetPomodoros(LoggableItemCalculatedStatus.Completed)
                   .OrderByDescending(p => p.Start).FirstOrDefault();
            }
        }

        public ICollection<Pomodoro> GetPomodoros(LoggableItemCalculatedStatus status, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            if (!dateFrom.HasValue)
            {
                dateFrom = DateTime.MinValue;
            }
            if (!dateTo.HasValue)
            {
                dateTo = DateTime.MaxValue;
            }
            return GetPomodoros().Where(p => p.CalculatedStatus == status &&
                p.Start.HasValue).AsEnumerable().Where(p =>
                    p.StartLocal.Value >= dateFrom.Value &&
                    p.StartLocal.Value <= dateTo.Value).ToList();
        }

        public ICollection<Pomodoro> GetPomodoros(PomodoroStatus status, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            if (!dateFrom.HasValue)
            {
                dateFrom = DateTime.MinValue;
            }
            if (!dateTo.HasValue)
            {
                dateTo = DateTime.MaxValue;
            }
            return GetPomodoros().Where(p => p.Status == status &&
                p.Start.HasValue).AsEnumerable().Where(p =>
                    p.StartLocal.Value >= dateFrom.Value &&
                    p.StartLocal.Value <= dateTo.Value).ToList();
        }

        public ICollection<PomodoroSet> PomodoroSets
        {
            get
            {
                return PomodoroSet.GetPomodoroSets(
                    GetPomodoros(), PomodoroContainerType.Action, GroupDifferentDates)
                    .OrderByDescending(s => s.StartLocal).ToList();
            }
        }

        public bool HasPlanifiedWork
        {
            get
            {
                return GetPomodoros(LoggableItemCalculatedStatus.Planified).Count > 0;
            }
        }

        public Pomodoro NextPlanifiedPomodoro
        {
            get
            {
                return GetPomodoros(LoggableItemCalculatedStatus.Planified)
                    .Where(pm => pm.Start.HasValue)
                    .OrderBy(pm => pm.Start.Value)
                    .FirstOrDefault();
            }
        }

        public DateTime? NextDeadline
        {
            get
            {
                switch (Type)
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

        public int CompletedPomodorosCount
        {
            get
            {
                return GetPomodoros(LoggableItemCalculatedStatus.Completed).Count;
            }
        }

        public string EffortInfo
        {
            get
            {
                return CompletedPomodorosIfEstimate.ToString() + " / " + EstimateWork.ToString();
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
                        .Where(p => p.Start.HasValue && p.CalculatedStatus == LoggableItemCalculatedStatus.Completed)
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
    }
}
