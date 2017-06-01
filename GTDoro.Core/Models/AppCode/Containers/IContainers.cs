using GTDoro.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace GTDoro.Core.Models
{

    public interface IActivityContainer : ITimePeriodContainer
    {
        ICollection<Activity> GetActivitiesByCalculatedStatus(Status status);

        ICollection<Activity> GetActivitiesByStatus(Status status);

        ICollection<Activity> GetActivities();
    }

    public interface ITimePeriodContainer
    {
        ICollection<TimePeriod> GetTimePeriods(LoggableItemCalculatedStatus status, DateTime? DateFrom = null, DateTime? DateTo = null);
     

        ICollection<TimePeriod> GetTimePeriods();

        TimePeriod FirstTimePeriod { get; }

        TimePeriod LastTimePeriod { get; }

        //PomodoroContainerChartsViewModel ChartsViewModel { get; }

    }
}