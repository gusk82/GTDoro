using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GTDoro.Core.Extensions;

namespace GTDoro.Core.Models
{
    public class CollectedThing
    {
        public const int NAME_MAX_LENGTH = 50;

        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(NAME_MAX_LENGTH, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string Name { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal { get { return CreationDate.ToUserLocalTime(User.TimeZoneId); } }
    }
}