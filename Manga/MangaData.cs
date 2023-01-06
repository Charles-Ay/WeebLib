using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;

namespace WeebLib.Manga
{
    public class MangaData : IData
    {
        public MangaData(string name, int firstChapter, double latestChapter, string initalLink, string source) : base(name, firstChapter, latestChapter, initalLink, source) { }
    }
}
