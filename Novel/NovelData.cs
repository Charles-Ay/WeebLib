using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;

namespace WeebLib.Novel
{
    public class NovelData : IData
    {
        /// <summary>
        /// Stores the data of the novel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="totalChapterNum"></param>
        /// <param name="latestChapter"></param>
        /// <param name="initalLink"></param>
        /// <param name="source"></param>
        public NovelData(string name, int totalChapterNum, double latestChapter, string initalLink, string source) : base(name, totalChapterNum, latestChapter, initalLink, source) { }
    }
}
