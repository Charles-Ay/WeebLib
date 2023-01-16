using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.Retrieval;
using WeebLib.Utility;
using static WeebLib.Novel.Retrieval.NovelSearcher;
using static WeebLib.Utility.WeebLibUtil;

namespace WeebLib.Novel.Parser
{
    public class NovelFetcher : IWebFetcher<NovelData>
    {
        /// <summary>
        /// Class to fetch novel data from a website
        /// </summary>
        public NovelFetcher()
        {
            SetWorkDir();
        }

        protected override string Fetch(NovelData data, int start, bool outputToFile = true)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            if (data.source == NovelUtil.sourceToString(NovelUtil.NovelSources.FullNovel))
                return new NovelScrapper().Scrape(new NovelSearcher().QueryFullNovelAndGetChapters(ref data, start), WorkDir, outputToFile);
            else return new NovelScrapper().Scrape(new NovelSearcher().QueryFreeWebNovelAndGetChapters(ref data, start), WorkDir, outputToFile);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Fetches content from web
        /// </summary>
        /// <param name="searchResults">Results of a search</param>
        /// <param name="first">first chapter to grab</param>
        /// <param name="amount">amount of chapters to grab</param>
        /// 
        /// <returns>text if output to file is false</returns>
        public string Fetch(SearchType searchResults, int first, int amount, bool outputToFile = true)
        {
            return Fetch(new NovelData(WeebLibUtil.RemoveSpecialCharacters(searchResults.name), amount, searchResults.latest, searchResults.link, searchResults.source), first, outputToFile);
        }

        /// <summary>
        /// Set the working directory
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="create">Flag to create a new folder but ignores the dir param</param>
        public void SetWorkDir(string dir, bool create)
        {
            if (create)
            {
                SetWorkDir();
            }
            else
            {
                SetWorkDir(dir);
            }
        }

        /// <summary>
        /// Set the working directory
        /// </summary>
        /// <param name="dir"></param>
        protected override void SetWorkDir(string dir = "")
        {
            if(dir == "")
            {
                dir = Directory.GetCurrentDirectory();
                if (string.IsNullOrEmpty(WorkDir))
                {
                    string newdir = Path.Combine(dir, "Novels");
                    Directory.CreateDirectory(newdir);
                    WorkDir = dir;
                    return;
                }
            }
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            WorkDir = dir;
        }
    }
}
