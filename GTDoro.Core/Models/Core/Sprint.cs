using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GTDoro.Core.Models;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTDoro.Core.Models
{
    public class Sprint : ActionContainer
    {
        public const int CODE_MAX_LENGTH = 14;

        #region DB fields

        public int ID { get; set; }

        [StringLength(Settings.PTA_NAME_MAX_LENGTH, ErrorMessage = Settings.VAL_ERR_NAME_MAX_MSG)]
        public string Name { get; set; }

        [Display(Name = "Start Date")]
        [UIHint("Date")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Start Date is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [DataType(DataType.DateTime)]
        public override DateTime? EndDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Initial Comment")]
        public string InitialComment { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Final Comment")]
        public string FinalComment { get; set; }
        
        [Required]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public override DateTime? CreationDate { get; set; }

        #endregion

        #region Calculated properties

        public string Code
        {
            get
            {
                return "Sprint " + OrderNum.ToString();
            }
        }

        public int OrderNum
        {
            get
            {
                if (Project != null && Project.Sprints != null)
                {
                    return Project.Sprints.Where(s => s.StartDate.Date < StartDate).Count() + 1;
                }
                return 1;
            }
        }

        #endregion

        #region Interface ActionContainer

        public override ICollection<Action> GetActions()
        {
            if (Project != null)
            {
                return Project.GetActions()
                    .Where(a => a.Pomodoros
                        .Any(p => p.StartLocal.HasValue && p.StartLocal.Value.Date >= StartDate.Date &&
                            (!EndDate.HasValue || EndDate.HasValue && p.StartLocal.Value.Date <= EndDate.Value.Date))
                ).ToList();
            }
            return new Action[0];
        }

        public override string PathItemName
        {
            get
            {
                return Code + " / " + Parent.PathItemName;
            }
        }

        public override string ItemName
        {
            get
            {
                return (String.IsNullOrWhiteSpace(Name) ? Code : Name);
            }
        }

        public override PomodoroContainerType Type
        {
            get { return PomodoroContainerType.Sprint; }
        }

        public override Color TypeColor
        {
            get { return Color.Purple; }
        }

        public override int Ident
        {
            get { return ID; }
        }

        public override string CssClass
        {
            get { return "gt-sprint" + (IsActive ? " gt-active" : string.Empty); }
        }

        public override bool InheritedStatus
        {
            get { return false; }
        }

        public override bool IsSelectable
        {
            get { return Status == Status.Active; }
        }

        public override ApplicationUser Owner
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
                Sprint sibling = Project.Sprints.OrderBy(s => s.StartDate).SkipWhile(s => s.ID != ID).Skip(1).FirstOrDefault();
                if (sibling == null)
                {
                    //end of list, get first
                    sibling = Project.Sprints.OrderBy(s => s.StartDate).FirstOrDefault();
                    //itself, not a sibling
                    if (sibling.ID == ID)
                    {
                        return null;
                    }
                }
                return sibling;
            }
        }

        [NotMapped]
        public override Status Status
        {
            get
            {
                if(DateTime.Today >= StartDate && 
                    (EndDate.HasValue == false || DateTime.Today <= EndDate))
                {
                    return Status.Active;
                }
                else if(EndDate.HasValue || DateTime.Today > EndDate)
                {
                    return Status.Completed;
                }
                return Status.OnHold;
            }
            set { }
        }

        public override ICollection<Pomodoro> GetPomodoros()
        {
            if (Project != null)
            {
                return Project.GetPomodoros().Where(p => p.StartLocal >= StartDate &&
                    (!EndDate.HasValue || p.StartLocal <= EndDateLocal.Value)).ToList();
            }
            return new Pomodoro[0];            
        }

        #endregion
    }
}