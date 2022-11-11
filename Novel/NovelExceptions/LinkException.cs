using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;

namespace WeebLib.Novel.NovelExceptions
{
    internal class LinkException : WeebLibException
    {
        public LinkException(string message) : base(message)
        {
        }
    }
}
