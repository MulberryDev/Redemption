using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Redemption
{
    class Logger
    {
        private static string source = "Redemption";
        private static string log = "Application";

        public Logger(string message)
        {
        }
        
        public static void WriteLine(string message)
        {
             if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, log);
             EventLog.WriteEntry(source, message);
        }

        public static void WriteLine(string message, object arg0)
        {
            if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, log);
            EventLog.WriteEntry(source, string.Format(message, arg0));
        }

        public static void WriteLine(string message, object arg0, object arg1)
        {
            if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, log);
            EventLog.WriteEntry(source, string.Format(message, arg0, arg1));
        }

        public static void WriteLine(string message, object arg0, object arg1, object arg2)
        {
            if (!EventLog.SourceExists(source)) EventLog.CreateEventSource(source, log);
            EventLog.WriteEntry(source, string.Format(message, arg0, arg1, arg2));
        }
    }
}
