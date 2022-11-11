using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.WeeLibExceptions
{
    public class WeebLibException : Exception
    {
        private WeebLibException(string message, Exception inner) : base(message, inner)
        {
        }
        private WeebLibException(string message) : base(message)
        {
        }
        public WeebLibException(Exception inner, string message)
        {
            message += $" - thrown from {Logger.GetThrowFileAndLineNumber(Environment.StackTrace)}";
            message = message.Replace(":", " at ");
            Logger.writeToLog(message);
            throw new WeebLibException(message);
        }
    }
}
