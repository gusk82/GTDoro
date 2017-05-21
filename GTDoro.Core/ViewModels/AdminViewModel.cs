using GTDoro.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class AdminViewModel
    {
        public string MessageHtmlContent { get; set;}
        public IEnumerable<ApplicationUser> Users { get; set; }
        public MorrisLineChartViewModel RegisteredUsersChartViewModel { get; set; }
    }
}