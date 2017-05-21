using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using GTDoro.Core.Models;
using System.Drawing;

namespace GTDoro.Core.Models
{
    public class Task : ActionContainer
    {
        public const int CODE_MAX_LENGTH = 24;

        public Task()
        {
            Actions = new HashSet<Action>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(CODE_MAX_LENGTH, ErrorMessage = "Code cannot be longer than 24 characters")]
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

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [DataType(DataType.DateTime)]
        public override DateTime? EndDate { get; set; }

        public override Status Status { get; set; }

        public LevelExtended Priority { get; set; }

        [Required] 
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<Action> Actions { get; set; }
        
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

        public override ICollection<Action> GetActions()
        {
            return Actions;
        }

        public override ICollection<Pomodoro> GetPomodoros()
        {
            List<Pomodoro> lstPomodoros = new List<Pomodoro>();
            if (Actions != null)
            {
                foreach (Action a in Actions)
                {
                    lstPomodoros.AddRange(a.Pomodoros);
                }
            }
            return lstPomodoros;
        }
                
        [NotMapped]
        [Display(Name = "Ext. Status")]
        public TaskExtendedStatus ExtendedStatus
        {
            get
            {
                if (InheritedStatus == false)
                {
                    switch (Status)
                    {
                        case Status.Cancelled:
                            return TaskExtendedStatus.Cancelled;
                        case Status.OnHold:
                            return TaskExtendedStatus.OnHold;
                        case Status.Completed:
                            return TaskExtendedStatus.Completed;
                        case Status.Active:
                            if (CompletedPomodorosCount > 0)
                            {
                                return TaskExtendedStatus.InProgress;
                            }
                            break;
                    }
                }
                else
                {
                    switch (this.Project.Status)
                    {
                        case Status.Cancelled:
                            return TaskExtendedStatus.CancelledInherited;
                        case Status.Completed:
                            return TaskExtendedStatus.CompletedInherited;
                        case Status.OnHold:
                            return TaskExtendedStatus.OnHoldInherited;
                    }
                }
                return TaskExtendedStatus.Created;
            }
        }
        
        public override bool IsSelectable
        {
            get
            {
                return IsActive && GetActions().Where(a => a.IsSelectable).Count() > 0;
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
                return Code + " / " + Parent.PathItemName;
            }
        }
        
        public override PomodoroContainerType Type
        {
            get 
            {
                return PomodoroContainerType.Task;
            }
        }

        public override int Ident
        {
            get { return ID; }
        }

        public override String CssClass
        {
            get { return "gt-task" + (IsActive ? " gt-active" : string.Empty); }
        }

        public override bool InheritedStatus
        {
            get
            {
                return  Status == Status.Active &&
                        Project != null && Project.IsActive == false;
            }
        }

        public override Identity.ApplicationUser Owner
        {
            get
            {
                if (Project != null)
                {
                    return Project.User;
                }
                return null;
            }
        }

        public override PomodoroContainer Parent { get { return Project; } }

        public override PomodoroContainer NextSibling
        {
            get
            {
                Task sibling = Project.Tasks.OrderBy(t => t.ID).SkipWhile(t => t.ID != ID).Skip(1).FirstOrDefault();
                if (sibling == null)
                {
                    //end of list, get first
                    sibling = Project.Tasks.OrderBy(p => p.ID).FirstOrDefault();
                    //itself, not a sibling
                    if (sibling.ID == ID)
                    {
                        return null;
                    }
                }
                return sibling;
            }
        }

        public override Color TypeColor
        {
            get { return Color.DarkBlue; }
        }
    }
}