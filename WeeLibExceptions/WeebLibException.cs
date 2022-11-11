using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.WeeLibExceptions
{
    public class WeebLibException : Exception
    {
        protected WeebLibException(string message) : base(SetExceptionMessage(message))
        {
        }
        private static string SetExceptionMessage(string message)
        {
            message += $" - thrown from {Logger.GetFileAndLineNumber(Environment.StackTrace)}";
            message = WeebLibUtil.ReplaceLastOccurrence(message, ":", " at ");
            Logger.writeToLog(message);
            return message;
        }
    }
}
