using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirMonit_Service.Models
{
    public class LogHelper
    {
        public static string Error { get; set; }
        public static string Query { get; set; }

        public static void WriteLog(string error, string query)
        {
            Error = Error;
            Query = query;
        }

        public static string GetLog()
        {
            return "[ERROR]: " + Error + "\n[QUERY]: " + Query;
        }
    }
}