using GTDoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTDoro.Models
{
    public class Settings
    {
        public const string ADMIN_USERNAME = "admin@admin.com";
        public const int TIMEZONE_OFFSET_MIN = 60;
        public const int POMOWORK = 25;
        public const int POMOBREAK = 5;
        public const int POMOCYCLE = POMOWORK + POMOBREAK;
        public const int DEFAULT_WORK_LENGTH = 2;
        public const string ICON_COMPLETED = "fa-check-square-o";
        public const string ICON_DEADLINE = "fa-compress";
        public const string DEFAULT_NULL_STRING_VALUE = "<not set>";
        public const string MSG_SUCCESSFUL_UPDATE = "Your data has been successfully updated.";
        public const string MSG_SUCCESSFUL_DELETE = "Your data has been successfully deleted.";
        public const string MSG_SUCCESSFUL_CREATE = "Your data has been successfully created.";
        public const string MSG_UNSUCCESSFUL_BECAUSE_WORKING = "Please cancel or confirm your current work before performing this operation";
        public const string MSG_SUCCESSFUL_OPERATION = "The operation has been successfully performed.";

        public const int PARAMETER_CODE_MAX_LENGTH = 24;
        public const int PTA_NAME_MAX_LENGTH = 36;
        public const string VAL_ERR_NAME_MAX_MSG = "Name cannot be longer than 36 characters";
        public const int PTA_DESCRIPTION_MAX_LENGTH = 500;
        public const string VAL_ERR_DESC_MAX_MSG = "Description cannot be longer than 500 characters";

        public const string DEFAULT_REG_MSG_SUBJECT = "Welcome to GTDoro - Account confimation";
        public const string DEFAULT_REG_MSG_CONTENT = "Please confirm your account by clicking this link:";

        public static string GetError404Path(string filePath)
        {
            return "/Base/Http404?aspxerrorpath=" + filePath;
        }

        public static DateTime DefaultPlanifyDateTime
        {
            get
            {
                return DateTime.Today.AddDays(1).AddHours(18);
            }
        }

        public static TimeSpan DefaultPlanifyTime
        {
            get
            {
                return DefaultPlanifyDateTime.TimeOfDay;
            }
        }


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


        public static Color[] GRAPHIC_COLORS 
        {
            get
            {
                return new Color[] { 
                    ColorTranslator.FromHtml("#E53A20"), //red
                    ColorTranslator.FromHtml("#E0CD0B"), //yellow
                    ColorTranslator.FromHtml("#008d4c"), //green
                    ColorTranslator.FromHtml("#428bca"), //blue
                    ColorTranslator.FromHtml("#4CD4F5"), //aqua
                    ColorTranslator.FromHtml("#932ab6"), //purple 
                    ColorTranslator.FromHtml("#5A6F74"), //grey
                    ColorTranslator.FromHtml("#ff851b"), //orange
                    ColorTranslator.FromHtml("#BBFFE0"), //light green
                    ColorTranslator.FromHtml("#001f3f"), //navy
                    ColorTranslator.FromHtml("#85144b"), //maroon
                    ColorTranslator.FromHtml("#3d9970"), //olive        
                };
            }
        }

        public static MenuSettings[] MenuSettings =
            new MenuSettings[]
            {
                new MenuSettings { LinkText="Dashboard", ActionName="Dashboard",ControllerName="Layout",Roles="All", IconCssClass="fa-dashboard", InMainMenu = true },                
                new MenuSettings { LinkText="Calendar", ActionName="Calendar",ControllerName="Layout",Roles="All", IconCssClass="fa-calendar", InMainMenu = true },                
                new MenuSettings { LinkText="Review", ActionName="Review",ControllerName="Layout",Roles="All", IconCssClass="fa-check", InMainMenu = true },
                //new MenuSettings { LinkText="Projects", ActionName="Project",ControllerName="Layout",Roles="All", IconCssClass="fa-gears", InMainMenu = true },

                new MenuSettings { LinkText="Reports", ActionName="Reports",ControllerName="Layout",Roles="All", IconCssClass="fa-bar-chart-o", InMainMenu = true },
                new MenuSettings { LinkText="Workspace", ActionName="Project",ControllerName="Layout",Roles="All", IconCssClass="fa-gears", InMainMenu = true,
                    SubNav = new List<MenuSettings>
                    {
                        new MenuSettings { LinkText="Projects", ActionName="Index",ControllerName="Project",Roles="All", IconCssClass="fa-bullseye", InMainMenu = true },
                        new MenuSettings { LinkText="Tasks", ActionName="Index",ControllerName="Task",Roles="All", IconCssClass="fa-dot-circle-o", InMainMenu = false },
                        new MenuSettings { LinkText="Actions", ActionName="Index",ControllerName="Action",Roles="All", IconCssClass="fa-circle", InMainMenu = false },
                        new MenuSettings { LinkText="Sprints", ActionName="Index",ControllerName="Sprint",Roles="All", IconCssClass="fa-calendar-o", InMainMenu = false }
                    }
                },
                new MenuSettings { LinkText="Admin", ActionName="Admin",ControllerName="Layout",Roles="Administrator", IconCssClass="fa-key", InMainMenu = true },
                new MenuSettings { LinkText="Help", ActionName="Help",ControllerName="Layout",Roles="All", IconCssClass="fa-question-circle", InMainMenu = true,
                    SubNav = new List<MenuSettings>
                    {
                        //new MenuSettings { LinkText="Fundamentos", ActionName="Fundamentos",ControllerName="Help",Roles="All", IconCssClass="fa-info-circle", InMainMenu = true },
                        //new MenuSettings { LinkText="Manual", ActionName="Manual",ControllerName="Help",Roles="All", IconCssClass="fa-book", InMainMenu = false },                        
                        new MenuSettings { LinkText="Basics", ActionName="Basics",ControllerName="Help",Roles="All", IconCssClass="fa-info-circle", InMainMenu = true },
                        new MenuSettings { LinkText="Manual", ActionName="Manual",ControllerName="Help",Roles="All", IconCssClass="fa-book", InMainMenu = false },                
                    }
                },
                new MenuSettings { LinkText="My Account", ActionName="Manage",ControllerName="Layout",Roles="All", IconCssClass="fa-user", InMainMenu = false },
                new MenuSettings { LinkText="Search", ActionName="Index",ControllerName="Search",Roles="All", IconCssClass="fa-search", InMainMenu = false },
                //new MenuSettings { LinkText="Workspace", ActionName="Project",ControllerName="Layout",Roles="All", IconCssClass="fa-bullseye", InMainMenu = false },
                //new MenuSettings { LinkText="Workspace", ActionName="Task",ControllerName="Layout",Roles="All", IconCssClass="fa-dot-circle-o", InMainMenu = false },
                //new MenuSettings { LinkText="Workspace", ActionName="Action",ControllerName="Layout",Roles="All", IconCssClass="fa-circle", InMainMenu = false },

                //new MenuSettings { LinkText="Admin", ActionName="Admin",ControllerName="Layout",Roles="Administrator", IconCssClass="fa-users", InMainMenu = true },
                //new MenuSettings { LinkText="Dashboard", ActionName="Dashboard",ControllerName="Layout",Roles="All", IconCssClass="fa-dashboard", InMainMenu = true },
                //new MenuSettings { LinkText="Calendar", ActionName="Calendar",ControllerName="Layout",Roles="All", IconCssClass="fa-calendar", InMainMenu = true },
                //new MenuSettings { LinkText="Reports", ActionName="Reports",ControllerName="Layout",Roles="All", IconCssClass="fa-bar-chart-o", InMainMenu = true },
                //new MenuSettings { LinkText="Review", ActionName="Review",ControllerName="Layout",Roles="All", IconCssClass="fa-check", InMainMenu = true },
                ////new MenuSettings { LinkText="Projects", ActionName="Project",ControllerName="Layout",Roles="All", IconCssClass="fa-gears", InMainMenu = true },
                //new MenuSettings { LinkText="My Account", ActionName="Manage",ControllerName="Layout",Roles="All", IconCssClass="fa-user", InMainMenu = false },
                //new MenuSettings { LinkText="Workspace", ActionName="Project",ControllerName="Layout",Roles="All", IconCssClass="fa-gears", InMainMenu = true },
                //new MenuSettings { LinkText="Workspace", ActionName="Task",ControllerName="Layout",Roles="All", IconCssClass="fa-dot-circle-o", InMainMenu = false },
                //new MenuSettings { LinkText="Workspace", ActionName="Action",ControllerName="Layout",Roles="All", IconCssClass="fa-circle", InMainMenu = false },
            };
    }

}