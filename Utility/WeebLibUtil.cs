using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Utility
{
    public class WeebLibUtil
    {
        public enum ContentType
        {
            Novel,
            Manga,
            Anime
        }

        /// <summary>
        /// Replace last occurance of a string
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Find"></param>
        /// <param name="Replace"></param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            return Source.Remove(place, Find.Length).Insert(place, Replace);
        }
        
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Hold the data for the search results
        /// </summary>
        public class SearchType
        {
            public SearchType(string name, string link, int latest, string source, string image = "")
            {
                this.name = name;
                this.link = link;
                this.latest = latest;
                this.source = source;
                this.image = image;
            }
            public string name { get; private set; }
            public string link { get; private set; }
            public int latest { get; private set; }
            public string source { get; private set; }
            public string image { get; private set; }
        }
    }
}
