using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;

namespace WeebLib.Manga.MangaExceptions
{
    internal class LastPageException : WeebLibException
    {
        public LastPageException(string message = "Last page reached") : base(message) { }
    }
}
