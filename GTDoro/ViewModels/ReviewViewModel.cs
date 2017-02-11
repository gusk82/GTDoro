using GTDoro.Models;
using Action = GTDoro.Models.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    public class ReviewViewModel
    {
        public CollectedThingDateViewModel CollectedThings { get; set; }
        public PomodoroContainerDateViewModel ExpiredActions { get; set; }
        public PomodoroContainerDateViewModel InactiveActions { get; set; }
        public PomodoroContainerDateViewModel TaskWithNoActiveActions { get; set; }
        public PomodoroContainerDateViewModel ProjectsWithNoActiveTasks { get; set; }
    }
}