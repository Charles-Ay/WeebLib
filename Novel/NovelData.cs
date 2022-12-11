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
        public NovelData(string name, int totalChapterNum, string initalLink, string source) : base(name, totalChapterNum, initalLink, source) { }
    }
}
