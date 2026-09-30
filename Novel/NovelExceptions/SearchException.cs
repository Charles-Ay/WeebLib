using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;

namespace WeebLib.Novel.NovelExceptions
{
    public class SearchException : WeebLibException
    {
        public SearchException(string message) : base(message)
        {
        }
    }
}
