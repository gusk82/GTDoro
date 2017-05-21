using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class ReportViewModel
    {
        public ReportType ReportType { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}