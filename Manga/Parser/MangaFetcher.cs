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
        public MangaFetcher()
        {
            SetWorkDir();
        }
        protected override string Fetch(MangaData data, int start, bool outputToFile = true)
        {
            return new MangaScrapper().Scrape(new MangaSearcher().QueryMangaSeeANdGetChapters(ref data, start), WorkDir);
        }

        public string Fetch(SearchType searchResults, int first, int amount, bool outputToFile)
        {
            return Fetch(new MangaData(searchResults.name, amount, searchResults.latest, searchResults.link, searchResults.source), first, outputToFile);
        }

        protected override void SetWorkDir(string dir = "")
        {
            dir = Directory.GetCurrentDirectory();
            var files = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                if (file.Contains("mangas") || file.Contains("Mangas"))
                {
                    WorkDir = dir;
                }
            }
            if (string.IsNullOrEmpty(WorkDir))
            {
                string newdir = Path.Combine(dir, "Mangas");
                Directory.CreateDirectory(newdir);
                WorkDir = dir;
            }
        }
    }
}
