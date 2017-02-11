using GTDoro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.ViewModels
{
    public class PomodoroContainerChartsViewModel
    {
        //public PomodoroContainer Item { get; set; }
        public MorrisLineChartViewModel WorkAmount { get; set; }
        
        public MorrisDonutChartViewModel WorkDivision { get; set; }
        public IEnumerable<MorrisDonutChartViewModel> MultipleWorkDivision { get; set; }
    }
}