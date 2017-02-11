using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTDoro.Models
{
    public class KeyValueParameter
    {
        public int ID { get; set; }
        [StringLength(Settings.PARAMETER_CODE_MAX_LENGTH)]
        public string Code { get; set; }
        public string Value { get; set; }
    }
}