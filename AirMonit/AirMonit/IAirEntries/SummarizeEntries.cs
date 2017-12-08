using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAirEntries
{
    public class SummarizeEntries
    {
        public string Particle { get; set; }
        public DateTime Date { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Average { get; set; }
        public string City { get; set; }
    }
}
