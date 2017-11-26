using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAirEntries
{
    public class AlarmEntry
    {
        public string Particle { get; set; }
        public string Condition { get; set; }
        public decimal[] ConditionValues { get; set; }
        public decimal EntryValue { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }

    }
}
