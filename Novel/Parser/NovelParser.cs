using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.Retrieval;
using static WeebLib.Novel.Parser.NovelSearcher;

namespace WeebLib.Novel.Parser
{
    public class NovelParser : IWebParser<NovelData>
    {
        public NovelParser()
        {
            SetWorkDir();
        }

        protected override int Fetch(NovelData data, int start)
        {
            NovelScrapper scrapper = new NovelScrapper();
            NovelSearcher searcher = new NovelSearcher();
            List<NovelData> novels = searcher.GetNovelChapters(ref data, ref start);
            return scrapper.Scrape(novels, base.WorkDir);
        }
        
        public int Fetch(SearchType searchResults, int first, int last)
        {
            return Fetch(new NovelData(searchResults.name, last, searchResults.link, searchResults.source), first);
        }

        protected override void SetWorkDir()
        {
            string dir = Directory.GetCurrentDirectory();
            var files = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                if (file.Contains("novels") || file.Contains("Novels"))
                {
                    WorkDir = dir;
                }
            }
            if(string.IsNullOrEmpty(WorkDir))
            {
                string newdir = Path.Combine(dir, "Novels");
                Directory.CreateDirectory(newdir);
                WorkDir = dir;
            }
        }
    }
}
