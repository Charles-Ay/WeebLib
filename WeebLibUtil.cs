using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib
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
    }
}
