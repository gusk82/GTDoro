using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GTDoro.Core.Models;
using Task = GTDoro.Core.Models.Task;
using Action = GTDoro.Core.Models.Action;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using GTDoro.Core.Models.Identity;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Security.Claims;

namespace GTDoro.Core.DAL
{
    public class GTDoroContext : IdentityDbContext<ApplicationUser>, IGTDoroContext
    {
        public UserManager<ApplicationUser> manager { get; private set; }

        public GTDoroContext()
            : base("GTDoroContext", throwIfV1Schema: false)
        {
            manager =  new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this));
        }

        static GTDoroContext()
        {
            //Database.SetInitializer<GTDoroContext>(new GTDoroInitializer());
            //Database.SetInitializer<GTDoroContext>(new MigrateDatabaseToLatestVersion<GTDoroContext, GTDoroConfiguration>());
        }

        public static GTDoroContext Create()
        {
            return new GTDoroContext();
        }

        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<TimePeriod> TimePeriods { get; set; }
        public DbSet<Pomodoro> Pomodoros { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CollectedThing> CollectedThings { get; set; }
        public DbSet<KeyValueParameter> KeyValueParameters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Project>().
                  HasMany(p => p.Tags).
                  WithMany(c => c.Projects).
                  Map(
                   m =>
                   {
                       m.MapLeftKey("ProjectID");
                       m.MapRightKey("TagID");
                       m.ToTable("ProjectTags");
                   });

            modelBuilder.Entity<Task>().
                  HasMany(t => t.Tags).
                  WithMany(c => c.Tasks).
                  Map(
                   m =>
                   {
                       m.MapLeftKey("TaskID");
                       m.MapRightKey("TagID");
                       m.ToTable("TaskTags");
                   });

            modelBuilder.Entity<Action>().
                  HasMany(t => t.Tags).
                  WithMany(c => c.Actions).
                  Map(
                   m =>
                   {
                       m.MapLeftKey("ActionID");
                       m.MapRightKey("TagID");
                       m.ToTable("ActionTags");
                   });

            modelBuilder.Entity<Activity>().
                  HasMany(t => t.Tags).
                  WithMany(c => c.Activities).
                  Map(
                   m =>
                   {
                       m.MapLeftKey("ActivityID");
                       m.MapRightKey("TagID");
                       m.ToTable("ActivityTags");
                   });

            modelBuilder.Entity<Sprint>().
                  HasMany(t => t.Tags).
                  WithMany(c => c.Sprints).
                  Map(
                   m =>
                   {
                       m.MapLeftKey("SprintID");
                       m.MapRightKey("TagID");
                       m.ToTable("SprintTags");
                   });

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }

        public Workspace GetMyWorkspace(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return null;
            }
            return new Workspace(currentUser);
        }

        public IQueryable<Project> GetMyProjects(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Project>().AsQueryable();
            }
            return Projects.Where(p => p.User.Id == currentUser.Id);
        }

        public Project GetProjectById(IPrincipal User, int Id)
        {
            return GetMyProjects(User).SingleOrDefault(p => p.ID == Id);
        }

        public IQueryable<Task> GetMyTasks(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Task>().AsQueryable();
            }
            return Tasks.Where(t => t.Project.User.Id == currentUser.Id);
        }

        public Task GetTaskById(IPrincipal User, int Id)
        {
            return GetMyTasks(User).SingleOrDefault(t => t.ID == Id);
        }

        public IQueryable<Action> GetMyActions(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Action>().AsQueryable();
            }
            return Actions.Where(a => a.Task.Project.User.Id == currentUser.Id);
        }

        public Action GetActionById(IPrincipal User, int Id)
        {
            return GetMyActions(User).SingleOrDefault(a => a.ID == Id);
        }

        public IQueryable<Activity> GetMyActivities(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Activity>().AsQueryable();
            }
            return Activities.Where(a => a.Task.Project.User.Id == currentUser.Id);
        }

        public Activity GetActivityById(IPrincipal User, int Id)
        {
            return GetMyActivities(User).SingleOrDefault(a => a.ID == Id);
        }

        public IQueryable<Pomodoro> GetMyPomodoros(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Pomodoro>().AsQueryable();
            }
            return Pomodoros.Where(p => p.Action.Task.Project.User.Id == currentUser.Id);
        }

        public Pomodoro GetPomodoroById(IPrincipal User, int Id)
        {
            return GetMyPomodoros(User).SingleOrDefault(p => p.ID == Id);
        }

        public IQueryable<TimePeriod> GetMyTimePeriods(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<TimePeriod>().AsQueryable();
            }
            return TimePeriods.Where(tp => tp.Activity.Task.Project.User.Id == currentUser.Id);
        }

        public TimePeriod GetTimePeriodById(IPrincipal User, int Id)
        {
            return GetMyTimePeriods(User).SingleOrDefault(tp => tp.ID == Id);
        }

        public IQueryable<Tag> GetMyTags(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Tag>().AsQueryable();
            }
            return Tags.Where(c => c.User.Id == currentUser.Id || c.User == null);
        }

        public Tag GetTagById(IPrincipal User, int Id)
        {
            return GetMyTags(User).SingleOrDefault(c => c.ID == Id);
        }

        public IQueryable<CollectedThing> GetMyCollectedThings(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<CollectedThing>().AsQueryable();
            }
            return CollectedThings.Where(ct => ct.User.Id == currentUser.Id);
        }

        public CollectedThing GetCollectedThingById(IPrincipal User, int Id)
        {
            return GetMyCollectedThings(User).SingleOrDefault(ct => ct.ID == Id);
        }

        public IQueryable<Sprint> GetMySprints(IPrincipal User)
        {
            ApplicationUser currentUser = GetUser(User.Identity);
            if (currentUser == null)
            {
                return Enumerable.Empty<Sprint>().AsQueryable();
            }
            return Sprints.Where(s => s.Project.User.Id == currentUser.Id);
        }

        public Sprint GetSprintById(IPrincipal User, int Id)
        {
            return GetMySprints(User).SingleOrDefault(s => s.ID == Id);
        }

        private ApplicationUser GetUser(IIdentity identity)
        {
            var user = manager.FindById(identity.GetUserId());

            if(user == null)
            {
                var claimsIdentity = identity as ClaimsIdentity;
                Claim identityClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "sub");
                if(identityClaim != null)
                {
                    user = Users.FirstOrDefault(u => u.Email == identityClaim.Value);
                }
            }
            return user;
        }

        #region Key Value Parameters Get/Set

        public KeyValueParameter GetParameterById(int Id)
        {
            return KeyValueParameters.Where(kvp => kvp.ID == Id).FirstOrDefault();
        }

        public string GetParameterValueById(int Id)
        {
            KeyValueParameter kvp = GetParameterById(Id);
            if (kvp != null)
            {
                return kvp.Value;
            }
            return string.Empty;
        }

        public KeyValueParameter GetParameterByCode(string Code)
        {
            return KeyValueParameters.Where(kvp => kvp.Code.Trim() == Code.Trim()).FirstOrDefault();
        }

        public KeyValueParameter GetParameterByCode(ParameterType Parameter)
        {
            return GetParameterByCode(Parameter.ToString());
        }

        public string GetParameterValueByCode(string Code)
        {
            KeyValueParameter kvp = GetParameterByCode(Code);
            if (kvp != null)
            {
                return kvp.Value;
            }
            return string.Empty;
        }

        public string GetParameterValueByCode(ParameterType Parameter)
        {
            return GetParameterValueByCode(Parameter.ToString());
        }

        public void SetParameterValue(string Code, string Value)
        {
            KeyValueParameter kvp = GetParameterByCode(Code);
            if (kvp == null)
            {
                kvp = new KeyValueParameter() { Code = Code };
                KeyValueParameters.Add(kvp);
            }
            else
            {
                Entry(kvp).State = EntityState.Modified;
            }
            kvp.Value = Value;
            SaveChanges();
        }

        public void SetParameterValue(ParameterType Parameter, string Value)
        {
            SetParameterValue(Parameter.ToString(), Value);
        }
        #endregion

        public async Task<IdentityResult> RegisterUser(ApplicationUser userModel, string password)
        {
            var result = await manager.CreateAsync(userModel, password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await manager.FindAsync(userName, password);

            return user;
        }

        private void ChangeState(object entity, EntityState state)
        {
            Entry(entity).State = state;
        }

        public void SetModified(object entity)
        {
            ChangeState(entity, EntityState.Modified);
        }

        public void SetDeleted(object entity)
        {
            ChangeState(entity, EntityState.Deleted);
        }
    }
}