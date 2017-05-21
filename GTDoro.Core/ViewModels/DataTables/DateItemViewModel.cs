using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    [NotMapped]
    public abstract class DateItemViewModel
    {
        public string HeaderTitle { get; set; }
        public string TableId { get; set; }
        public string IconCssClass { get; set; }
        public int RowsPerPage { get; set; }
    }

    public enum ReportTypeDate
    {
        LastPomodoro,
        NextDeadline,
        NextPlanifiedPomodoro,
        LastCreationDate
    }

}