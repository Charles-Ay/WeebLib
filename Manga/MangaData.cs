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
        /// <summary>
        /// Holds the data of the manga
        /// </summary>
        /// <param name="name"></param>
        /// <param name="firstChapter"></param>
        /// <param name="latestChapter"></param>
        /// <param name="initalLink"></param>
        /// <param name="source"></param>
        public MangaData(string name, int firstChapter, double latestChapter, string initalLink, string source) : base(name, firstChapter, latestChapter, initalLink, source) { }
    }
}
