using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Interfaces
{
    public abstract class IData
    {
        public string name { get; private set; }
        public int totalChapters { get; private set; }
        public int latestChapter { get; private set; }
        internal string initalLink { get; private set; }
        public string source { get; private set; }

        public IData(string name, int totalChapterNum, int latestChapter, string initalLink, string source)
        {
            this.name = name;
            this.totalChapters = totalChapterNum;
            this.latestChapter = latestChapter;
            this.initalLink = initalLink;
            this.source = source;
        }
    }
}
