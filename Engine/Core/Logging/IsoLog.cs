using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Iso.Engine.Core.Logging
{
    public enum ELogVerbosity : byte
    {
        Trace,
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

        public static void Log(string category, string message, ELogVerbosity verbosity = ELogVerbosity.Log, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            string logMsg;

            if (verbosity == ELogVerbosity.Trace || verbosity == ELogVerbosity.Debug)
            {
                logMsg = string.Format("({0}:{1}) -> [{2}] {3} : {4}", callerMember, callerLine, verbosity, category, message);
            }
            else
            {
                logMsg = string.Format("[{0}] {1} : {2}", verbosity, category, message);
            }

            Debug.WriteLine(logMsg);

            assertLog(verbosity, logMsg);
        }

        /// <summary>
        /// Logs a string to the console using IsoLog and formats it.
        /// </summary>
        /// <param name="category">Log Category</param>
        /// <param name="format">Format using curly braces, e.g. {0}</param>
        /// <param name="verbosity">Verbosity, might not show up depending on compiler flags</param>
        /// <param name="args">any args that will be parsed respecting format string</param>
        public static void Logf(string category, string format, ELogVerbosity verbosity = ELogVerbosity.Log, params object[] args)
        {
            Log(category, string.Format(format, args), verbosity);
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
