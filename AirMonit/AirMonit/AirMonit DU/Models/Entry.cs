using System;

namespace AirMonit_DU.Models
{
    public class Entry
    {
        public int no2 { get; set; }
        public int co { get; set; }
        public int o3 { get; set; }
        public DateTime date { get; set; }
        public City city { get; set; }

        public enum City
        {
            LEIRIA,
            COIMBRA,
            PORTO,
            LISBOA
        }
    }
}