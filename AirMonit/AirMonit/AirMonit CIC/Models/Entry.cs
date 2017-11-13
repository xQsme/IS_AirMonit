using System;

namespace AirMonit_CIC.Models
{
    public class Entry
    {
        public int no2 { get; set; }
        public Boolean no2Alarm { get; set; }
        public int co { get; set; }
        public Boolean coAlarm { get; set; }
        public int o3 { get; set; }
        public Boolean o3Alarm { get; set; }
        public DateTime date { get; set; }
        public City city { get; set; }

        public enum City
        {
            LEIRIA,
            COIMBRA,
            LISBOA,
            PORTO
        }
    }
}