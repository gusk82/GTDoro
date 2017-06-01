using GTDoro.Core.ViewModels;
using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Drawing;
using GTDoro.Core.Models.Identity;
using GTDoro.Core.Extensions;

namespace GTDoro.Core.Models
{
    /// <summary>
    /// Work Container
    /// </summary>
    public abstract class LoggableItemContainer
    {
        public abstract decimal? Effort { get; }
        public abstract string PathItemName { get; }
        public abstract string ItemName { get; }
        public abstract Color TypeColor { get; }
        public abstract int Ident { get; }
        public abstract String CssClass { get; }
        public abstract bool InheritedStatus { get; }
        public abstract bool IsSelectable { get; }
        public abstract bool IsFinished { get; }
        public abstract ApplicationUser Owner { get; }
        public abstract Status Status { get; set; }
        public abstract DateTime? CreationDate { get; set; }
        public abstract DateTime? EndDate { get; set; }
        public abstract LoggableItemContainer Parent { get; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDateLocal { get { return EndDate.ToUserLocalTime(Owner.TimeZoneId); } }

        [Display(Name = "Status")]
        public Status CalculatedStatus
        {
            get
            {
                if(InheritedStatus)
                {
                    return Parent.CalculatedStatus;
                }
                return Status;
            }
        }

        public bool IsActive
        {
            get { return CalculatedStatus == Status.Active; }
        }

        protected bool GroupDifferentDates { get; set;}
              
        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreationDateLocal 
        { 
            get
            {
                return CreationDate.ToUserLocalTime(Owner.TimeZoneId);
            }
        }

        public Color RandomColor
        {
            get
            {
                return string.IsNullOrWhiteSpace(ItemName) ? 
                    Color.Black : ItemName.GetRandomColor();
            }
        }

        public string StatusIconHtml { get { return CalculatedStatus.GetIconHtmlTag() + InheritedStatusIconHtml; } }

        public string InheritedStatusIconHtml
        {
            get
            {
                if(InheritedStatus)
                {
                    return "<i title=\"Inherited (item status: " + Status.GetAttributeDisplayName() 
                        + ")\" class=\"gt-status gt-inherited fa fa-long-arrow-down\"></i>";
                }
                return string.Empty;
            }
        }

        public EffortGroup EffortGroup
        {
            get
            {
                if (Effort.HasValue == false)
                {
                    return EffortGroup.Indeterminate;
                }
                if (Effort.Value > 150)
                {
                    return EffortGroup.VeryExceeded;
                }
                if (Effort.Value > 100)
                {
                    return EffortGroup.Exceeded;
                }
                if (Effort.Value > 0)
                {
                    return EffortGroup.InProgress;
                }
                return EffortGroup.Created;
            }
        }

        #region Tags

        private ICollection<Tag> _tags;

        /// <summary>
        /// DB Tags
        /// </summary>
        public virtual ICollection<Tag> Tags
        {
            get
            {
                return _tags ?? (_tags = new HashSet<Tag>());
            }
            set
            {
                _tags = value;
            }
        }

        /// <summary>
        /// Own tags + Inherited tags
        /// </summary>
        public IEnumerable<Tag> OwnAndInheritedTags
        {
            get
            {
                return OwnTags.Concat(InheritedTags);
            }
        }

        /// <summary>
        /// DB Tags excluding inherited tags
        /// </summary>
        public IEnumerable<Tag> OwnTags
        {
            get
            {
                var inheritedTagIDs = InheritedTags.Select(t => t.ID);
                return Tags.Where(t => !inheritedTagIDs.Contains(t.ID));
            }
        }

        /// <summary>
        /// All tags from parents
        /// </summary>
        public IEnumerable<Tag> InheritedTags
        {
            get
            {
                return Parent != null ?
                    Parent.OwnTags.Concat(Parent.InheritedTags) :
                    new List<Tag>();
            }
        }

        /// <summary>
        /// All available user tags excluding own and inherited tags
        /// </summary>
        public IEnumerable<Tag> GetNotSelectedTags(IQueryable<Tag> availableTags)
        {
            var ownTagIDs = OwnTags.Select(t => t.ID);
            var inheritedTagIDs = InheritedTags.Select(t => t.ID);
            return availableTags.Where(t => !ownTagIDs.Contains(t.ID) && !inheritedTagIDs.Contains(t.ID));
        }

        #endregion

    }	

    

    
}
