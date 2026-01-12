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

            assertLog(verbosity, logMsg);
        }

        public static void Logf(string category, string format, ELogVerbosity? verbosity = ELogVerbosity.Log, params object[] args)
        {
            string userfmt = string.Format(format, args);
            string logMsg = string.Format("[{0}]: {1} : {2}", verbosity, category, userfmt);

            assertLog(verbosity, logMsg);   
        }

        private static void assertLog(ELogVerbosity? verbosity, string logMsg)
        {
            bool isValid = verbosity != null;
            bool fatal = verbosity == ELogVerbosity.Fatal;

            bool panic = isValid && !fatal;

            Debug.Assert(panic, logMsg);
        }
    }
}
