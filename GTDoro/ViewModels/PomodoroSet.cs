using GTDoro.Models;
using Action = GTDoro.Models.Action;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    [NotMapped]
    public class PomodoroSet : List<Pomodoro>
    {
        private Pomodoro FirstItem
        {
            get
            {
                return this.OrderBy(p => p.StartLocal).FirstOrDefault();
            }
        }
        public bool IsEmpty { get { return Count == 0; } }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? StartLocal { get { return !IsEmpty ? FirstItem.StartLocal : null; } }
        public PomodoroStatus Status { get { return !IsEmpty ? FirstItem.Status : PomodoroStatus.Planified; } }
        public Action Action { get { return !IsEmpty ? FirstItem.Action : null; } }

        public new void Add(Pomodoro item)
        {
            if (!IsEmpty && item.Status != Status)
            {
                throw new ArgumentException("This set has been created with a different status");
            }
            base.Add(item);
        }

        public static ICollection<PomodoroSet> GetPomodoroSets(IEnumerable<Pomodoro> pomodoros,
             PomodoroContainerType containerType = PomodoroContainerType.Action, bool GroupDifferentDates = false)
        {
            List<PomodoroSet> list = new List<PomodoroSet>();
            if (pomodoros.ToList().Count == 0 || containerType == PomodoroContainerType.Unspecified)
            {
                return list;
            }

            PomodoroSet set = new PomodoroSet();
            Pomodoro last = null;
            bool sameContainer = false;
            foreach (Pomodoro item in pomodoros.OrderBy(p => p.StartLocal))
            {
                if (last != null)
                {
                    switch (containerType)
                    {
                        case PomodoroContainerType.Project:
                            sameContainer = last.Action.Task.ProjectID == item.Action.Task.ProjectID;
                            break;
                        case PomodoroContainerType.Task:
                            sameContainer = last.Action.TaskID == item.Action.TaskID;
                            break;
                        case PomodoroContainerType.Action:
                            sameContainer = last.ActionID == item.ActionID;
                            break;
                        case PomodoroContainerType.Sprint:
                            sameContainer = last.Action.Task.ProjectID == item.Action.Task.ProjectID;
                            break;
                        case PomodoroContainerType.Workspace:
                            sameContainer = true;
                            break;
                    }
                }
                if (last == null || (sameContainer && last.Status == item.Status) &&
                    (GroupDifferentDates || (last.Start.HasValue && item.Start.HasValue && last.StartLocal.Value.Date == item.StartLocal.Value.Date)))
                {
                    set.Add(item);
                }
                else
                {
                    list.Add(set);
                    set = new PomodoroSet();
                    set.Add(item);
                }
                last = item;
            }
            list.Add(set);
            return list;
        }
    }
}