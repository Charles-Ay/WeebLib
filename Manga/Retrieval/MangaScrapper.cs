using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Manga.Parser;
using WeebLib.Novel;

namespace WeebLib.Manga.Retrieval
{
    internal class MangaScrapper : IWebScrapper<MangaData>
    {
        protected string? mangaText;
        public override int Scrape(List<MangaData> data, string dir)
        {
            bool returnedValue = false;
            int amount = 0;

            //Check if the "Manga" Dir exist. If not make a new one
            if (!Directory.Exists(Path.Combine(dir, "Manga"))) Directory.CreateDirectory(Path.Combine(dir, "Manga"));
            dir = Path.Combine(dir, "Manga");

            foreach (MangaData manga in data)
            {
                MangaSourceParser sourceParser = new();

                var html = base.GetSite(manga.initalLink);
                if (manga.initalLink.Contains("bato")) returnedValue = MangaSourceParser.Parse(MangaUtil.MangaSources.Bato, html, out mangaText);
                //else if (manga.initalLink.Contains("mangakakalot")) returnedValue = MangaSourceParser.Parse(MangaUtil.MangaSources.MangaKakalot, html, out mangaText);
                //else if (manga.initalLink.Contains("mangadex")) returnedValue = MangaSourceParser.Parse(MangaUtil.MangaSources.MangaDex, html, out mangaText);
            }
            
            throw new NotImplementedException();
        }
    }
}
