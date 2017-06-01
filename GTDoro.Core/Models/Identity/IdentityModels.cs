using GTDoro.Core.Extensions;
using GTDoro.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace GTDoro.Core.Models.Identity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MinLength(6)]
        [StringLength(50)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        
        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime? CreationDate { get; set; }

        [Display(Name = "Selected Action")]
        public int? ActionID { get; set; }
        public virtual Action Action { get; set; }
        
        public virtual ICollection<Project> Projects { get; set; }

        private Workspace workspace;
        public Workspace Workspace
        {
            get
            {
                if(workspace == null)
                {
                    workspace = new Workspace(this);
                }
                return workspace;
            }
        }
        [NotMapped]
        public ICollection<Task> Tasks { get { return Workspace.GetTasks(); } }
        [NotMapped]
        public ICollection<Action> Actions { get { return Workspace.GetActions(); } }
        [NotMapped]
        public ICollection<Activity> Activities { get { return Workspace.GetActivities(); } }
        [NotMapped]
        public ICollection<TimePeriod> TimePeriods { get { return Workspace.GetTimePeriods(); } }
        [NotMapped]
        public ICollection<Pomodoro> Pomodoros { get { return Workspace.GetPomodoros(); } }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal { get { return CreationDate.ToUserLocalTime(TimeZoneId); } }

        [NotMapped]
        public int TimesetOffsetMinutes = 120;

        public async System.Threading.Tasks.Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public bool WorkingPanelAvailable
        {
            get
            {
                if (Action != null)
                {
                    return Action.GetPomodoros(PomodoroStatus.Working).Count == 0 &&
                        Action.GetPomodoros(PomodoroStatus.Unconfirmed).Count == 0;
                }
                return true;
            }
        }

        public async System.Threading.Tasks.Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager, bool isPersistent)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.SetIsPersistent(isPersistent);
            return userIdentity;
        }
    }

}