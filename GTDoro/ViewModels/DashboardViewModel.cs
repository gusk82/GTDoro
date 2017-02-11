using GTDoro.Models;
using Action = GTDoro.Models.Action;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    [NotMapped]
    public class DashboardViewModel
    {
        public Workspace MyWorkspace { get; set; }

        public IEnumerable<Project> ActiveProjects { get; set; }
        public IEnumerable<Task> ActiveTasks { get; set; }
        public IEnumerable<Action> ActiveActions { get; set; }
        public IEnumerable<Pomodoro> CompletedPomodoros { get; set; }

        public ICollection<LineChartValue> WorkAmount;
        public ICollection<DonutChartValue> ProjectWorkDivision;
        public ICollection<DonutChartValue> TaskWorkDivision;
        public ICollection<DonutChartValue> ActionWorkDivision;

        public DateItemViewModel LastActiveTasks { get; set; }
        public DateItemViewModel LastActiveActions { get; set; }
        public DateItemViewModel UpcomingDeadlines { get; set; }
        //public DateItemViewModel UpcomingPlanifiedActions { get; set; }
        public DateItemViewModel LastCreatedActions { get; set; }

        public MorrisLineChartViewModel WorkAmountChartViewModel { get; set; }
        public MorrisDonutChartViewModel WorkDivisionViewModel { get; set; }
        //public IEnumerable<MorrisDonutChartViewModel> WorkDivisionViewModels { get; set; }
    }    
}