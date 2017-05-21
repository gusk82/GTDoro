using GTDoro.Web.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.Web.ViewModels
{
    public class SettingsViewModel
    {
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
        public string TimeZoneId { get; set; }

        public static IEnumerable<SelectListItem> TimeZoneList
        {
            get
            {
                return TimeZoneInfo
                    .GetSystemTimeZones()
                    .Select(t => new SelectListItem
                    {
                        Text = t.DisplayName,
                        Value = t.Id/*,
                                Selected = Model != null && t.Id == Model.Id*/
                    });
            }
        }
    }

}