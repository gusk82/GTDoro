using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class WorkingPanelViewModel
    {
        public Pomodoro pomodoro { get; set; }
        public Action action { get; set; }
        public DateTime? TargetTime
        {
            get
            {
                if (pomodoro != null && pomodoro.Status == PomodoroStatus.Working)
                {
                    //get the target time to complete the work
                    return pomodoro.Start.Value.AddMinutes(Settings.POMOWORK);
                }
                return null;
            }
        }
        public DateTime? BreakTargetTime
        {
            get
            {
                if (pomodoro != null && pomodoro.Status == PomodoroStatus.Unconfirmed)
                {
                    //get the target time to complete the break
                    return pomodoro.Start.Value.AddMinutes(Settings.POMOCYCLE);
                }
                return null;
            }
        }
        public WorkingStatus Status
        {
            get
            {
                if (pomodoro != null)
                {
                    switch (pomodoro.Status)
                    {
                        case PomodoroStatus.Working:
                            return WorkingStatus.Working;
                        case PomodoroStatus.Unconfirmed:
                            if (pomodoro.Start.Value.AddMinutes(Settings.POMOCYCLE) > DateTime.UtcNow)
                            {
                                return WorkingStatus.BreakTime;
                            }
                            return WorkingStatus.PendingConfirmation;
                    }
                }
                else
                {
                    return (action != null) ? 
                        WorkingStatus.ActionSelected : WorkingStatus.NoActionSelected;
                }
                return WorkingStatus.Inconsistent;
            }
        }
    }
}