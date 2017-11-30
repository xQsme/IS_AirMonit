using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonit_Alarm.model
{
    class FormException : Exception
    {
        public List<string> Errors { get; set; }

        public FormException(List<string> errors)
        {
            this.Errors = errors;
        }
    }
}
