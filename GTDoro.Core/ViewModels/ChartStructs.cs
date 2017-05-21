using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GTDoro.Core.ViewModels
{    
    public class LineChartValue
    {
        public DateTime Date { get; set; }
        public int Value { get; set; }
    }

    public class DonutChartValue
    {
        public Decimal Percentage { get; set; }
        public PomodoroContainer Container { get; set; }
    }
}