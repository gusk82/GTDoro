using GTDoro.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    public class SettingsViewModel
    {
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
        public string TimeZoneId { get; set; }
    }
}