using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeebLib.Utility.WeebLibUtil;

namespace WeebLib.WeeLibExceptions
{
    public class ContentException : WeebLibException
    {
        protected ContentException(string message, ContentType content) : base(ContentEdit(message, content))
        {
        }
        
        private static string ContentEdit(string message, ContentType content)
        {
            return $"{message} : unable to retrive {content}";
        }
    }
}
