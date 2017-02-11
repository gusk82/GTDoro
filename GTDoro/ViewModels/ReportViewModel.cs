using GTDoro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    public class ReportViewModel
    {
        public ReportType ReportType { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}