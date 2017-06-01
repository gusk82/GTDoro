using GTDoro.Core.Models;
using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Drawing;

namespace GTDoro.Core.Models
{
    public class Project : TaskContainer
    {
        public const int CODE_MAX_LENGTH = 14;

        public Project()
        {
            Tasks = new HashSet<Task>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [StringLength(CODE_MAX_LENGTH, ErrorMessage = "Code cannot be longer than 14 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(Settings.PTA_NAME_MAX_LENGTH, ErrorMessage = Settings.VAL_ERR_NAME_MAX_MSG)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(Settings.PTA_DESCRIPTION_MAX_LENGTH, ErrorMessage = Settings.VAL_ERR_DESC_MAX_MSG)]
        public string Description { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public override DateTime? CreationDate { get; set; }

        /// <summary>
        /// Date when the action was completed
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        public override DateTime? EndDate { get; set; }

        public override Status Status { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Sprint> Sprints { get; set; }
                
        public void SetCode(string Code)
        {
            Code = Code ?? string.Empty;
            this.Code = Code.Substring(0, Math.Min(CODE_MAX_LENGTH, Code.Length)); 
        }

        public void SetName(string Name)
        {
            Name = Name ?? string.Empty;
            this.Name = Name.Substring(0, Math.Min(Settings.PTA_NAME_MAX_LENGTH, Name.Length));
        }

        public override ICollection<Pomodoro> GetPomodoros()
        {
            List<Pomodoro> lstPomodoros = new List<Pomodoro>();
            if (Tasks != null)
            {
                foreach (Task t in Tasks)
                {
                    lstPomodoros.AddRange(t.GetPomodoros());
                }
            }
            return lstPomodoros;
        }

        public override ICollection<Action> GetActions()
        {
            List<Action> lstActions = new List<Action>();
            foreach (Task t in Tasks)
            {
                lstActions.AddRange(t.Actions);
            }
            return lstActions;
        }

        public override ICollection<Activity> GetActivities()
        {
            List<Activity> lstActivities = new List<Activity>();
            foreach (Task t in Tasks)
            {
                lstActivities.AddRange(t.Activities);
            }
            return lstActivities;
        }

        public override ICollection<Task> GetTasks()
        {
            return Tasks;
        }

        [NotMapped]
        [Display(Name = "Ext. Status")]
        public ProjectExtendedStatus ExtendedStatus
        {
            get
            {
                switch (Status)
                {
                    case Status.Cancelled:
                        return ProjectExtendedStatus.Cancelled;
                    case Status.OnHold:
                        return ProjectExtendedStatus.OnHold;
                    case Status.Completed:
                        return ProjectExtendedStatus.Completed;
                    case Status.Active:
                        if (CompletedPomodorosCount > 0)
                        {
                            return ProjectExtendedStatus.InProgress;
                        }
                        break;
                }
                return ProjectExtendedStatus.Created;
            }
        }

        public override bool IsSelectable
        {
            get
            {
                return IsActive && GetTasks().Where(a => a.IsSelectable).Count() > 0;
            }
        }
        
        public override string ItemName
        {
            get
            {
                return (String.IsNullOrWhiteSpace(Name) ? Code : Name);
            }
        }

        public override string PathItemName
        {
            get 
            {
                return Code; 
            }
        }

        public override PomodoroContainerType Type
        {
            get
            {
                return PomodoroContainerType.Project;
            }
        }

        public override int Ident
        {
            get { return ID; }
        }

        public override String CssClass
        {
            get { return "gt-project" + (IsActive ? " gt-active" : string.Empty); }
        }

        public override bool InheritedStatus
        {
            get { return false; }
        }
        
        public override ApplicationUser Owner
        {
            get { return User; }
        }

        public Workspace Workspace { get { return new Workspace(User); } }

        public override LoggableItemContainer Parent { get { return Workspace; } }

        public override PomodoroContainer NextSibling
        {
            get
            {
                ICollection<Project> projectList = Workspace.GetProjects();
                Project sibling = projectList.OrderBy(p => p.ID).SkipWhile(p => p.ID != ID).Skip(1).FirstOrDefault();
                if(sibling == null)
                {
                    //end of list, get first
                    sibling = projectList.OrderBy(p => p.ID).FirstOrDefault();
                    //itself, not a sibling
                    if(sibling.ID == ID)
                    {
                        return null;
                    }
                }
                return sibling;
            }
        }

        public override Color TypeColor
        {
            get { return Color.Red; }
        }
    }
}