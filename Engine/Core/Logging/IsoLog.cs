using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Logging
{
    public enum ELogVerbosity : byte
    {
        Display,
        Debug,
        Info,
        Log,
        Warning,
        Error,
        Fatal,
    }

    public static class IsoLog
    {
        public static void Log(string category, string message, ELogVerbosity? verbosity = ELogVerbosity.Log)
        {
            string logMsg = string.Format("[{0}]: {1} : {2}", verbosity, category, message);
            System.Diagnostics.Debug.WriteLine(logMsg);

            if (verbosity == ELogVerbosity.Fatal)
            {
                Debug.Assert(false, logMsg);
            }
        }
    }
}
