using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GTDoro.Core.Extensions;

namespace GTDoro.Core.Models
{
    public class Tag
    {
        public int ID { get; set; }

        [Required] 
        public string Code { get; set; }

        [Required] 
        public string Name { get; set; }

        public string Description { get; set; }

        public string IconCssClass { get; set; }
        public string GroupName { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDate { get; set; }

        public bool IsFixed { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Action> Actions { get; set; }
        public virtual ICollection<Sprint> Sprints { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal { get { return CreationDate.ToUserLocalTime(User.TimeZoneId); } }
    }
}