using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Project Container
    /// </summary>
    public abstract class ProjectContainer : TaskContainer
    {

        public ICollection<Project> GetProjectByCalculatedStatus(Status status)
        {
            return GetProjects().Where(p => p.CalculatedStatus == status).ToList();
        }

        public ICollection<Project> GetProjectsByStatus(Status status)
        {
            return GetProjects().Where(p => p.Status == status).ToList();
        }

        public abstract ICollection<Project> GetProjects();

    }
}
