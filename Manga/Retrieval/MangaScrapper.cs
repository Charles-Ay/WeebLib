using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Manga.Parser;
using WeebLib.Novel;
using WeebLib.Manga.MangaExceptions;
using System.Net;

namespace WeebLib.Manga.Retrieval
{
    internal class MangaScrapper : IWebScrapper<MangaData>
    {
        protected string? mangaText;
        public override string Scrape(List<MangaData> data, string dir, bool outputToFile = true)
        {
            List<string> images = new List<string>();

            //Check if the "Manga" Dir exist. If not make a new one
            if (!Directory.Exists(Path.Combine(dir, "Manga"))) Directory.CreateDirectory(Path.Combine(dir, "Manga"));
            var mangaDir = Path.Combine(dir, "Manga");



            foreach (MangaData manga in data)
            {
                //Check if the "Manga" Dir exist. If not make a new one
                if (!Directory.Exists(Path.Combine(mangaDir, $"{manga.name} Chapter {manga.latestChapter}"))) Directory.CreateDirectory(Path.Combine(mangaDir, $"{manga.name} Chapter {manga.latestChapter}"));
                dir = Path.Combine(mangaDir, $"{manga.name} Chapter {manga.latestChapter}");
                MangaSourceParser sourceParser = new();

                var html = GetSite(manga.initalLink);
                if (manga.initalLink.Contains("kissmanga")) images = MangaSourceParser.Parse(MangaUtil.MangaSources.KissManga, html);
                if(images.Count > 0)
                {
                    int imgNumber = 1;
                    foreach (string image in images)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(image);
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            using (Stream stream = response.GetResponseStream())
                            {
                                using (FileStream fileStream = new FileStream(Path.Combine(dir, $"{manga.name} - Chapter {manga.latestChapter}_{imgNumber}.jpg"), FileMode.Create, FileAccess.Write))
                                {
                                    byte[] buffer = new byte[4096];
                                    int bytesRead;
                                    do
                                    {
                                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                                        fileStream.Write(buffer, 0, bytesRead);
                                    } while (bytesRead != 0);
                                }
                            }
                        }
                        ++imgNumber;
                    }
                }
                else
                {
                    throw new MangaContentException();
                }
            }
            return "";
        }
    }
}
