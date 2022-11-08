using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeebLib.Novel.Parser
{
    internal class SourceParser
    {
        internal bool Parse(NovelUtil.NovelSources source, HtmlDocument html, out string novelText)
        {
            novelText = "";
            switch (source)
            {
                case NovelUtil.NovelSources.FreeWebNovel:
                    return FreeWebNovelParse(html, out novelText);
                case NovelUtil.NovelSources.NovelTrench:
                    return NovelTrenchParse(html, out novelText); ;
                default:
                    return false;
            }
        }

        private static bool FreeWebNovelParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//h1[@class='tit']"))
                    {
                        novelText = node.InnerText;
                    }

                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='txt ']"))
                    {
                        novelText = node.InnerText;
                    }
                }
                catch (Exception e)
                {
                    Logger.writeToLog($"HTML ERROR - Line:{Logger.GetLineNumber(e)} -- {e.Message}");
                    return false;
                }
            }
            else return false;
            return true;
        }

        private static bool NovelTrenchParse(HtmlDocument html, out string novelText)
        {
            novelText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    foreach (HtmlNode node in html.DocumentNode.SelectNodes("//div[@class='text-left']"))
                    {
                        novelText = HttpUtility.HtmlDecode(node.InnerText);
                    }
                }
                catch (Exception e)
                {
                    Logger.writeToLog($"HTML ERROR - Line:{Logger.GetLineNumber(e)} -- {e.Message}");
                    return false;
                }
            }
            else return false;
            return true;
        }
    }
}
