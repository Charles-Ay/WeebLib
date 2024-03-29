﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Utility;

namespace WeebLib.WeeLibExceptions
{
    public class WeebLibException : Exception
    {
        public string nonStackMessage = "";
        protected WeebLibException(string message) : base(SetExceptionMessage(message))
        {
            nonStackMessage = message;
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
