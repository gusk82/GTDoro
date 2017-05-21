using GTDoro.Core.Models;
using System;
using System.Collections.Generic;

namespace GTDoro.Core.Exception

{
    public class CalendarException : System.Exception
    {
        public CalendarWarning Warning { get; protected set; }

        public CalendarException(CalendarWarning warning, string message)
            : base(message)
        {
            Warning = warning;
        }
    }

    public class ConflictCalendarException : CalendarException
    {
        public List<Pomodoro> ExistingPomodoros { get; protected set; }

        public ConflictCalendarException(CalendarWarning warning, List<Pomodoro> existingPomodoros)
            : base(warning, "Conflict found between proposed interval and existing pomodoros.")
        {
            ExistingPomodoros = existingPomodoros;
        }
    }
}