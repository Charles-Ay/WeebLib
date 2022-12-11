using HtmlAgilityPack;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Novel;

namespace WeebLib.Manga.Parser
{
    internal class MangaSourceParser 
    {
        /// <summary>
        /// Parses the source of a novel
        /// </summary>
        /// <param name="source"></param>
        /// <param name="html"></param>
        /// <param name="mangaText"></param>
        /// <returns>returns true if novel was sucessfully parsed</returns>
        internal static bool Parse(MangaUtil.MangaSources source, HtmlDocument html, out string mangaText)
        {
            mangaText = "";
            switch (source)
            {
                case MangaUtil.MangaSources.Bato:
                    return BatoParse(html, out mangaText);
                default:
                    return false;
            }
        }
        
        private static bool BatoParse(HtmlDocument html, out string mangaText)
        {
            mangaText = "";
            if (html.DocumentNode != null)
            {
                try
                {
                    //Get title
                    //mangaText += html.DocumentNode.SelectSingleNode("//h1[@class='tit']");

                    //access token to be concatinated for image requests
                    string accessToken = html.DocumentNode.SelectSingleNode("//meta[@property='og:image']").GetAttributeValue("content", "");
                    accessToken = accessToken.Substring(accessToken.IndexOf("acc=") + 13);
                    accessToken = "acc=" + accessToken;
                    accessToken = accessToken.Replace("&amp;exp", "&exp");

                    var imageListStartIndex = html.DocumentNode.InnerHtml.IndexOf("imgHttpLis = ") + "imgHttpLis = ".Length ;
                    var imageListEndIndex = html.DocumentNode.InnerHtml.IndexOf("    const batoPass");
                    String result = html.DocumentNode.InnerHtml.Substring(imageListStartIndex, imageListEndIndex - imageListStartIndex);
                    result = result.Substring(1, result.Length - 2);
                    result = result.Substring(0, result.Length - 2);

                    string[] imageUrls = result.Split("\",\"");
                    for(int i = 0; i < imageUrls.Length; ++i)
                    {
                        imageUrls[i] = imageUrls[i].Replace("\"", "");
                        imageUrls[i] += $"?{accessToken}";
                    }


                }
                catch (Exception)
                {
                    return false;
                }
            }
            else return false;
            return true;
        }
    }
}
