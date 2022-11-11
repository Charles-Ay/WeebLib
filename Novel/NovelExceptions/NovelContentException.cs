using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;
using static WeebLib.WeebLibUtil;

namespace WeebLib.Novel.NovelExceptions
{
    internal class NovelContentException : ContentException
    {
        public NovelContentException(string message, ContentType content) : base(message, content)
        {
        }
    }
}
