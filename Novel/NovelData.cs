using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Novel
{
    public class NovelData
    {
        public string name { get; private set; }
        public int totalChapters { get; private set; }
        internal string initalLink { get; private set; }
        public string source { get; private set; }


        public NovelData(string name, int totalChapterNum, string initalLink, string source)
        {
            this.name = name;
            totalChapters = totalChapterNum;
            this.initalLink = initalLink;
            this.source = source;
        }
    }
}
