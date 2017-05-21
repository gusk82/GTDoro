using GTDoro.Core.Models;
using Action = GTDoro.Core.Models.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace GTDoro.Core.ViewModels
{
    public class CalendarViewModel
    {
        public IEnumerable<Pomodoro> WorkHistory { get; set; }
        
        public CalendarViewMode Mode { get; set; }
        public PomodoroContainerType ContainerType { get; set; }
        public IEnumerable<Pomodoro> PlanifiedWork { get; set; }
        public IEnumerable<Action> ActiveActions { get; set; }
        public int? Height { get; set; }
        public bool DisplayX { get; set; }

        public IEnumerable<PomodoroSet> DailyWorkHistory
        {
            get
            {
                return PomodoroSet.GetPomodoroSets(WorkHistory, ContainerType);
            }
        }

        public string LastDateParams
        {
            get
            {
                Pomodoro lastPomodoro = WorkHistory.OrderByDescending(p => p.StartLocal).FirstOrDefault();
                if (lastPomodoro == null)
                {
                    return string.Empty;
                }
                return "year: " + lastPomodoro.StartLocal.Value.Year.ToString() + ", " +
                    "month: " + (lastPomodoro.StartLocal.Value.Month - 1).ToString() + ", ";
            }
        }

        public Dictionary<int, Color> DictProjectColors
        {
            get
            {
                var colors = Settings.GRAPHIC_COLORS;
                Dictionary<int, Color> dictProjectColors = new Dictionary<int,Color>();
                int count = 0;
                foreach(var line in WorkHistory.GroupBy(p => p.Action.Task.ProjectID)                        
                    .Select(group => new { 
                            ProjectID = group.Key,
                            Count = group.Count()
                    })
                    .OrderByDescending(l => l.Count))
                    
                {
                    dictProjectColors.Add(line.ProjectID, colors[count++ % colors.Length]);
                }
                return dictProjectColors;
            }
        }

        public Dictionary<int, Color> DictTaskColors
        {
            get
            {
                var colors = Settings.GRAPHIC_COLORS;
                Dictionary<int, Color> dictTaskColors = new Dictionary<int, Color>();
                int count = 0;
                foreach (var line in WorkHistory.GroupBy(p => p.Action.TaskID)
                    .Select(group => new
                    {
                        TaskID = group.Key,
                        Color = colors[count++ % colors.Length]
                    }))
                {
                    dictTaskColors.Add(line.TaskID, line.Color);
                }
                return dictTaskColors;
            }
        }

        public static string GetIntensityColorRGB(int value, PomodoroContainerType containerType)
        {
            Color c = GetIntensityColor(value, containerType);
            return GetColorRGB(c);
        }

        public static string GetColorRGB(Color c)
        {
            return "rgb(" + c.R + ", " + c.G + ", " + c.B + ")";
        }

        public static Color GetIntensityColor(int value, PomodoroContainerType containerType)
        {
            int mult = 0;
            switch(containerType)
            {
                case PomodoroContainerType.Workspace:
                    mult = 300;
                    break;
                case PomodoroContainerType.Project:
                case PomodoroContainerType.Sprint:
                    mult = 400;
                    break;
                case PomodoroContainerType.Task:
                    mult = 600;
                    break;
                case PomodoroContainerType.Action:
                    mult = 800;
                    break;
            }

            float x = Math.Min(4095, value * mult);
            int r = Math.Max(0, 200 - (int)(160f * (x / 4095f)));
            int g = Math.Max(0, 200 - (int)(219f * (x / 4095f)));
            int b = Math.Max(0, 200 - (int)(19f * (x / 4095f)));
            return Color.FromArgb(r, g, b);
        }

        public static Color DetermineFontColor(Color backgroundColour)
        {
            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = 1 - (0.299 * backgroundColour.R + 0.587 * backgroundColour.G + 0.114 * backgroundColour.B) / 255;

            if (luminance < 0.5)
                return Color.Black;

            return Color.White;
        }

        public static Color ConvertToSafeColor(Color color)
        {
            return Color.FromArgb(ConvertToSafeColor(color.R), ConvertToSafeColor(color.G), ConvertToSafeColor(color.B));
        }

        private static int ConvertToSafeColor(int colorComponenValue)
        {
            int c = colorComponenValue;
            if (c < 26)         { return 0;}
            else if(c < 77)     {return 51;}
            else if (c < 128)   {return 102;}
            else if (c < 179)   {return 153;}
            else if (c < 230)   {return 204;}
            return 255;
        }
    }
}