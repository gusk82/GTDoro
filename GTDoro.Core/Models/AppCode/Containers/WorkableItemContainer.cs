using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    public abstract class WorkableItemContainer : ActionContainer, IActivityContainer
    {
        public TimePeriod FirstTimePeriod
        {
            get
            {
                return GetTimePeriods(LoggableItemCalculatedStatus.Completed)
                    .OrderBy(tp => tp.Start).FirstOrDefault();
            }
        }
            

        public TimePeriod LastTimePeriod => throw new NotImplementedException();

        public abstract ICollection<Activity> GetActivities();
        
        public ICollection<Activity> GetActivitiesByCalculatedStatus(Status status)
        {
            return GetActivities().Where(a => a.CalculatedStatus == status).ToList();
        }

        public ICollection<Activity> GetActivitiesByStatus(Status status)
        {
            return GetActivities().Where(a => a.Status == status).ToList();
        }

        public ICollection<TimePeriod> GetTimePeriods(LoggableItemCalculatedStatus status, DateTime? dateFrom = default(DateTime?), DateTime? dateTo = default(DateTime?))
        {
            if (!dateFrom.HasValue)
            {
                dateFrom = DateTime.MinValue;
            }
            if (!dateTo.HasValue)
            {
                dateTo = DateTime.MaxValue;
            }
            return GetTimePeriods().Where(tp => tp.CalculatedStatus == status &&
                tp.Start.HasValue).AsEnumerable().Where(tp =>
                    tp.StartLocal.Value >= dateFrom.Value &&
                    tp.StartLocal.Value <= dateTo.Value).ToList();
        }

        public ICollection<TimePeriod> GetTimePeriods()
        {
            List<TimePeriod> lstTimePeriods = new List<TimePeriod>();
            ICollection<Activity> Activities = GetActivities();
            if (Activities != null)
            {
                foreach (Activity a in Activities)
                {
                    lstTimePeriods.AddRange(a.TimePeriods);
                }
            }
            return lstTimePeriods;
        }
    }
}
