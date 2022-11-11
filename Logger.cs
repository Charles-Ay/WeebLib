using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib
{
    internal class Logger
    {
        private static string LogDir = Path.Combine(Directory.GetCurrentDirectory(), $@"Logs");
        private static string NAME = Path.Combine(LogDir, $"Logs_{DateTime.Now.ToFileTime()}.log");

        internal static void writeToLog(string text)
        {
            if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
            if (!File.Exists(NAME)) File.Create(NAME).Dispose();
            TextWriter writer = new StreamWriter(NAME, true);
            writer.WriteLine(text);
            writer.Close();
        }

        internal static int GetLineNumber(string stackTrace)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var index = stackTrace.LastIndexOf(lineSearch);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (index != -1)
            {
                var lineNumberText = stackTrace.Substring(index + lineSearch.Length);
                if (!int.TryParse(lineNumberText, out lineNumber))
                {
                    throw new Exception("Could not parse line number.");
                }
            }
            return lineNumber;
        }

        internal static string GetThrowFileAndLineNumber(string stackTrace)
        {
            var line = "";
            const string lineSearch = "\\";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var index = stackTrace.LastIndexOf(lineSearch);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (index != -1)
            {
                line = stackTrace.Substring(index + lineSearch.Length);
            }
            return line;
        }
    }
}