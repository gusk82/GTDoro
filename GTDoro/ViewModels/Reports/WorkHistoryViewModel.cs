using GTDoro.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels.Reports
{
    public class WorkHistoryViewModel : ReportViewModel
    {
        //public IPagedList<Pomodoro> WorkHistory { get; set; }
        public ICollection<Pomodoro> WorkHistory { get; set; }

        private ICollection<PomodoroSet> _setWorkHistory { get; set; }
        public ICollection<PomodoroSet> setWorkHistory
        {
            get
            {
                if (_setWorkHistory == null)
                {
                    _setWorkHistory = PomodoroSet.GetPomodoroSets(WorkHistory);
                }
                return _setWorkHistory;
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
                    Dates = WorkHistory.ToList()
                        .Where(p => p.Start.HasValue && p.CalculatedStatus == PomodoroCalculatedStatus.Completed)
                        .Select(p => p.StartLocal.Value).AsEnumerable(),
                    Interval = DateInterval.Monthly,
                    Label = "Work units",
                    HtmlElementId = "work-amount"
                };

                viewModel.MultipleWorkDivision = new List<MorrisDonutChartViewModel> { 
                    //work project division (donut chart)
                    new MorrisDonutChartViewModel
                    {
                        HeaderTitle = "Projects",
                        Items = WorkHistory.GroupBy(p => p.Action.Task.ProjectID).Select(group => group.First().Action.Task.Project),
                        HtmlElementId = "project-chart",
                        From = Start,
                        To = End
                    },
                    //work task division (donut chart)
                     new MorrisDonutChartViewModel
                    {
                        HeaderTitle = "Tasks",
                        Items = WorkHistory.GroupBy(p => p.Action.TaskID).Select(group => group.First().Action.Task),
                        HtmlElementId = "task-chart",
                        UseFullPathItemName = true,
                        From = Start,
                        To = End
                    }
                };

                return viewModel;
            }
        }

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
            viewModel.WorkHistory = WorkHistory.Where(p => p.Status == PomodoroStatus.Completed);
            viewModel.Mode = CalendarViewMode.ViewNumber;
            viewModel.ContainerType = PomodoroContainerType.Workspace;
            viewModel.DisplayX = displayX;
            viewModel.Height = height;
            return viewModel;
        }
    }
}