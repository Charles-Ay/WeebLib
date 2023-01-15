using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Interfaces;
using WeebLib.Novel.NovelExceptions;
using WeebLib.Novel.Parser;
using WeebLib.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace WeebLib.Novel.Retrieval
{
    internal class NovelScrapper : IWebScrapper<NovelData>
    {
        protected string novelText = "";
        public override string Scrape(List<NovelData> data, string dir, bool outputToFile = true)
        {
            bool returnedValue = false;

            foreach (NovelData novel in data)
            {
                NovelSourceParser sourceParser = new NovelSourceParser();
                
                var html = base.GetSite(novel.initalLink);
                if (novel.initalLink.Contains("freewebnovel")) returnedValue = NovelSourceParser.Parse(NovelUtil.NovelSources.FreeWebNovel, html, out novelText);
                else if (novel.initalLink.Contains("fullnovel") || novel.initalLink.Contains("full-novel")) returnedValue = NovelSourceParser.Parse(NovelUtil.NovelSources.FullNovel, html, out novelText);
                else if (novel.initalLink.Contains("noveltrench")) returnedValue = NovelSourceParser.Parse(NovelUtil.NovelSources.NovelTrench, html, out novelText);

                if (returnedValue == false)
                {
                    throw new NovelContentException("Unable to parse novel content", WeebLibUtil.ContentType.Novel);
                }
                else if (novelText == "")
                {
                    throw new NovelContentException("Empty novel content", WeebLibUtil.ContentType.Novel);
                }

                RemoveExtraText();
                ReplaceTextFormating();
                
                if (outputToFile)
                {
                    //Total chapters becomes current chapter
                    var novelDir = Path.Combine(dir, novel.name);
                    if (!Directory.Exists(novel.name))
                    {
                        Directory.CreateDirectory(novelDir);
                    }

                    string fileName = Path.Combine(novelDir, $"{novel.name} - Chapter {novel.totalChapters}.txt");
                    if (!File.Exists(fileName)) File.Create(fileName).Dispose();

                    //FileStream writitng due to Numeric character reference
                    //https://en.wikipedia.org/wiki/Numeric_character_reference
                    FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate);
                    StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.WriteLine(novelText);
                    streamWriter.Close();
                    stream.Close();
                }
                else
                {
                    if (string.IsNullOrEmpty(novelText))
                    {
                        throw new NovelContentException("Empty novel content", WeebLibUtil.ContentType.Novel);
                    }
                    return novelText;
                }
            }
            return "";
        }

        private void ReplaceTextFormating()
        {
            novelText = novelText.Replace("&mdash;", "-");
            novelText = novelText.Replace("&amp;", "&");
            novelText = novelText.Replace("HtmlAgilityPack.HtmlNode", "");
        }

        private void RemoveExtraText()
        {
            novelText = novelText.Replace("𝐹𝑩𝒐𝑜𝑘𝑚𝙖𝙧𝑘 this website i𝚗𝑛𝘳e𝚊𝒹.c𝒐𝙢 to update the latest 𝒏𝗼𝘷𝘦𝘭𝘴.", "");
            novelText = novelText.Replace("𝘝𝐢𝐬𝐢𝐭 𝘧ree𝘸𝒆𝙗n𝒏𝗼vel.c𝒐𝙢, for the best novel reading 𝒆𝒙𝒑𝒆𝘳𝘪𝘦𝘯𝒄𝒆.", "");
            novelText = novelText.Replace("F𝙤𝒍𝒍𝑜𝑤 current novels on 𝘧𝑟ee𝘸e𝙗𝙣ov𝒆l&period;𝑐𝙤m.", "");
            novelText = novelText.Replace("𝙉𝙚𝙬 novel chapters are published 𝙤𝙣 fr𝘦𝒆𝙬e𝘣𝘯𝒐vel.𝒄𝗼𝙢.", "");
            novelText = novelText.Replace("𝘕𝙚𝑤 novel chapters are published 𝗼𝗻 𝑓ree𝘄e𝗯𝗻ov𝒆l&period;𝘤𝗼𝑚.", "");
            novelText = novelText.Replace("𝑩𝒐𝑜𝑘𝑚𝙖𝙧𝑘 this website f𝑟𝘦𝘦𝙬e𝗯𝗻ov𝐞l.𝒄𝘰𝒎 to update the latest 𝑛𝙤𝑣𝙚𝙡𝙨.", "");
            novelText = novelText.Replace("𝐵𝗼𝗼𝗸𝗺𝗮𝗿𝗸 this website 𝑓𝑟ee𝘸e𝘣𝗻ov𝒆l*𝘤𝙤𝘮 to update the latest 𝗻𝗼𝑣𝘦𝘭𝘴.", "");
            novelText = novelText.Replace("𝙏𝙝𝙞𝙨 chapter is updated 𝙗𝙮 𝚏𝘳e𝑒𝘄e𝙗𝘯ov𝐞l.c𝙤m.", "");
            novelText = novelText.Replace("𝑻𝒉𝒊𝒔 chapter upload first 𝒂𝒕 fr𝑒e𝙬𝑒𝗯𝙣ov𝒆l.𝒄𝗼m.", "");
        }
    }
}
