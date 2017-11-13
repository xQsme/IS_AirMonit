using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirMonit_DLog.Models
{
    public class Entry
    {
        public int id { get; set; }
        public Parameter parameter { get; set; }
        public decimal value { get; set; }
        public DateTime date { get; set; }
        public City city { get; set; }

        public enum Parameter
        {
            NITROGEN_DIOXIDE,
            CARBON_MONOXIDE,
            OZONE
        }
        public enum City
        {
            LEIRIA,
            LISBOA,
            PORTO
        }
    }
}