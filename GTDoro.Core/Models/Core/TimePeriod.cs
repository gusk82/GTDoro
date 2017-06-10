using GTDoro.Core.Extensions;
using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    public class TimePeriod
    {
        public int ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? Start { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? End { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public TimeSpan Break { get; set; }

        public PomodoroStatus Status { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDate { get; set; }

        [Required]
        public int ActivityID { get; set; }
        public virtual Activity Activity { get; set; }

        [NotMapped]
        [Display(Name = "Status")]
        public LoggableItemCalculatedStatus CalculatedStatus
        {
            get
            {
                switch (Status)
                {
                    case PomodoroStatus.Working:
                        return LoggableItemCalculatedStatus.Working;
                    case PomodoroStatus.Cancelled:
                        return LoggableItemCalculatedStatus.Cancelled;
                    case PomodoroStatus.Completed:
                        return LoggableItemCalculatedStatus.Completed;
                    case PomodoroStatus.Unconfirmed:
                        return LoggableItemCalculatedStatus.Unconfirmed;
                    case PomodoroStatus.Planified:
                        if (Start.HasValue && Start.Value < DateTime.UtcNow.ToUserLocalTime(Owner.TimeZoneId))
                        {
                            return LoggableItemCalculatedStatus.Expired;
                        }
                        break;
                }
                return LoggableItemCalculatedStatus.Planified;
            }
        }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? StartLocal { get { return Start.ToUserLocalTime(Owner.TimeZoneId); } }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm}")]
        public DateTime? EndLocal { get { return End.ToUserLocalTime(Owner.TimeZoneId); } }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal { get { return CreationDate.ToUserLocalTime(Owner.TimeZoneId); } }

        public ApplicationUser Owner { get { return Activity.Owner; } }

        public TimeSpan TimeLogged
        {
            get
            {
                DateTime end = (End.HasValue ? End.Value : DateTime.UtcNow);
                if (!Start.HasValue || Start.Value >= end)
                {
                    return TimeSpan.Zero;
                }
                return (end - Start.Value) - Break;
            }
        }

        public decimal MonetaryValue
        {
            get
            {
                if(!Activity.HourlyRate.HasValue)
                {
                    return decimal.Zero;
                }
                double dbValue = TimeLogged.TotalHours * (double)Activity.HourlyRate.Value;
                decimal dcValue = (decimal)Math.Round(dbValue, 2);
                return Math.Max(dcValue, decimal.Zero);
            }
        }
    }
}
