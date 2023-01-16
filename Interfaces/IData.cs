using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    /// <summary>
    /// This is the interface for the data that store the information of the anime/manga/novel.
    /// </summary>
    public abstract class IData
    {
        public string name { get; private set; }
        public int totalChapters { get; private set; }
        public double latestChapter { get; private set; }
        internal string initalLink { get; private set; }
        public string source { get; private set; }

        /// <summary>
        /// This is the constructor for the IData class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="totalChapterNum"></param>
        /// <param name="latestChapter"></param>
        /// <param name="initalLink"></param>
        /// <param name="source"></param>
        public IData(string name, int totalChapterNum, double latestChapter, string initalLink, string source)
        {
            this.name = name;
            this.totalChapters = totalChapterNum;
            this.latestChapter = latestChapter;
            this.initalLink = initalLink;
            this.source = source;
        }
    }
}
