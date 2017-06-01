using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Action Container
    /// </summary>
    public abstract class ActionContainer : PomodoroContainer
    {
        public ICollection<Action> GetActionsByCalculatedStatus(Status status)
        {
            return GetActions().Where(a => a.CalculatedStatus == status).ToList();
        }

        public ICollection<Action> GetActionsByStatus(Status status)
        {
            return GetActions().Where(a => a.Status == status).ToList();
        }

        public abstract ICollection<Action> GetActions();

        public decimal? Progress
        {
            get
            {
                ICollection<Action> completed = GetActionsByCalculatedStatus(Status.Completed);
                ICollection<Action> active = GetActionsByCalculatedStatus(Status.Active);

                if (completed.Count == 0 && active.Count == 0)
                {
                    return null;
                }
                return 100M * ((decimal)completed.Count / (decimal)(completed.Count + active.Count));
            }
        }

        public string ProgressInfo
        {
            get
            {
                ICollection<Action> completed = GetActionsByCalculatedStatus(Status.Completed);
                ICollection<Action> active = GetActionsByCalculatedStatus(Status.Active);

                return completed.Count.ToString() + " / " + (completed.Count + active.Count).ToString();
            }
        }

        public ProgressGroup ProgressGroup
        {
            get
            {
                if (Progress.HasValue == false || Progress.Value == 0)
                {
                    return ProgressGroup.Created;
                }
                if (Progress.Value >= 100)
                {
                    return ProgressGroup.Completed;
                }
                return ProgressGroup.InProgress;
            }
        }

        public override decimal? Effort
        {
            get
            {
                if (EstimateWork > 0)
                {
                    return 100M * ((decimal)CompletedPomodorosIfEstimate / (decimal)EstimateWork);
                }
                return null;
            }
        }

        public override int EstimateWork
        {
            get
            {
                //return GetActions()
                //    .Where(a => a.Estimate.HasValue && a.Estimate.Value > 0)
                //    .Sum(a => a.Estimate.Value);
                return GetActions().Sum(a => a.EstimateWork);
            }
        }

        public override int CompletedPomodorosIfEstimate
        {
            get
            {
                return GetActions()
                    .Where(a => a.Estimate.HasValue)
                    .Sum(a => a.CompletedPomodorosCount);
            }
        }

        public override bool ContainsSelectedAction
        {
            get
            {
                if (Owner != null)
                {
                    return GetActions().Where(a => a.ID == Owner.ActionID).Count() > 0;
                }
                return false;
            }
        }

        public static bool IsFinishedStatus(Status status)
        {
            return status == Status.Completed || status == Status.Cancelled;
        }

        public override bool IsFinished
        {
            get { return IsFinishedStatus(Status); }
        }
    }
}
