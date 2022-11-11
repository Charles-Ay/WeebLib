using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;

namespace WeebLib.Novel.NovelExceptions
{
    public class SourceException : WeebLibException
    {
        //public SourceException(string message, Exception inner) : base(message, inner)
        //{
        //}
        //public SourceException(string message) : base(message)
        //{
        //}
        public SourceException(Exception inner, string message) : base(inner, message)
        {
        }
    }
}
