using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using GTDoro.Core.Extensions;
using GTDoro.Core.Models.Identity;

namespace GTDoro.Core.Models
{
    public class Pomodoro
    {
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? Start { get; set; }
        public PomodoroStatus Status { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDate { get; set; }

        [Required]
        public int ActionID { get; set; }
        public virtual Action Action { get; set; }

        [NotMapped]
        [Display(Name = "Status")]
        public PomodoroCalculatedStatus CalculatedStatus
        {
            get
            {
                switch (Status)
                {
                    case PomodoroStatus.Working:
                        return PomodoroCalculatedStatus.Working;
                    case PomodoroStatus.Cancelled:
                        return PomodoroCalculatedStatus.Cancelled;
                    case PomodoroStatus.Completed:
                        return PomodoroCalculatedStatus.Completed;
                    case PomodoroStatus.Unconfirmed:
                        return PomodoroCalculatedStatus.Unconfirmed;
                    case PomodoroStatus.Planified:
                        if (Start.HasValue && Start.Value < DateTime.UtcNow.ToUserLocalTime(Owner.TimeZoneId))
                        {
                            return PomodoroCalculatedStatus.Expired;
                        }
                        break;
                }
                return PomodoroCalculatedStatus.Planified;
            }
        }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? StartLocal { get { return Start.ToUserLocalTime(Owner.TimeZoneId); } }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal { get { return CreationDate.ToUserLocalTime(Owner.TimeZoneId); } }

        public ApplicationUser Owner { get { return Action.Owner;  } }
    }
}