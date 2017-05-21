using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class MorrisDonutChartViewModel
    {
        public string HeaderTitle { get; set; }
        public string HtmlElementId { get; set; }
        public IEnumerable<PomodoroContainer> Items { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool UseFullPathItemName { get; set; }
        
        private void loadRows()
        {
            _dataRows = new List<string>();
            _colorRows = new List<string>();
            Color randomColor;
            int count = 0;
            var colors = Settings.GRAPHIC_COLORS;
            int counter = 0;
            foreach (PomodoroContainer item in Items)
            {
                count = item.GetPomodoros(PomodoroCalculatedStatus.Completed, From, To).Count();
                if (count > 0)
                {
                    _dataRows.Add("{ label: '" + (UseFullPathItemName ? item.PathItemName : item.ItemName) + "', value: " + count.ToString() + " }");
                    randomColor = colors[counter++ % colors.Length];
                    _colorRows.Add("'rgb(" + randomColor.R.ToString() + ", " + randomColor.G.ToString() + ", " + randomColor.B.ToString() + ")'");
                }
            }
        }
        private List<string> _dataRows;
        private List<string> dataRows
        {
            get
            {
                if (_dataRows == null)
                {
                    loadRows();
                }
                return _dataRows;
            }
        }
        private List<string> _colorRows;
        private List<string> colorRows
        {
            get
            {
                if (_colorRows == null)
                {
                    loadRows();
                }
                return _colorRows;
            }
        }

        public string Script
        {


            get
            {
                return
                "<script type=\"text/javascript\">" +
                "   var donut;" + 
                "   $(function () {" +
                "        \"use strict\";" +
                "        donut = new Morris.Donut({" +
                "            element: '" + HtmlElementId + "'," +
                "            resize: true," +
                "            colors: [" + String.Join(", ", colorRows) +
                "            ]," +
                "            data: [" + String.Join(", ", dataRows) +
                "            ]," +
                "            hideHover: 'auto'" +
                "        });" +
                "    })" +
                "</script>";
            }
        }
    }
}