using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAirEntries
{
    public class IncidentEntry
    {
        public string Message { get; set; }
        public string Event { get; set; }
        public string Publisher { get; set; }
        public string OtherEvent { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
    }
}
