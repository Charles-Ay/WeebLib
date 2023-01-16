using HtmlAgilityPack;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WeebLib.Novel;

namespace WeebLib.Manga.Parser
{
    internal class MangaSourceParser 
    {
        /// <summary>
        /// Parses the source of a manga
        /// </summary>
        /// <param name="source"></param>
        /// <param name="html"></param>
        /// <param name="mangaText"></param>
        /// <returns>returns true if manga was sucessfully parsed</returns>
        internal static List<string> Parse(MangaUtil.MangaSources source, HtmlDocument html)
        {
            switch (source)
            {
                case MangaUtil.MangaSources.KissManga:
                    return KissMangaParse(html);
                default:
                    return new List<string>();
            }
        }
        
        /// <summary>
        /// Parse kissmanga data
        /// </summary>
        /// <param name="html"></param>
        /// <returns>List of image urls for a chapter</returns>
        private static List<string> KissMangaParse(HtmlDocument html)
        {
            var imageUrls = new List<string>();
            if (html.DocumentNode != null)
            {
                try
                {
                    var imageNode = html.DocumentNode.SelectSingleNode("//div[@id='centerDivVideo']");
                    foreach (var node in imageNode.SelectNodes(".//img"))
                    {
                        imageUrls.Add(node.Attributes["src"].Value);
                    }
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }
            else return new List<string>();
            return imageUrls;
        }
    }
}
