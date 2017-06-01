using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Task Container
    /// </summary>
    public abstract class TaskContainer : WorkableItemContainer
    {

        public ICollection<Task> GetTasksByCalculatedStatus(Status status)
        {
            return GetTasks().Where(t => t.CalculatedStatus == status).ToList();
        }

        public ICollection<Task> GetTasksByStatus(Status status)
        {
            return GetTasks().Where(t => t.Status == status).ToList();
        }

        public abstract ICollection<Task> GetTasks();

    }
}
