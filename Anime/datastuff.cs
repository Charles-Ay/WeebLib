using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeebLib.Anime
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AiredStart
    {
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
    }

    public class AvailableEpisodes
    {
        public int sub { get; set; }
        public int dub { get; set; }
        public int raw { get; set; }
    }

    public class Data
    {
        public Shows shows { get; set; }
    }

    public class Dub
    {
        public string episodeString { get; set; }
        public string notes { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
    }

    public class Edge
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public string nativeName { get; set; }
        public string thumbnail { get; set; }
        public LastEpisodeInfo lastEpisodeInfo { get; set; }
        public LastEpisodeDate lastEpisodeDate { get; set; }
        public string type { get; set; }
        public Season season { get; set; }
        public double? score { get; set; }
        public AiredStart airedStart { get; set; }
        public AvailableEpisodes availableEpisodes { get; set; }
        public string episodeDuration { get; set; }
        public DateTime lastUpdateEnd { get; set; }
        public string __typename { get; set; }
    }

    public class LastEpisodeDate
    {
        public Sub sub { get; set; }
        public Dub dub { get; set; }
        public Raw raw { get; set; }
    }

    public class LastEpisodeInfo
    {
        public Sub sub { get; set; }
        public Dub dub { get; set; }
        public Raw raw { get; set; }
    }

    public class PageInfo
    {
        public int total { get; set; }
        public string __typename { get; set; }
    }

    public class Raw
    {
        public string episodeString { get; set; }
        public int? year { get; set; }
        public int? month { get; set; }
        public int? date { get; set; }
        public int? hour { get; set; }
        public int? minute { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    }

    public class Season
    {
        public string quarter { get; set; }
        public int year { get; set; }
    }

    public class Shows
    {
        public PageInfo pageInfo { get; set; }
        public List<Edge> edges { get; set; }
        public string __typename { get; set; }
    }

    public class Sub
    {
        public string episodeString { get; set; }
        public string notes { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int date { get; set; }
    }


}
