using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class MenuSettings
    {
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Roles { get; set; }
        public string IconCssClass { get; set; }
        public IEnumerable<MenuSettings> SubNav { get; set; }
        public bool InMainMenu { get; set; }

        public bool HasSubNav
        { 
            get 
            { 
                return SubNav != null && SubNav.Count() > 0; 
            } 
        }
        public bool IsActive(string Title)
        {
            if (Title == "Project" || Title == "Task" || Title == "Action" || Title == "Sprint")
            {
                return LinkText == "Workspace";
            }
            if (Title == "Basics" || Title == "Manual")
            {
                return LinkText == "Help";
            }
            return LinkText == Title;
        }
        public static MenuSettings ActiveMenuSettings(string Title)
        {
            return Settings.MenuSettings
                .Where(ms => ms.IsActive(Title))
                .FirstOrDefault() ?? new MenuSettings();
        }

        public static MenuSettings GetMenuSettings(string LinkText)
        {
            return Settings.MenuSettings
                .Where(ms => ms.LinkText == LinkText)
                .FirstOrDefault() ?? new MenuSettings();
        }
    }

}