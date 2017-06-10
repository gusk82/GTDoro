using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using Task = GTDoro.Core.Models.Task;
using Microsoft.AspNet.Identity;
using GTDoro.Core.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;

namespace GTDoro.Core.DAL
{
    public interface IGTDoroContext
    {
        DbSet<Sprint> Sprints { get; set; }
        DbSet<TimePeriod> TimePeriods { get; set; }
        DbSet<Pomodoro> Pomodoros { get; set; }
        DbSet<Action> Actions { get; set; }
        DbSet<Activity> Activities { get; set; }
        DbSet<Task> Tasks { get; set; }
        DbSet<Project> Projects { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<CollectedThing> CollectedThings { get; set; }
        DbSet<KeyValueParameter> KeyValueParameters { get; set; }

        IQueryable<TimePeriod> GetMyTimePeriods(IPrincipal User);
        TimePeriod GetTimePeriodById(IPrincipal User, int Id);
        IQueryable<Activity> GetMyActivities(IPrincipal User);
        Activity GetActivityById(IPrincipal User, int Id);
        IQueryable<Task> GetMyTasks(IPrincipal User);
        Task GetTaskById(IPrincipal User, int Id);
        IQueryable<Project> GetMyProjects(IPrincipal User);
        Project GetProjectById(IPrincipal User, int Id);

        Task<IdentityResult> RegisterUser(ApplicationUser userModel, string password);
        Task<IdentityUser> FindUser(string userName, string password);

        void SetModified(object entity);
        void SetDeleted(object entity);
        int SaveChanges();

    }
}
