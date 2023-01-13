using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.WeeLibExceptions;

namespace WeebLib.Manga.MangaExceptions
{
    internal class MangaContentException : WeebLibException
    {
        public MangaContentException(string message = "Unable to parse manga content") : base(message) { }
    }
}
