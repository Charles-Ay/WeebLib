using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Manga.Retrieval;
using WeebLib.Novel;
using static WeebLib.Utility.WeebLibUtil;

namespace WeebLib.Manga.Parser
{
    public class MangaFetcher : IWebFetcher<MangaData>
    {
        /// <summary>
        /// Class to fetch manga data from a website
        /// </summary>
        public MangaFetcher()
        {
            SetWorkDir();
        }

        protected override string Fetch(MangaData data, int start, bool outputToFile = true)
        {
            return new MangaScrapper().Scrape(new MangaSearcher().QueryKissMangaAndGetChapters(ref data, start), WorkDir);
        }

        /// <summary>
        /// Fetches content from web
        /// </summary>
        /// <param name="searchResults">Results of a search</param>
        /// <param name="first">first chapter to grab</param>
        /// <param name="amount">amount of chapters to grab</param>
        public string Fetch(SearchType searchResults, int first, int amount, bool outputToFile)
        {
            return Fetch(new MangaData(searchResults.name, amount, searchResults.latest, searchResults.link, searchResults.source), first, outputToFile);
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

        protected override void SetWorkDir(string dir = "")
        {
            if (dir == "")
            {
                dir = Directory.GetCurrentDirectory();
                if (string.IsNullOrEmpty(WorkDir))
                {
                    string newdir = Path.Combine(dir, "Manga");
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
