using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.Retrieval;
using static WeebLib.Novel.Retrieval.NovelSearcher;
using static WeebLib.Utility.WeebLibUtil;

namespace WeebLib.Novel.Parser
{
    public class NovelFetcher : IWebFetcher<NovelData>
    {
        public NovelFetcher()
        {
            SetWorkDir();
        }

        protected override int Fetch(NovelData data, int start)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            if (data.source == NovelUtil.sourceToString(NovelUtil.NovelSources.FullNovel))
                return new NovelScrapper().Scrape(new NovelSearcher().QueryFullNovelAndGetChapters(ref data, start), WorkDir);
            else return new NovelScrapper().Scrape(new NovelSearcher().GetNovelChapters(ref data, start), WorkDir);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Fetches content from web
        /// </summary>
        /// <param name="searchResults">Results of a search</param>
        /// <param name="first">first chapter to grab</param>
        /// <param name="amount">amount of chapters to grab</param>
        /// <returns>number of chapters retrived</returns>
        public int Fetch(SearchType searchResults, int first, int amount)
        {
            return Fetch(new NovelData(searchResults.name, amount, searchResults.link, searchResults.source), first);
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
