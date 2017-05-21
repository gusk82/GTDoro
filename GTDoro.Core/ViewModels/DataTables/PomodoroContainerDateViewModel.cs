using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class PomodoroContainerDateViewModel : DateItemViewModel
    {
        public PomodoroContainerType Type { get; set; }
        public IEnumerable<PomodoroContainer> Items { get; set; }

        public ReportTypeDate ReportTypeDate { get; set; }
        public DateTime? GetDate(PomodoroContainer pc)
        {
            switch (ReportTypeDate)
            {
                case ReportTypeDate.NextDeadline:
                    return pc.NextDeadline;
                case ReportTypeDate.NextPlanifiedPomodoro:
                    return (pc.NextPlanifiedPomodoro != null) ? pc.NextPlanifiedPomodoro.Start : null;
                case ReportTypeDate.LastCreationDate:
                    return (pc.CreationDate ?? null);
                default:
                    return (pc.LastPomodoro != null) ? pc.LastPomodoro.Start : null;
            }
        }
    }
}