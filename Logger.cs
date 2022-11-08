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

        private Logger()
        {
            //if (File.Exists(NAME))
            //{
            //    File.Delete(NAME);
            //}
            if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
        }

        internal static void writeToLog(string text)
        {
            if (!File.Exists(NAME)) File.Create(NAME).Dispose();
            TextWriter writer = new StreamWriter(NAME, true);
            writer.WriteLine(text);
            writer.Close();
        }

        internal static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }
    }
}