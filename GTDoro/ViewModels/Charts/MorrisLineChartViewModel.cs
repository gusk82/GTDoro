using GTDoro.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    public class MorrisLineChartViewModel
    {
        public string HeaderTitle { get; set; }
        public IEnumerable<DateTime> Dates { get; set; }
        public DateInterval Interval { get; set; }
        public string Label { get; set; }
        public string HtmlElementId { get; set; }
        public Color? LineColor { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int GetCountUntil(DateTime To)
        {
            return Dates.Where(d => d <= To).Count();
        }

        private List<string> DataRows
        {
            get
            {
                List<string> dataRows = new List<string>();
                string desc = String.Empty;
                int count = 0;
                IEnumerable<DateTime> FilteredDates = Dates;
                if(From.HasValue)
                {
                    FilteredDates = FilteredDates.Where(fd => fd >= From.Value);
                }
                else
                {
                    From = Dates.DefaultIfEmpty().Min();
                }
                if(To.HasValue)
                {
                    FilteredDates = FilteredDates.Where(fd => fd >= To.Value);
                }
                else
                {
                    To = Dates.DefaultIfEmpty().Max();
                }
                DateTime currentDate = new DateTime(From.Value.Year, From.Value.Month, 1);
                DateTime nextDate = currentDate.AddMonths((int)Interval).AddDays(-1);
                do
                {
                    desc = currentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    count = FilteredDates.Where(d => d >= currentDate && d <= nextDate).Count();
                    currentDate = currentDate.AddMonths((int)Interval);
                    nextDate = currentDate.AddMonths((int)Interval).AddDays(-1);
                    //if((int)Interval > 1)
                    //{
                    //    desc += "-" + currentDate.ToString("MMM", CultureInfo.InvariantCulture);
                    //}
                    dataRows.Add("{ y: '" + desc + "', '" + Label + "': " + count.ToString() + " }");
                } while(currentDate < To.Value);

                return dataRows;
            }
        }

        public string Script
        {
            get
            {
                return
                "<script type=\"text/javascript\">" +
                "    var IndexToMonth = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec' ];" +
                "    var line;" + 
                "    $(function () {" +
                "        \"use strict\";" +
                "        line = new Morris.Line({" +
                "            element: '" + HtmlElementId + "'," +
                "            resize: true," +
                "            data: [" +  String.Join(", ", DataRows) +
                "            ]," +
                "            xkey: 'y'," +
                "            ykeys: ['" + Label + "']," +
                "            labels: ['" + Label + "']," +
                "            lineColors: ['" + HtmlColorCode + "']," +
                "            parseTime: false," +
                "            hideHover: 'auto'," +
                "            xLabelFormat: function (x) { var date = new Date(x.label); return date.getFullYear() == 1 ? 'No work' : IndexToMonth[ date.getMonth() ] + ' ' + date.getFullYear(); }," +
                "        });" +
                "    })" +
                "</script>";
            }
        }

        private string HtmlColorCode
        {
            get
            {
                return LineColor.HasValue ? ColorTranslator.ToHtml(LineColor.Value) : "#3c8dbc";
            }
        }
    }
}