# WeebLib
refer to https://github.com/Charles-Ay/AniTool

<p align="center"><img src="https://capsule-render.vercel.app/api?type=soft&fontColor=FFA73E&text=Charles-Ay/WeebLib&height=170&fontSize=60&desc=For%20A%20Smarter%20Weeb&descAlignY=75&descAlign=28&color=00000000&animation=twinkling"></p>


<p align="center"><a href="https://github.com/Charles-Ay/WeebLib"><img src="https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg" height="30px"><img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg" height="30px"><img src="https://forthebadge.com/images/badges/built-with-love.svg" height="30px"></a></p>

### Basic Usage:

**Novel:** 

```C#
NovelFetcher fetcher = new();
fetcher.SetWorkDir("", false);
NovelSearcher searcher = new();
//Can also just do searcher.Search(1, "Naruto");
searcher.Search(1, "naruto", NovelUtil.sourceToString(NovelUtil.NovelSources.FreeWebNovel));

int novelIWant = 1;
int chapterIWant = 10;
int amountOfChaptersToFetch = 1;
bool outputToFile = false;

string text = fetcher.Fetch(searcher.Results()[novelIWant], chapterIWant, amountOfChaptersToFetch, outputToFile);
```

**Manga:** 

```C#
MangaFetcher fetcher = new();
MangaSearcher searcher = new();
//Can also just do searcher.Search(1, "Naruto");
searcher.Search(1, "Naruto", MangaUtil.sourceToString(MangaUtil.MangaSources.KissManga));

int mangaIWant = 1;
int chapterIWant = 10;
int amountOfChaptersToFetch = 1;
bool outputToFile = false;

string text = fetcher.Fetch(searcher.Results()[mangaIWant], chapterIWant, amountOfChaptersToFetch, outputToFile);
```
__Important to note that for now manga is always outputed to file__


#### Windows:

Only Supports Windows for now.

#### Linux:

Planned for the future or if anyone else wants to create a linux version.

#### Mac:

Gross. jkjk. I'm too poor to afford one so can't make a compatable version :(

### Core features

- Took longer than I thought it would
- The option to output LN to string or file
- Pretty light weight if you dont include the manga stuff
- Scrapes the Sources for novel links and request to raw HTML so it's up to date
- Integrates HtmlAgilityPack for efficent and effective parsing.
- Uses .NET 6 so it's not hard to port/integrate

### Supported Light Novel Sites

| Website                                      | Enum Name       |
| :------------------------------------------: | :-----------------: |
| [FreeWebNovel.com](https://FreeWebNovel.com/)| `FreeWebNovel`          |
| [Full-Novel.com](https://full-novel.com/)    | `FullNovel`        |

### Supported Manga Sites

| Website                                      | Enum Name       |
| :------------------------------------------: | :-----------------: |
| [KissManga.org](https://kissmanga.org/)      | `KissManga`          |

### Want More sites?

Yeah, ill add some more when I get around to it. You can raise a issue if you really want a specific one. Please try to make sure that the websites you request meets most of the following criteria (it's a headache to work with sites that don't and I don't want to add a webdriver as that add lots of overhead)
- Supports HTTP Request and doesnt hide the content inside obscure Java-Script **Cough, Cough** Every anime/manga site :()
- Should have some sort of search function(like a search bar)
- The chapters should have some logical ordering/naming. Not something like https://trashnovelsite.com/novel/32113/chapter-3. This makes it so much harder to get the right novel

**Note:** Your request may be denied in case of Cloudflare protections and powerful anti-bot scripts(ew).

### Disclaimer

The disclaimer of this project can be found [here.](./LEAGAL.md) (usual legal mobo-jumbo)