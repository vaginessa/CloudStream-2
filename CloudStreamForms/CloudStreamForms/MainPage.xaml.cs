using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Jint;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using static CloudStreamForms.Main;
//using Android.Util;
//using Android.Content;
using Xamarin.Essentials;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace CloudStreamForms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //public const int POSTER_WITH = 90;
        //public const int POSTER_HIGHT = 135;

        //public static int PosterAtScreenWith { get { return GetPostersWithAtCurrentScreen(); } }
        //  public static int PosterAtScreenHight { get { return GetPostersHightAtCurrentScreen(); } }

        public static bool initialized = false;
        public static string intentData = "";
        public static MainPage mainPage;



        /*
        public List<ImageButton> imageButtons = new List<ImageButton>();

        ImageButton CreateButton(string source = "")
        {
            return new ImageButton { Source = source, HeightRequest = POSTER_HIGHT, WidthRequest = POSTER_WITH, Margin = new Thickness() { Top = 0, Bottom = 0, Left = 0, Right = 0 }, Padding = new Thickness() { Top = 0, Bottom = 0, Left = 0, Right = 0 }, VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.Transparent };
        }*/

        public static void PushPageFromUrlAndName(string url, string name)
        {
            try {
                Poster _p = new Poster() { url = url, name = name };
                Search.PushPage(_p, mainPage.Navigation);
            }
            catch (Exception) {

            }

        }

        public MainPage()
        {
            InitializeComponent(); mainPage = this;
            /*
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                       delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                               System.Security.Cryptography.X509Certificates.X509Chain chain,
                                               System.Net.Security.SslPolicyErrors sslPolicyErrors)
                       {
                           return true; // **** Always accept
                       };*/
            //ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
            //  ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize
            Application.Current.PageAppearing += Current_PageAppearing;
            searchLoaded += MainPage_searchLoaded;
            CloudStreamForms.App.OBrowser += App_OBrowser;
            LoadSeachPage();

            //PushPageFromUrlAndName("tt0944947", "Game Of Thrones");

            //Page p = new MovieResult();
            //  Navigation.PushModalAsync(p);
            //imageButtons.Add(CreateButton(""));
            // GRID.Children.Add((ImageButton)imageButtons[imageButtons.Count - 1], 0, 0);

        }

        private void App_OBrowser(object sender, string e)
        {
            OpenBrowser(e);
        }

        public void LoadSeachPage()
        {
            Page p = new Search();
            Navigation.PushModalAsync(p);
            //NavigationPage.SetHasBackButton(p, true);

        }

        private void MainPage_searchLoaded(object sender, List<Poster> e)
        {
            /*
            MainThread.BeginInvokeOnMainThread(() => {
                foreach (var item in GRID.Children) {
                    Grid.SetColumn(item, 0);
                    Grid.SetRow(item, 0);
                }
                GRID.Children.Clear();

                imageButtons = new List<ImageButton>();
                for (int i = 0; i < e.Count; i++) {
                    string _url =  e[i].posterUrl;//.Replace("@@", "@").Replace("@._V1_.jpg", "@._CR0,0,0,0_UX402_UY596._SY298_SX201_AL_.jpg");
                    imageButtons.Add(CreateButton(_url));
                    print(i + ". " + e[i].posterUrl);
                    GRID.Children.Add((ImageButton)imageButtons[imageButtons.Count - 1], 20, 20);
                }

                MainPage_SizeChanged(null, null);

            });
            */



        }


        private void SeachBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuickSearch(e.NewTextValue);
        }

        private void Current_PageAppearing(object sender, Page e)
        {
            if (!initialized) {
                initialized = true;
                // Application.Current.MainPage.SizeChanged += MainPage_SizeChanged;


                /*
                Application.Current.MainPage.SizeChanged += MainPage_SizeChanged;
                MainPage_SizeChanged(null, null); */
            }
        }








        // -------------------------------- END --------------------------------

        public struct Vector2
        {
            public int x;
            public int y;
            public Vector2(int X, int Y)
            {
                x = X;
                y = Y;
            }

        }
        private void MainPage_SizeChanged(object sender, EventArgs e)
        {

            /*
            if (imageButtons.Count == 0 || !initialized) return;

            if (Device.OS == TargetPlatform.Windows) {
                GRID.RowSpacing = -POSTER_HIGHT - 5;
            }
            for (int i = 0; i < imageButtons.Count; i++) {
                try {
                    Grid.SetColumn(imageButtons[i], i % PosterAtScreenWith);
                    Grid.SetRow(imageButtons[i], (int)(i / PosterAtScreenWith));
                }
                catch (Exception) {
                    throw;
                }

            }*/


        }
        /*
        public static Vector2 GetRez()
        {
            int intHeight = (int)DeviceDisplay.MainDisplayInfo.Height;
            int intWidth = (int)DeviceDisplay.MainDisplayInfo.Width;

            if (Device.OS == TargetPlatform.Android) {
                bool reverse = intHeight < intWidth;

                int rows = reverse ? 5 : 3;
                float f = (float)(POSTER_WITH * rows) / (float)intWidth;
                intWidth = POSTER_WITH * rows;
                intHeight = (int)(intHeight * f);
            }
            else if (Device.OS == TargetPlatform.Windows) {
                intHeight = (int)Application.Current.MainPage.Height;
                intWidth = (int)Application.Current.MainPage.Width;
            }
            //intHeight = (int)Application.Current.MainPage.Height;
            //intWidth = (int)Application.Current.MainPage.Width;

            /*
#if __ANDROID__
         intHeight = (int)(App.Current.Resources["DisplayMetrics"].HeightPixels / App.Current.Resources["DisplayMetrics"].Density);
         intWidth = (int)(App.Current.Resources["DisplayMetrics"].WidthPixels / App.Current.Resources["DisplayMetrics"].Density);
#else
#if __IOS___
         intHeight = UIScreen.MainScreen.Bounds.Height; 
         intWidth = UIScreen.MainScreen.Bounds.Width;
#else
#if WINDOWS_PHONE_APP
         intHeight = Window.Current.Bounds.Height; 
         intWidth = Window.Current.Bounds.Width;
#else
                 intHeight = (int)Application.Current.MainPage.Height;
                 intWidth = (int)Application.Current.MainPage.Width;
#endif
#endif
#endif

            return new Vector2(intWidth, intHeight);
        }*/
        /*
        private static int GetPostersWithAtCurrentScreen()
        {


            return (int)((double)GetRez().x / (double)POSTER_WITH);

        }
        private static int GetPostersHightAtCurrentScreen()
        {
            //print(Application.Current.MainPage.Width + "|" + POSTER_WITH);

            // return (int)(Application.Current.MainPage.Height / (double)POSTER_HIGHT);
            return (int)((double)GetRez().y / (double)POSTER_HIGHT);

        }*/

        private void Button_Clicked(object sender, EventArgs e)
        {
            LoadSeachPage();
        }
    }

    public static class Main
    {
        // -------------------- CONSTS --------------------


        public const bool MOVIES_ENABLED = true;
        public const bool TVSERIES_ENABLED = true;
        public const bool ANIME_ENABLED = true;

        public const bool CHROMECAST_ENABLED = true;
        public const bool DOWNLOAD_ENABLED = true;
        public const bool SEARCH_FOR_UPDATES_ENABLED = true;

        public const bool INLINK_SUBTITLES_ENABLED = false;
        public const bool GLOBAL_SUBTITLES_ENABLED = false;
        public const bool GOMOSTEAM_ENABLED = true;
        public const bool SUBHDMIRROS_ENABLED = true;
        public const bool BAN_SUBTITLE_ADS = true;

        public const bool PLAY_SELECT_ENABLED = false;

        public const bool DEBUG_WRITELINE = true;
        public const string USERAGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
        public const int MIRROR_COUNT = 10;
        public const int HD_MIRROR_COUNT = 4;
        public const int ANIME_MIRRORS_COUNT = 1;

        public const bool REPLACE_IMDBNAME_WITH_POSTERNAME = true;

        // -------------------- ALL METHODS --------------------

        //public static string pathVLC = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";

        public static void OpenBrowser(string url)
        {
            print("Trying to open: " + url);
            if (CheckIfURLIsValid(url)) {
                try {
                    Launcher.OpenAsync(new Uri(url));
                }
                catch (Exception) {
                    print("BROWSER LOADED ERROR, SHOULD NEVER HAPPEND!!");
                }
            }
        }
        public static void PlayVLCWithSingleUrl(string url, string name = "", string subtitleLoc = "")
        {
            App.PlayVLCWithSingleUrl(url, name, subtitleLoc);

        }


        /// <summary>
        /// Get a shareble url of the current movie
        /// </summary>
        /// <param name="extra"></param>
        /// <param name="redirectingName"></param>
        /// <returns></returns>
        public static string ShareMovieCode(string extra, string redirectingName = "Redirecting to CloudStream 2")
        {
            const string baseUrl = "CloudStreamForms";
            //Because I don't want to host my own servers I "Save" a js code on a free js hosting site. This code will automaticly give a responseurl that will redirect to the CloudStream app.
            string code = ("var x = document.createElement('body');\n var s = document.createElement(\"script\");\n s.innerHTML = \"window.location.href = '" + baseUrl + ":" + extra + "';\";\n var h = document.createElement(\"H1\");\n var div = document.createElement(\"div\");\n div.style.width = \"100%\";\n div.style.height = \"100%\";\n div.align = \"center\";\n div.style.padding = \"130px 0\";\n div.style.margin = \"auto\";\n div.innerHTML = \"" + redirectingName + "\";\n h.append(div);\n x.append(h);\n x.append(s);\n parent.document.body = x;").Replace("%", "%25");
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("https://js.do/mod_perl/js.pl");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = "action=save_code&js_code=" + code + "&js_title=&js_permalink=&js_id=&is_update=false";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            // Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            reader.Close();
            dataStream.Close();
            response.Close();
            string rLink = "https://js.do/code/" + FindHTML(responseFromServer, "js_permalink\":\"", "\"");
            return rLink;
            // Clean up the streams.
        }





        // -------------------- THREDS --------------------


        public static List<int> activeThredIds = new List<int>();
        public static List<TempThred> tempThreds = new List<TempThred>();
        public static int thredNumber = 0; // UNIQUE THRED NUMBER FOR EATCH THREAD CREATED WHEN TEMPTHREDS THRED IS SET

        /// <summary>
        /// Same as thred.Join(); but removes refrense in tempThreds 
        /// </summary>
        /// <param name="tempThred"></param>
        public static void JoinThred(TempThred tempThred)
        {
            activeThredIds.Remove(tempThred.ThredId);
            GetThredActive(tempThred);
        }

        public static bool GetThredActive(TempThred tempThred, bool autoPurge = true)
        {
            bool active = activeThredIds.Contains(tempThred.ThredId);
            if (!active && autoPurge) { PurgeThred(tempThred); }
            return active;
        }

        public static void PurgeThred(TempThred tempThred)
        {
            try {
                // print("Purged: " + tempThred.ThredId);
                activeThredIds.Remove(tempThred.ThredId);
                tempThreds.Remove(tempThred);
                // print(tempThred.Thread.Name);
                try {

                    if (DeviceInfo.Platform == DevicePlatform.UWP) {
                        tempThred.Thread.Join();
                        tempThred.Thread.Abort();

                    }
                    else {
                        //tempThred.Thread.Join();

                    }

                }
                catch (Exception) {
                }


            }
            catch (Exception) {

            }

        }

        /// <summary>
        ///  THRED ID IS THE THREDS POURPOSE
        ///  0=Normal, 1=SEARCHTHRED, 2=GETTITLETHREAD, 3=LINKTHRED, 4=DOWNLOADTHRED, 5=TRAILERTHREAD, 6=EPISODETHREAD
        /// </summary>
        public static void PurgeThreds(int typeId)
        {
            print("PURGING ALL THREADS TYPE OF: " + typeId);
            if (typeId == -1) {
                activeThredIds = new List<int>();
                tempThreds = new List<TempThred>();
            }
            else {
                //  List<int> _activeThredIds = new List<int>();
                List<TempThred> _tempThreds = new List<TempThred>();
                foreach (TempThred t in tempThreds) {
                    if (t.typeId == typeId) {
                        //  PurgeThred(t);
                        _tempThreds.Add(t);
                    }
                    /*
                    if (t.typeId != typeId) {
                        _tempThreds.Add(t);
                        _activeThredIds.Add(t.ThredId);
                    }
                    else {
                        try {

                        }
                        catch (Exception) {

                            throw;
                        }
                        t.Thread.Abort();
                    }
                    */
                }
                for (int i = 0; i < _tempThreds.Count; i++) {
                    PurgeThred(_tempThreds[i]);
                }
                //activeThredIds = _activeThredIds;
                //  tempThreds = _tempThreds;
            }
        }

        public struct TempThred
        {
            /// <summary>
            /// THRED ID IS THE THREDS POURPOSE
            /// 0=Normal, 1=SEARCHTHRED, 2=GETTITLETHREAD, 3=LINKTHRED, 4=DOWNLOADTHRED, 5=TRAILERTHREAD, 6=EPISODETHREAD
            /// </summary>
            public int typeId;

            private int _thredId;
            /// <summary>
            /// THR ID IF THE THRED (UNIQUE)
            /// </summary>
            public int ThredId { private set { _thredId = value; } get { return _thredId; } }

            public System.Threading.Thread _thread;
            public System.Threading.Thread Thread
            {
                set {
                    if (_thread == null) {
                        thredNumber++; _thredId = thredNumber; activeThredIds.Add(_thredId); tempThreds.Add(this);
                    }
                    _thread = value;
                }
                get { return _thread; }
            }
        }

        // -------------------- MOVIES --------------------

        public enum MovieType { Movie, TVSeries, Anime, AnimeMovie }
        public enum PosterType { Imdb, Raw }



        public struct Trailer
        {
            public string name;
            public string url;
            public string posterUrl;
        }

        public struct Movies123
        {
            public string year;
            public string imdbRating;
            public string genre;
            public string plot;
            public string runtime;
            public string posterUrl;
            public string name;
            public MovieType type;
        }


        public struct MALSeason
        {
            public string name;
            public bool dubExists;
            public bool subExists;
            public string baseUrl;
            public string japName;
            public string engName;
        }


        public struct MALSeasonData
        {
            public List<MALSeason> seasons;
            public string malUrl;
        }

        public struct MALData
        {
            public string japName;
            public string engName;
            public List<MALSeasonData> seasonData;
            public bool done;
            public List<int> currentActiveMaxEpsPerSeason;
        }

        public struct Title
        {
            public string name;
            public string ogName;
            //public string altName;
            public string id;
            public string year;
            public string ogYear;
            public string rating;
            public string runtime;
            public string posterUrl;
            public string description;
            public int seasons;

            public MALData MALData;

            public MovieType movieType;
            public List<string> genres;
            public List<Trailer> trailers;
            public List<Poster> recomended;

            public Movies123MetaData movies123MetaData;
        }

        public struct Movies123MetaData
        {
            public string movieLink;
            public List<Movies123SeasonData> seasonData;
        }

        public struct Movies123SeasonData
        {
            public string seasonUrl;
            public List<string> episodeUrls;
        }

        public struct Poster
        {
            public string name;
            public string extra; // (Tv-Series) for exampe
            public string posterUrl;
            public string year;
            public string rank;

            //public int posterX;
            //public int posterY;

            //public string id; // IMDB ID

            public string url;
            public PosterType posterType; // HOW DID YOU GET THE POSTER, IMDB SEARCH OR SOMETHING ELSE
        }

        public struct Link
        {
            public string name;
            public string url;
            public int priority;
            //public string posterUrl;
            //public string description;
        }

        public struct Episode
        {
            public List<Link> links;
            public string name;
            public string rating;
            public string description;
            public string date;
            public string posterUrl;
            public int maxProgress;
            private int _progress;
            public int Progress { set { _progress = value; linkAdded?.Invoke(null, value); } get { return _progress; } }
        }

        public struct Subtitle
        {
            public string name;
            public string url;
        }

        public struct Movie
        {
            public Title title;
            // public List<Link> links;
            public List<Subtitle> subtitles;
            public List<Episode> episodes;
        }

        // -------------------- END --------------------

        /*
    static void Main(string[] args)
    {

        activeSearchResults = new List<Poster>();
        string searchFor = "Attack On Titan";
        QuickSearch(searchFor);
        print("Searching For: " + searchFor);
        searchLoaded += (o, e) =>
        {
            print("Loading Recomendations");
            GetImdbTitle(e.First());
        };
        titleLoaded += (o, e) =>
        {
            print("Loading Links");
            print(e.title.name);
            print(e.title.rating);
            for (int i = 0; i < e.title.recomended.Count; i++)
            {
                print(e.title.recomended[i].name);
            }
            GetImdbEpisodes();
                //print("NAME: " + e.title.name + "|" + e.title.trailers.First().url);
            };
        trailerLoaded += (o, e) =>
        {
            print(e);
        };
        epsiodesLoaded += (o, e) =>
        {
            GetEpisodeLink(1);
        };
        linkAdded += (o, e) =>
        {
            print(e / 14f);
        };

        bool done = false;
        while (!done)
        {
            try
            {
                float progress = activeMovie.episodes[1].Progress / (float)activeMovie.episodes[1].maxProgress;
                //print(progress);
                done = progress == 1;
            }
            catch (Exception)
            {

            }
        }
        foreach (var item in activeMovie.episodes[1].links)
        {
            print("");
            print(item.name + " | " + item.url);
            print("");
        }
        Console.ReadKey();
    }
    */

        // -------------------- SEARCH --------------------

        public static List<Poster> activeSearchResults = new List<Poster>();
        public static Movie activeMovie = new Movie();
        public static string activeTrailer = "";

        public static event EventHandler<Poster> addedSeachResult;
        public static event EventHandler<Movie> titleLoaded;
        public static event EventHandler<List<Poster>> searchLoaded;
        public static event EventHandler<string> trailerLoaded;
        public static event EventHandler<List<Episode>> epsiodesLoaded;
        public static event EventHandler<int> linkAdded;
        public static event EventHandler<MALData> malDataLoaded;
        public static event EventHandler<Episode> linksProbablyDone;
        public static event EventHandler<Movie> movie123FishingDone;


        // public static int tempInt = 0;
        public static int linksDone = 0;

        public static void QuickSearch(string text, bool purgeCurrentSearchThread = true, bool onlySearch = true)
        {

            if (purgeCurrentSearchThread) {
                PurgeThreds(1);
            }

            TempThred tempThred = new TempThred();

            tempThred.typeId = 1;

            tempThred.Thread = new System.Threading.Thread(() => {
                try {
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    text = rgx.Replace(text, "").ToLower();
                    if (text == "") {
                        return;
                    }
                    string qSearchLink = "https://v2.sg.media-imdb.com/suggestion/" + text.Substring(0, 1) + "/" + text.Replace(" ", "_") + ".json";
                    string result = DownloadString(qSearchLink, tempThred);
                    //print(qSearchLink+ "|" +result);
                    string lookFor = "{\"i\":{\"";

                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                    activeSearchResults = new List<Poster>();

                    int counter = 0;
                    while (result.Contains(lookFor) && counter < 100) {
                        counter++;
                        string name = ReadJson(result, "\"l");
                        string posterUrl = ReadJson(result, "imageUrl");
                        string extra = ReadJson(result, "\"q");
                        string year = FindHTML(result, "\"y\":", "}"); string oyear = year;
                        string years = FindHTML(year, "yr\":\"", "\""); if (years.Length > 4) { year = years; }
                        string id = ReadJson(result, "\"id");
                        string rank = FindHTML(result, "rank\":", ",");
                        if (extra == "feature") { extra = ""; }

                        if (year != "" && id.StartsWith("tt") && !extra.Contains("video game")) {
                            AddToActiveSearchResults(new Poster() { name = name, posterUrl = posterUrl, extra = extra, year = year, rank = rank, url = id, posterType = PosterType.Imdb });
                        }
                        result = RemoveOne(result, "y\":" + oyear);
                    }

                    if (onlySearch) {
                        if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                        searchLoaded?.Invoke(null, activeSearchResults);
                    }
                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "QuickSearch";
            tempThred.Thread.Start();
        }

        public static void GetMALData()
        {
            TempThred tempThred = new TempThred();
            tempThred.typeId = 2; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {

                    string year = activeMovie.title.year.Substring(0, 4); // this will not work in 8000 years time :)
                    string _d = DownloadString("https://myanimelist.net/search/prefix.json?type=anime&keyword=" + activeMovie.title.name, tempThred);
                    string url = "";
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    string lookFor = "\"name\":\"";
                    bool done = false;
                    string currentSelectedYear = "";
                    while (_d.Contains(lookFor) && !done) { // TO FIX MY HERO ACADIMEA CHOOSING THE SECOND SEASON BECAUSE IT WAS FIRST SEARCHRESULT
                        string name = FindHTML(_d, lookFor, "\"");
                        string _url = FindHTML(_d, "url\":\"", "\"").Replace("\\/", "/");
                        string startYear = FindHTML(_d, "start_year\":", ",");
                        string aired = FindHTML(_d, "aired\":\"", "\"");
                        string _aired = FindHTML(aired, ", ", " ", readToEndOfFile: true);
                        if (!name.Contains(" Season") && year == _aired) {
                            url = _url;
                            done = true;
                            currentSelectedYear = _aired;
                        }
                        _d = RemoveOne(_d, lookFor);
                        _d = RemoveOne(_d, "\"id\":");
                    }

                    /*

                    string d = DownloadString("https://myanimelist.net/search/all?q=" + activeMovie.title.name);

                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                    d = RemoveOne(d, " <div class=\"picSurround di-tc thumb\">"); // DONT DO THIS USE https://myanimelist.net/search/prefix.json?type=anime&keyword=my%20hero%20acadimea
                    string url = "";//"https://myanimelist.net/anime/" + FindHTML(d, "<a href=\"https://myanimelist.net/anime/", "\"");
                    */

                    if (url == "") return;

                    WebClient webClient = new WebClient();
                    webClient.Encoding = Encoding.UTF8;

                    string d = webClient.DownloadString(url);
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    string jap = FindHTML(d, "Japanese:</span> ", "<").Replace("  ", "").Replace("\n", ""); // JAP NAME IS FOR SEARCHING, BECAUSE ALL SEASONS USE THE SAME NAME
                    string eng = FindHTML(d, "English:</span> ", "<").Replace("  ", "").Replace("\n", "");

                    string currentName = FindHTML(d, "<span itemprop=\"name\">", "<");
                    List<MALSeasonData> data = new List<MALSeasonData>() { new MALSeasonData() { malUrl = url, seasons = new List<MALSeason>() } };

                    string sqlLink = "-1";

                    // ----- GETS ALL THE SEASONS OF A SHOW WITH MY ANIME LIST AND ORDERS THEM IN THE CORRECT SEASON (BOTH Shingeki no Kyojin Season 3 Part 2 and Shingeki no Kyojin Season 3 is season 3) -----
                    while (sqlLink != "") {
                        string _malLink = (sqlLink == "-1" ? url.Replace("https://myanimelist.net", "") : sqlLink);
                        currentName = FindHTML(d, "<span itemprop=\"name\">", "<");
                        string sequel = FindHTML(d, "Sequel:", "</a></td>") + "<";
                        sqlLink = FindHTML(sequel, "<a href=\"", "\"");
                        string _jap = FindHTML(d, "Japanese:</span> ", "<").Replace("  ", "").Replace("\n", "");
                        string _eng = FindHTML(d, "English:</span> ", "<").Replace("  ", "").Replace("\n", "");

                        if (currentName.Contains("Part ") && !currentName.Contains("Part 1")) // WILL ONLY WORK UNTIL PART 10, BUT JUST HOPE THAT THAT DOSENT HAPPEND :)
                        {
                            data[data.Count - 1].seasons.Add(new MALSeason() { name = currentName });
                        }
                        else {
                            data.Add(new MALSeasonData() {
                                seasons = new List<MALSeason>() { new MALSeason() { name = currentName, engName = _eng, japName = _jap } },
                                malUrl = "https://myanimelist.net" + _malLink
                            });
                        }
                        if (sqlLink != "") {
                            try {
                                d = webClient.DownloadString("https://myanimelist.net" + sqlLink);
                            }
                            catch (Exception) {
                                d = "";
                            }
                        }
                    }
                    activeMovie.title.MALData = new MALData() {
                        seasonData = data,
                        japName = jap,
                        engName = eng,
                        done = false,
                    };
                    if (activeMovie.title.MALData.japName != "error") {
                        d = DownloadString("https://www9.gogoanime.io/search.html?keyword=" + activeMovie.title.MALData.japName, tempThred);
                        string look = "<p class=\"name\"><a href=\"/category/";


                        while (d.Contains(look)) {
                            string ur = FindHTML(d, look, "\"").Replace("-dub", "");
                            string adv = FindHTML(d, look, "</a");
                            string title = FindHTML(adv, "title=\"", "\"").Replace(" (TV)", ""); // TO FIX BLACK CLOVER
                            string _title = title.Replace(" (Dub)", "");
                            string __d = RemoveOne(d, look);
                            string __year = FindHTML(__d, "Released: ", " ");
                            int ___year = int.Parse(__year);
                            int ___year2 = int.Parse(currentSelectedYear);
                            if (___year >= ___year2) {
                                for (int i = 0; i < activeMovie.title.MALData.seasonData.Count; i++) {
                                    for (int q = 0; q < activeMovie.title.MALData.seasonData[i].seasons.Count; q++) {
                                        MALSeason ms = activeMovie.title.MALData.seasonData[i].seasons[q];
                                        print(_title + "|" + ms.name + "|" + ___year + "__" + ___year2 + "|");

                                        if (ms.name == _title) {

                                            if (_title == title) {
                                                activeMovie.title.MALData.seasonData[i].seasons[q] = new MALSeason() { name = ms.name, baseUrl = ur, subExists = true, dubExists = ms.dubExists, japName = ms.japName, engName = ms.engName };
                                            }
                                            else {
                                                activeMovie.title.MALData.seasonData[i].seasons[q] = new MALSeason() { name = ms.name, baseUrl = ur, dubExists = true, subExists = ms.subExists, japName = ms.japName, engName = ms.engName };
                                            }
                                        }
                                    }
                                }
                            }
                            d = d.Substring(d.IndexOf(look) + 1, d.Length - d.IndexOf(look) - 1);
                        }
                        for (int i = 0; i < activeMovie.title.MALData.seasonData.Count; i++) {
                            for (int q = 0; q < activeMovie.title.MALData.seasonData[i].seasons.Count; q++) {
                                MALSeason ms = activeMovie.title.MALData.seasonData[i].seasons[q];

                                if (ms.dubExists) {
                                    print(i + ". " + ms.name + " | Dub E" + ms.baseUrl);
                                }
                                if (ms.subExists) {
                                    print(i + ". " + ms.name + " | Sub E" + ms.baseUrl);
                                }
                            }
                        }

                    }

                    MALData md = activeMovie.title.MALData;

                    activeMovie.title.MALData = new MALData() {
                        seasonData = md.seasonData,
                        japName = md.japName,
                        done = true,
                    };
                    //print(sequel + "|" + realSquel + "|" + sqlLink);

                }
                catch {
                    activeMovie.title.MALData.japName = "error";
                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "MAL Search";
            tempThred.Thread.Start();
        }

        /*   WebClient client = new WebClient();
                    string d = "";
                    try {
                        d = client.DownloadString("https://www3.gogoanime.io/category/" + fwordLink[titleID]);

                    }
                    catch (System.Exception) {

                    }
                    if (d != "") {
                        string id = FindHTML(d, "<input type=\"hidden\" value=\"", "\"");
                        d = client.DownloadString("https://ajax.apimovie.xyz/ajax/load-list-episode?ep_start=0&ep_end=100000&id=" + id + "&default_ep=0");
                        //print(d);
                        // Console.ReadLine();
                        string fS = "<li><a href=\" ";
                        List<string> _links = new List<string>();
                        while (d.Contains(fS)) {
                            string _link = FindHTML(d, fS, "\"");
                            _links.Add(_link);
                            d = d.Substring(d.IndexOf(fS) + 1, d.Length - 1 - d.IndexOf(fS));
                        }
                        if (onlyEps) {
                            addToTitleEps = _links.Count;
                        }
                        else {
                            for (int i = 0; i < _links.Count; i++) {
                                string title = ToTitle(_links[i]);

                                d = client.DownloadString("https://www3.gogoanime.io" + _links[i]);
                                string mp4 = "https://www.mp4upload.com/embed-" + FindHTML(d, "data-video=\"https://www.mp4upload.com/embed-", "\"");
                                if (mp4 == "https://www.mp4upload.com/embed-") {
                                    mp4 = FindHTML(d, "data-video=\"//vidstreaming.io/streaming.php?", "\"");
                                    if (mp4 != "") {
                                        mp4 = "http://vidstreaming.io/streaming.php?" + mp4;
                                        d = client.DownloadString(mp4);
                                        string mxLink = FindHTML(d, "sources:[{file: \'", "\'");
                                        print("Backup: " + title + " | Browser: " + mp4 + " | RAW (NO ADS): " + mxLink);
                                        if (!activeLinks.Contains(mxLink)) {
                                            print("---------------------------------" + cThred + " | " + thredNumber + "--------------------------------------------");

                                            if (cThred != thredNumber) return;
                                            if (!mxLink.StartsWith("Error")) {
                                                activeLinks.Add(mxLink);
                                                activeLinksNames.Add(title);
                                                progress = (int)System.Math.Round((100f * i) / _links.Count);
                                                ax_Links.ax_links.ChangeBar(progress);
                                            }
                                            else {
                                                print("vidstreaming mx link starts w error");
                                            }
                                        }
                                    }
                                    else {
                                        print(title + " | Error :(");
                                    }
                                }
                                else {

                                    try {
                                        d = client.DownloadString(mp4);
                                        string mxLink = Getmp4uploadByFile(d);
                                        print(title + " | BrowserMp4: " + mp4 + " | DownloadMp4: " + mxLink);
                                        if (!activeLinks.Contains(mxLink)) {
                                            print("---------------------------------" + cThred + " | " + thredNumber + "--------------------------------------------");

                                            if (cThred != thredNumber) return;
                                            activeLinks.Add(mxLink);
                                            activeLinksNames.Add(title);
                                            progress = (int)System.Math.Round((100f * i) / _links.Count);
                                            ax_Links.ax_links.ChangeBar(progress);
                                        }
                                    }
                                    catch (System.Exception) {
                                        print(title + " | BrowserMp4: " + mp4);

                                    }
                                }

                            }
                        }
                    }*/
        public static void GetImdbTitle(Poster imdb, bool purgeCurrentTitleThread = true, bool autoSearchTrailer = true)
        {
            if (purgeCurrentTitleThread) {
                PurgeThreds(2);
            }
            activeMovie = new Movie();
            // TurnNullMovieToActive(movie);
            TempThred tempThred = new TempThred();
            tempThred.typeId = 2; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {

                    string url = "https://imdb.com/title/" + imdb.url.Replace("https://imdb.com/title/", "") + "/";
                    string d = GetHTML(url); // DOWNLOADSTRING WILL GET THE LOCAL LAUNGEGE, AND NOT EN, THAT WILL MESS WITH RECOMENDATIONDS, GetHTML FIXES THAT
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS


                    List<string> keyWords = new List<string>();
                    string _d = DownloadString(url + "keywords", tempThred);
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                    string _lookFor = "data-item-keyword=\"";
                    while (_d.Contains(_lookFor)) {
                        keyWords.Add(FindHTML(_d, _lookFor, "\""));
                        _d = RemoveOne(_d, _lookFor);
                    }
                    for (int i = 0; i < keyWords.Count; i++) {
                        // print("Keyword: " + keyWords[i]);
                    }

                    if (d != "") {
                        // ------ THE TITLE ------

                        try {
                            // ----- GET -----

                            int seasons = 0; // d.Count<string>("");
                            for (int i = 1; i <= 100; i++) {
                                if (d.Contains("episodes?season=" + i)) {
                                    seasons = i;
                                }
                            }
                            string result = FindHTML(d, "<div class=\"title_wrapper\">", "</a>            </div>");
                            string descript = FindHTML(d, "\"description\": \"", "\"");
                            string ogName = FindHTML(d, "\"name\": \"", "\"");
                            string rating = FindHTML(d, "\"ratingValue\": \"", "\"");
                            string posterUrl = FindHTML(d, "\"image\": \"", "\"");
                            string genres = FindHTML(d, "\"genre\": [", "]");
                            string type = FindHTML(d, "@type\": \"", "\"");
                            string _trailer = FindHTML(d, "\"trailer\": ", "uploadDate");
                            string trailerUrl = "https://imdb.com" + FindHTML(_trailer, "\"embedUrl\": \"", "\"");
                            string trailerImg = FindHTML(_trailer, "\"thumbnailUrl\": \"", "\"");
                            string trailerName = FindHTML(_trailer, "\"name\": \"", "\"");
                            string keyWord = FindHTML(d, "\"keywords\": \"", "\"");
                            string duration = FindHTML(d, "<time datetime=\"PT", "\"").Replace("M", "min");
                            string year = FindHTML(d, "datePublished\": \"", "-");

                            //<span class="bp_sub_heading">66 episodes</span> //total episodes

                            List<string> allGenres = new List<string>();
                            int counter = 0;
                            while (genres.Contains("\"") && counter < 20) {
                                counter++;
                                string genre = FindHTML(genres, "\"", "\"");
                                allGenres.Add(genre);
                                genres = genres.Replace("\"" + genre + "\"", "");
                            }

                            MovieType movieType = (!keyWords.Contains("anime") ? (type == "Movie" ? MovieType.Movie : MovieType.TVSeries) : (type == "Movie" ? MovieType.AnimeMovie : MovieType.Anime)); // looks ugly but works

                            // ----- SET -----
                            activeMovie.title = new Title() {
                                name = REPLACE_IMDBNAME_WITH_POSTERNAME ? imdb.name : ogName,
                                posterUrl = posterUrl,
                                trailers = new List<Trailer>(),
                                rating = rating,
                                genres = allGenres,
                                id = imdb.url.Replace("https://imdb.com/title/", ""),
                                description = descript,
                                runtime = duration,
                                seasons = seasons,
                                MALData = new MALData() { japName = "", seasonData = new List<MALSeasonData>() },
                                movieType = movieType,
                                year = year,
                                ogName = ogName,
                            };

                            activeMovie.title.trailers.Add(new Trailer() { url = trailerUrl, posterUrl = trailerImg, name = trailerName });
                            if (movieType == MovieType.Anime) {
                                print("GEt mal data");
                                GetMALData();
                            }
                            else {
                                FishMovies123Links();
                            }

                            if (autoSearchTrailer) { GetRealTrailerLinkFromImdb(trailerUrl); }
                        }
                        catch (Exception) {

                        }

                        // ------ RECOMENDATIONS ------

                        activeMovie.title.recomended = new List<Poster>();
                        string lookFor = "<div class=\"rec_item\" data-info=\"\" data-spec=\"";
                        for (int i = 0; i < 12; i++) {
                            try {
                                string result = FindHTML(d, lookFor, "/> <br/>");
                                string id = FindHTML(result, "data-tconst=\"", "\"");
                                string name = FindHTML(result, "title=\"", "\"");
                                string posterUrl = FindHTML(result, "loadlate=\"", "\"");

                                d = RemoveOne(d, result);
                                Poster p = new Poster() { url = id, name = name, posterUrl = posterUrl, posterType = PosterType.Imdb };

                                // if (!activeMovie.title.recomended.Contains(p)) {
                                activeMovie.title.recomended.Add(p);
                                // }

                            }
                            catch (Exception) {

                            }
                        }

                        titleLoaded?.Invoke(null, activeMovie);
                    }
                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "Imdb Recomended";
            tempThred.Thread.Start();
        }


        public static void MonitorFunc(Action a, int sleepTime = 100)
        {
            Thread t = new Thread(() => {

                while (true) {
                    a();
                    Thread.Sleep(sleepTime);
                }

            }) {
                Name = "MonitorFunc"
            };
            t.Start();
        }

        public static void FishMovies123Links() // TO MAKE LINK EXTRACTION EASIER
        {
            TempThred tempThred = new TempThred();
            tempThred.typeId = 2; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {
                    if (activeMovie.title.movieType == MovieType.Anime) { return; }

                    bool canMovie = GetSettings(MovieType.Movie);
                    bool canShow = GetSettings(MovieType.TVSeries);

                    string rinput = ToDown(activeMovie.title.name, replaceSpace: "+");
                    string movies123 = "https://movies123.pro/search/" + rinput.Replace("+", "%20") + ((activeMovie.title.movieType == MovieType.Movie || activeMovie.title.movieType == MovieType.AnimeMovie) ? "/movies" : "/series");

                    string d = DownloadString(movies123, tempThred);
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    int counter = 0; // NOT TO GET STUCK, JUST IN CASE

                    List<Movies123SeasonData> seasonData = new List<Movies123SeasonData>();

                    while ((d.Contains("/movie/") || d.Contains("/tv-series/")) && counter < 100) {
                        counter++;

                        /*
                        data - filmName = "Iron Man"
                    data - year = "2008"
                    data - imdb = "IMDb: 7.9"
                    data - duration = "126 min"
                    data - country = "United States"
                    data - genre = "Action, Adventure, Sci-Fi"
                    data - descript = "Tony a boss of a Technology group, after his encounter in Afghanistan, became a symbol of justice as he built High-Tech armors and suits, to act as..."
                    data - star_prefix = ""
                    data - key = "0"
                    data - quality = "itemAbsolute_hd"
                    data - rating = "4.75"
                            */

                        // --------- GET TYPE ---------

                        int tvIndex = d.IndexOf("/tv-series/");
                        int movieIndex = d.IndexOf("/movie/");
                        bool isMovie = movieIndex < tvIndex;
                        if (tvIndex == -1) { isMovie = true; }
                        if (movieIndex == -1) { isMovie = false; }

                        Movies123 movie123 = new Movies123();

                        // --------- GET CROSSREFRENCE DATA ---------

                        movie123.year = ReadDataMovie(d, "data-year");
                        movie123.imdbRating = ReadDataMovie(d, "data-imdb").ToLower().Replace(" ", "").Replace("imdb:", "");
                        movie123.runtime = ReadDataMovie(d, "data-duration").Replace(" ", "");
                        movie123.genre = ReadDataMovie(d, "data-genre");
                        movie123.plot = ReadDataMovie(d, "data-descript");
                        movie123.type = isMovie ? MovieType.Movie : MovieType.TVSeries; //  "movie" : "tv-series";

                        string lookfor = isMovie ? "/movie/" : "/tv-series/";

                        // --------- GET FWORLDLINK, FORWARLINK ---------

                        int mStart = d.IndexOf(lookfor);
                        if (mStart == -1) {
                            debug("API ERROR!");
                            // print(mD);
                            debug(movie123.year + "|" + movie123.imdbRating + "|" + isMovie + "|" + lookfor);
                            continue;
                        }
                        d = d.Substring(mStart, d.Length - mStart);
                        d = d.Substring(7, d.Length - 7);
                        //string bMd = RemoveOne(mD, "<img src=\"/dist/image/default_poster.jpg\"");
                        movie123.posterUrl = ReadDataMovie(d, "<img src=\"/dist/image/default_poster.jpg\" data-src");




                        string rmd = lookfor + d;
                        //string realAPILink = mD.Substring(0, mD.IndexOf("-"));
                        string fwordLink = "https://movies123.pro" + rmd.Substring(0, rmd.IndexOf("\""));
                        if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                        if (!isMovie) {
                            fwordLink = rmd.Substring(0, rmd.IndexOf("\"")); // /tv-series/ies/the-orville-season-2/gMSTqyRs
                            fwordLink = fwordLink.Substring(11, fwordLink.Length - 11); //ies/the-orville-season-2/gMSTqyRs
                            string found = fwordLink.Substring(0, fwordLink.IndexOf("/"));
                            if (!found.Contains("-")) {
                                fwordLink = fwordLink.Replace(found, ""); //the-orville-season-2/gMSTqyRs
                            }
                            fwordLink = "https://movies123.pro" + "/tv-series" + fwordLink;
                        }

                        // --------- GET NAME ECT ---------
                        //if (false) {
                        int titleStart = d.IndexOf("title=\"");
                        string movieName = d.Substring(titleStart + 7, d.Length - titleStart - 7);
                        movieName = movieName.Substring(0, movieName.IndexOf("\""));
                        movieName = movieName.Replace("&amp;", "and");
                        movie123.name = movieName;
                        //}

                        if ((isMovie && canMovie) || (!isMovie && canShow)) {
                            //FWORDLINK HERE
                            //   print(activeMovie.title.name + "||||" + movie123.name + " : " + activeMovie.title.rating + " : " + movie123.imdbRating + " : " + activeMovie.title.movieType + " : " + movie123.type + " : " + activeMovie.title.runtime + " : " + movie123.runtime);

                            // GET RATING IN INT (10-100)
                            if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                            string s1 = activeMovie.title.rating;
                            string s2 = movie123.imdbRating;
                            if (s2.ToLower() == "n/a") {
                                continue;
                            }

                            if (!s1.Contains(".")) { s1 += ".0"; }
                            if (!s2.Contains(".")) { s2 += ".0"; }

                            int i1 = int.Parse(s1.Replace(".", ""));
                            int i2 = int.Parse(s2.Replace(".", ""));

                            if ((i1 == i2 || i1 == i2 - 1 || i1 == i2 + 1) && activeMovie.title.movieType == movie123.type && movie123.name.ToLower().Contains(activeMovie.title.name.ToLower())) { // --- THE SAME ---
                                                                                                                                                                                                    // counter = 10000;
                                                                                                                                                                                                    //print("FWORDLINK: " + fwordLink);
                                if (activeMovie.title.movieType == MovieType.TVSeries) {
                                    //<a data-ep-id="
                                    string _d = DownloadString(fwordLink, tempThred);
                                    string _lookFor = "<a data-ep-id=\"";
                                    //print(_d);
                                    List<string> sData = new List<string>();
                                    while (_d.Contains(_lookFor)) {
                                        string rLink = FindHTML(_d, _lookFor, "\"");
                                        //   print("RLINK: " + rLink);
                                        sData.Add(rLink + "-watch-free.html");
                                        _d = RemoveOne(_d, _lookFor);
                                    }
                                    seasonData.Add(new Movies123SeasonData() { seasonUrl = fwordLink, episodeUrls = sData });
                                }
                                else {
                                    activeMovie.title.movies123MetaData = new Movies123MetaData() { movieLink = fwordLink, seasonData = new List<Movies123SeasonData>() };
                                }

                            }
                        }
                    }

                    seasonData.Reverse();
                    if (MovieType.TVSeries == activeMovie.title.movieType) {
                        Title t = activeMovie.title;
                        activeMovie.title = new Title() {
                            description = t.description,
                            MALData = t.MALData,
                            genres = t.genres,
                            id = t.id,
                            movieType = t.movieType,
                            name = t.name,
                            ogName = t.ogName,
                            ogYear = t.ogYear,
                            posterUrl = t.posterUrl,
                            rating = t.rating,
                            recomended = t.recomended,
                            runtime = t.runtime,
                            seasons = t.seasons,
                            trailers = t.trailers,
                            year = t.year,
                            movies123MetaData = new Movies123MetaData() { movieLink = "", seasonData = seasonData },
                        };
                    }

                    movie123FishingDone?.Invoke(null, activeMovie);

                    // MonitorFunc(() => print(">>>" + activeMovie.title.movies123MetaData.seasonData.Count),0);
                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "Movies123MetaData";
            tempThred.Thread.Start();


        }
        public static void GetRealTrailerLinkFromImdb(string url, bool purgeCurrentTrailerThread = true)
        {
            if (purgeCurrentTrailerThread) {
                PurgeThreds(5);
            }
            TempThred tempThred = new TempThred();
            tempThred.typeId = 5; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {
                    url = url.Replace("video/imdb", "videoplayer");
                    string d = GetHTML(url);
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    string realTrailerUrl = FindHTML(d, "videoUrl\":\"", "\"");

                    for (int i = 0; i < 10; i++) {
                        realTrailerUrl = RemoveOne(realTrailerUrl, "\\u002F");
                    }
                    try {
                        realTrailerUrl = realTrailerUrl.Substring(5, realTrailerUrl.Length - 5);
                        realTrailerUrl = ("https://imdb-video.media-imdb.com/" + (url.Substring(url.IndexOf("/vi") + 1, url.Length - url.IndexOf("/vi") - 1)) + "/" + realTrailerUrl).Replace("/videoplayer", "");
                        activeTrailer = realTrailerUrl;
                        trailerLoaded?.Invoke(null, realTrailerUrl);
                    }
                    catch (Exception) {

                    }

                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "Trailer";
            tempThred.Thread.Start();
        }

        public static void GetImdbEpisodes(int season = 1, bool purgeCurrentSeasonThread = true)
        {
            if (purgeCurrentSeasonThread) {
                PurgeThreds(6);
            }
            if (activeMovie.title.movieType == MovieType.Anime || activeMovie.title.movieType == MovieType.TVSeries) {
                TempThred tempThred = new TempThred();
                tempThred.typeId = 6; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        string url = "https://www.imdb.com/title/" + activeMovie.title.id + "/episodes?season=" + season;
                        string d = DownloadString(url, tempThred);
                        if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                        int eps = 0;

                        for (int q = 0; q < 1000; q++) {
                            if (d.Contains("?ref_=ttep_ep" + q)) {
                                eps = q;
                            }
                        }
                        if (activeMovie.title.movieType == MovieType.Anime) {
                            while (!activeMovie.title.MALData.done) {
                                Thread.Sleep(100);
                                if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                            }
                            //string _d = DownloadString("");
                        }

                        activeMovie.episodes = new List<Episode>();

                        for (int q = 1; q <= eps; q++) {
                            string lookFor = "?ref_=ttep_ep" + q;
                            try {
                                d = d.Substring(d.IndexOf(lookFor), d.Length - d.IndexOf(lookFor));
                                string name = FindHTML(d, "title=\"", "\"");
                                string rating = FindHTML(d, "<span class=\"ipl-rating-star__rating\">", "<");
                                string descript = FindHTML(d, "<div class=\"item_description\" itemprop=\"description\">", "<").Replace("\n", "").Replace("  ", "");
                                string date = FindHTML(d, "<div class=\"airdate\">", "<").Replace("\n", "").Replace("  ", "");
                                string posterUrl = FindHTML(d, "src=\"", "\"");

                                if (descript != "Know what this is about?") {
                                    activeMovie.episodes.Add(new Episode() { date = date, name = name, description = descript, rating = rating, posterUrl = posterUrl });
                                }
                            }
                            catch (Exception) {

                            }
                        }
                        //print(activeMovie.title.MALData.japName + "<<<<<<<<<<<<<<<<<<<<<<<<");
                        //     https://www9.gogoanime.io/category/mix-meisei-story

                        epsiodesLoaded?.Invoke(null, activeMovie.episodes);
                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Season Info";
                tempThred.Thread.Start();
            }
            else {
                Episode ep = new Episode() { name = activeMovie.title.name };
                activeMovie.episodes = new List<Episode>();
                activeMovie.episodes.Add(ep);
                epsiodesLoaded?.Invoke(null, activeMovie.episodes);
            }
        }

        /// <summary>
        /// RETURN SUBTITLE STRING
        /// </summary>
        /// <param name="imdbTitleId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string DownloadSubtitle(string imdbTitleId, string lang = "eng")
        {
            try {
                string rUrl = "https://www.opensubtitles.org/en/search/sublanguageid-" + lang + "/imdbid-" + imdbTitleId + "/sort-7/asc-0"; // best match first
                string d = DownloadString(rUrl);
                string _url = "https://www.opensubtitles.org/" + lang + "/subtitles/" + FindHTML(d, "en/subtitles/", "\'");

                d = DownloadString(_url);
                const string subAdd = "https://dl.opensubtitles.org/en/download/file/";
                string subtitleUrl = subAdd + FindHTML(d, "download/file/", "\"");
                if (subtitleUrl != subAdd) {
                    string s = HTMLGet(subtitleUrl, "https://www.opensubtitles.org");
                    if (BAN_SUBTITLE_ADS) {
                        List<string> bannedLines = new List<string>() { "Support us and become VIP member", "to remove all ads from www.OpenSubtitles.org", "to remove all ads from OpenSubtitles.org", "Advertise your product or brand here", "contact www.OpenSubtitles.org today" }; // No advertisement
                        foreach (var banned in bannedLines) {
                            s = s.Replace(banned, "");
                        }
                    }
                    s = s.Replace("\n\n", "");
                    return s;
                }
                else {
                    return "";
                }
            }
            catch (Exception) {
                return "";
            }
        }

        public static List<string> GetAllEpsFromAnime(Movie currentMovie, int currentSeason, bool isDub)
        {
            List<string> baseUrls = new List<string>();

            try {
                for (int q = 0; q < currentMovie.title.MALData.seasonData[currentSeason].seasons.Count; q++) {
                    MALSeason ms = currentMovie.title.MALData.seasonData[currentSeason].seasons[q];

                    if ((ms.dubExists && isDub) || (ms.subExists && !isDub)) {
                        //  dstring = ms.baseUrl;
                        string burl = ms.baseUrl;
                        if (!baseUrls.Contains(burl)) {
                            baseUrls.Add(burl);
                        }
                        //print("BASEURL " + ms.baseUrl);
                    }
                }
            }
            catch (Exception) {
            }
            return baseUrls;
        }

        public static void GetEpisodeLink(int episode = -1, int season = 1, bool purgeCurrentLinkThread = true, bool onlyEpsCount = false, bool isDub = true)
        {
            if (purgeCurrentLinkThread) {
                PurgeThreds(3);
            }

            TempThred tempThred = new TempThred();

            tempThred.typeId = 3; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {

                    string rinput = ToDown(activeMovie.title.name, replaceSpace: "+"); // THE URL SEARCH STRING

                    bool animeSeach = activeMovie.title.movieType == MovieType.Anime && ANIME_ENABLED; // || activeMovie.title.movieType == MovieType.AnimeMovie &&
                    bool movieSearch = activeMovie.title.movieType == MovieType.Movie || activeMovie.title.movieType == MovieType.AnimeMovie || activeMovie.title.movieType == MovieType.TVSeries;

                    int maxProgress = 0;
                    if (movieSearch) { maxProgress += MIRROR_COUNT + HD_MIRROR_COUNT; }
                    if (animeSeach) { maxProgress += ANIME_MIRRORS_COUNT; }

                    // --------- CLEAR EPISODE ---------
                    int normalEpisode = episode == -1 ? 0 : episode - 1;                     //normalEp = ep-1;

                    if (activeMovie.episodes.Count <= normalEpisode) { activeMovie.episodes.Add(new Episode()); }
                    Episode cEpisode = activeMovie.episodes[normalEpisode];
                    activeMovie.episodes[normalEpisode] = new Episode() {
                        links = new List<Link>(),
                        maxProgress = maxProgress,
                        Progress = 0,
                        posterUrl = cEpisode.posterUrl,
                        rating = cEpisode.rating,
                        name = cEpisode.name,
                        date = cEpisode.date,
                        description = cEpisode.description
                    };

                    if (animeSeach) { // use https://www3.gogoanime.io/ or https://vidstreaming.io/

                        while (!activeMovie.title.MALData.done) {
                            Thread.Sleep(100);
                            if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                        }

                        string fwordLink = "";
                        List<string> fwords = GetAllEpsFromAnime(activeMovie, season, isDub);
                        for (int i = 0; i < fwords.Count; i++) {
                            // print("FW: " + fwords[i]);
                        }

                        // --------------- GET WHAT SEASON THE EPISODE IS IN ---------------

                        int sel = -1;
                        int _episode = int.Parse(episode.ToString());
                        int floor = 0;
                        int subtract = 0;
                        // print(activeMovie.title.MALData.currentActiveMaxEpsPerSeason);
                        if (activeMovie.title.MALData.currentActiveMaxEpsPerSeason != null) {
                            for (int i = 0; i < activeMovie.title.MALData.currentActiveMaxEpsPerSeason.Count; i++) {
                                int seling = floor + activeMovie.title.MALData.currentActiveMaxEpsPerSeason[i];

                                if (episode > floor && episode <= seling) {
                                    sel = i;
                                    subtract = floor;

                                }
                                //print(activeMovie.title.MALData.currentActiveMaxEpsPerSeason[i] + "<<");
                                floor += activeMovie.title.MALData.currentActiveMaxEpsPerSeason[i];
                            }
                        }
                        //print("sel: " + sel);
                        if (sel != -1) {
                            try {
                                fwordLink = fwords[sel].Replace("-dub", "") + (isDub ? "-dub" : "");
                            }
                            catch (Exception) {

                            }
                        }

                        if (fwordLink != "") { // IF FOUND
                            string dstring = "https://www3.gogoanime.io/" + fwordLink + "-episode-" + (episode - subtract);
                            print("DSTRING: " + dstring);
                            string d = DownloadString(dstring, tempThred);



                            string mp4 = "https://www.mp4upload.com/embed-" + FindHTML(d, "data-video=\"https://www.mp4upload.com/embed-", "\"");
                            print(mp4);
                            try {
                                string _d = DownloadString(mp4, tempThred);
                                if (!GetThredActive(tempThred)) { return; };
                                string mxLink = Getmp4UploadByFile(_d);
                                print(mxLink);

                                if (CheckIfURLIsValid(mxLink)) {
                                    Episode ep = activeMovie.episodes[normalEpisode];
                                    if (ep.links == null) {
                                        activeMovie.episodes[normalEpisode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                    }
                                    activeMovie.episodes[normalEpisode].links.Add(new Link() { priority = 0, url = mxLink, name = "Mp4Upload" }); // [MIRRORCOUNTER] IS LATER REPLACED WITH A NUMBER TO MAKE IT EASIER TO SEPERATE THEM, CAN'T DO IT HERE BECAUSE IT MUST BE ABLE TO RUN SEPARETE THREADS AT THE SAME TIME
                                    linkAdded?.Invoke(null, 1);
                                }
                            }
                            catch (System.Exception) {
                                print("BrowserMp4: " + mp4);

                            }

                            string fembed = FindHTML(d, "data-video=\"https://www.fembed.com/v/", "\"");
                            if(fembed == "") {
                                fembed = FindHTML(d, "data-video=\"https://gcloud.live/v/", "\"");
                            }
                            if (fembed != "") {
                                string _d = PostRequest("https://www.fembed.com/api/source/" + fembed, "https://www.fembed.com/v/" + fembed, "r=&d=www.fembed.com", tempThred);
                                if (_d != "") {
                                    string lookFor = "\"file\":\"";
                                    string _labelFind = "\"label\":\"";
                                    while (_d.Contains(_labelFind)) {
                                        string link = FindHTML(_d, lookFor, "\",\"");


                                        //  d = RemoveOne(d, link);
                                        link = link.Replace("\\/", "/");

                                        string label = FindHTML(_d, _labelFind, "\"");
                                        print(label + "|" + link);
                                        if (CheckIfURLIsValid(link)) {

                                            Episode ep = activeMovie.episodes[normalEpisode];
                                            if (ep.links == null) {
                                                activeMovie.episodes[normalEpisode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                            }
                                            activeMovie.episodes[normalEpisode].links.Add(new Link() { priority = 0, url = link, name = "XStream " + label }); // [MIRRORCOUNTER] IS LATER REPLACED WITH A NUMBER TO MAKE IT EASIER TO SEPERATE THEM, CAN'T DO IT HERE BECAUSE IT MUST BE ABLE TO RUN SEPARETE THREADS AT THE SAME TIME
                                            linkAdded?.Invoke(null, 1);
                                        }

                                        _d = RemoveOne(_d, _labelFind);
                                    }

                                }
                            }


                            string vid = FindHTML(d, "data-video=\"//vidstreaming.io/streaming.php?", "\"");
                            if (vid == "") {
                                vid = FindHTML(d, "//vidstreaming.io/streaming.php?", "\"");
                            }
                            if (vid != "") {
                                string dLink = "https://vidstreaming.io/download?" + vid;
                                string _d = DownloadString(dLink, tempThred);
                                if (!GetThredActive(tempThred)) { return; };

                                string linkContext = FindHTML(_d, "<h6>Link download</h6>", " </div>");
                                print(linkContext + " :LX");
                                string lookFor = "href=\"";
                                string rem = "<div class=<\"dowload\"><a";
                                linkContext = RemoveOne(linkContext, rem);
                                while (linkContext.Contains(lookFor)) {
                                    string link = FindHTML(linkContext, lookFor, "\"");
                                    string _nameContext = FindHTML(linkContext, link, "</a></div>") + "</a></div>";
                                    string name = "Vidstreaming (" + FindHTML(_nameContext, "            (", "</a></div>");
                                    link = link.Replace("&amp;", "&");

                                    print("LINK: " + link + "|" + name);

                                    if (CheckIfURLIsValid(link)) {

                                        Episode ep = activeMovie.episodes[normalEpisode];
                                        if (ep.links == null) {
                                            activeMovie.episodes[normalEpisode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                        }
                                        activeMovie.episodes[normalEpisode].links.Add(new Link() { priority = 0, url = link, name = name }); // [MIRRORCOUNTER] IS LATER REPLACED WITH A NUMBER TO MAKE IT EASIER TO SEPERATE THEM, CAN'T DO IT HERE BECAUSE IT MUST BE ABLE TO RUN SEPARETE THREADS AT THE SAME TIME
                                        linkAdded?.Invoke(null, 1);
                                    }

                                    linkContext = RemoveOne(linkContext, lookFor);
                                }

                                /* // OLD CODE, ONLY 403 ERROR DOSEN'T WORK ANYMORE
                                vid = "http://vidstreaming.io/streaming.php?" + vid;
                                string _d = DownloadString(vid); if (!GetThredActive(tempThred)) { return; };
                                string mxLink = FindHTML(_d, "sources:[{file: \'", "\'");
                                print("Browser: " + vid + " | RAW (NO ADS): " + mxLink);
                                if (CheckIfURLIsValid(mxLink)) {
                                    Episode ep = activeMovie.episodes[normalEpisode];
                                    if (ep.links == null) {
                                        activeMovie.episodes[normalEpisode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                    }
                                    activeMovie.episodes[normalEpisode].links.Add(new Link() { priority = 0, url = mxLink, name = "Vidstreaming" }); // [MIRRORCOUNTER] IS LATER REPLACED WITH A NUMBER TO MAKE IT EASIER TO SEPERATE THEM, CAN'T DO IT HERE BECAUSE IT MUST BE ABLE TO RUN SEPARETE THREADS AT THE SAME TIME
                                    linkAdded?.Invoke(null, 2);

                                }
                                */
                            }
                            else {
                                print("Error :(");
                            }
                        }
                    }
                    if (movieSearch) { // use https://movies123.pro/


                        // --------- SETTINGS ---------

                        bool canMovie = GetSettings(MovieType.Movie);
                        bool canShow = GetSettings(MovieType.TVSeries);

                        // -------------------- HD MIRRORS --------------------

                        if (GOMOSTEAM_ENABLED) {
                            TempThred minorTempThred = new TempThred();
                            minorTempThred.typeId = 3; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                            minorTempThred.Thread = new System.Threading.Thread(() => {
                                try {
                                    string find = activeMovie.title.name.ToLower() + (activeMovie.title.movieType == MovieType.TVSeries ? "-season-" + season : "");
                                    find = find.Replace("\'", "-");
                                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    find = rgx.Replace(find, "");


                                    find = find.Replace(" - ", "-").Replace(" ", "-");

                                    if (activeMovie.title.movieType == MovieType.TVSeries) { // ADD CORRECT FORMAT; https://gomostream.com/show/game-of-thrones/01-01
                                        find = find.Replace("-season-", "/");

                                        for (int i = 0; i < 10; i++) {
                                            if (find.EndsWith("/" + i)) {
                                                find = find.Replace("/" + i, "/0" + i);
                                            }
                                        }

                                        if (episode.ToString() != "-1") {
                                            find += "-" + episode;
                                        }

                                        for (int i = 0; i < 10; i++) {
                                            if (find.EndsWith("-" + i)) {
                                                find = find.Replace("-" + i, "-0" + i);
                                            }
                                        }
                                    }

                                    string gomoUrl = "https://gomostream.com/" + ((activeMovie.title.movieType == MovieType.Movie || activeMovie.title.movieType == MovieType.AnimeMovie) ? "movie" : "show") + "/" + find;
                                    print(gomoUrl);
                                    DownloadGomoSteam(gomoUrl, tempThred, normalEpisode);
                                }
                                finally {
                                    JoinThred(minorTempThred);
                                }
                            });
                            minorTempThred.Thread.Name = "Mirror Thread";
                            minorTempThred.Thread.Start();
                        }
                        if (SUBHDMIRROS_ENABLED) {
                            if (activeMovie.title.movies123MetaData.movieLink != null) {
                                if (activeMovie.title.movieType == MovieType.TVSeries) {
                                    int normalSeason = season - 1;
                                    List<Movies123SeasonData> seasonData = activeMovie.title.movies123MetaData.seasonData;
                                    // ---- TO PREVENT ERRORS START ----
                                    if (seasonData != null) {
                                        if (seasonData.Count > normalSeason) {
                                            if (seasonData[normalSeason].episodeUrls != null) {
                                                if (seasonData[normalSeason].episodeUrls.Count > normalEpisode) {
                                                    // ---- END ----

                                                    string fwordLink = seasonData[normalSeason].seasonUrl + "/" + seasonData[normalSeason].episodeUrls[normalEpisode];
                                                    print(fwordLink);
                                                    for (int f = 0; f < MIRROR_COUNT; f++) {
                                                        GetLinkServer(f, fwordLink, tempThred, normalEpisode);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else {
                                    for (int f = 0; f < MIRROR_COUNT; f++) {
                                        GetLinkServer(f, activeMovie.title.movies123MetaData.movieLink, tempThred); // JUST GET THE MOVIE
                                    }
                                }
                            }

                        }
                    }
                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "Get Links";
            tempThred.Thread.Start();


        }

        public static bool GetSettings(MovieType type = MovieType.Movie)
        {
            return true;
        }



        public static Episode SetEpisodeProgress(int progress, Episode ep)
        {
            return new Episode() { Progress = progress, date = ep.date, description = ep.description, links = ep.links, maxProgress = ep.maxProgress, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
        }
        public static Episode SetEpisodeProgress(Episode ep)
        {
            return new Episode() { Progress = ep.Progress + 1, date = ep.date, description = ep.description, links = ep.links, maxProgress = ep.maxProgress, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
        }


        public static void AddToActiveSearchResults(Poster p)
        {
            if (!activeSearchResults.Contains(p)) {

                bool add = true;
                for (int i = 0; i < activeSearchResults.Count; i++) {
                    if (activeSearchResults[i].posterUrl == p.posterUrl) {
                        add = false;
                    }
                }
                if (add) {
                    //print(p.name + "|" + p.posterUrl);
                    activeSearchResults.Add(p);
                    addedSeachResult?.Invoke(null, p);
                }
            }
        }

        /* ------------------------------------------------ EXAMPLE THRED ------------------------------------------------

            TempThred tempThred = new TempThred();
            tempThred.typeId = 1; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() =>
            {
                try {


                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                }
                finally {
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "QuickSearch";
            tempThred.Thread.Start();



            */

        // -------------------- METHODS --------------------
        static string HTMLGet(string uri, string referer, bool br = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            request.Method = "GET";
            request.ContentType = "text/html; charset=UTF-8";
            // webRequest.Headers.Add("Host", "trollvid.net");
            request.UserAgent = USERAGENT;
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Referer = referer;

            // webRequest.Headers.Add("Cookie", "__cfduid=dc6e854c3f07d2a427bca847e1ad5fa741562456483; _ga=GA1.2.742704858.1562456488; _gid=GA1.2.1493684150.1562456488; _maven_=popped; _pop_=popped");
            request.Headers.Add("TE", "Trailers");



            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                //  print(response.GetResponseHeader("set-cookie").ToString());


                // using (Stream stream = response.GetResponseStream())
                if (br) {
                    /*
                    using (BrotliStream bs = new BrotliStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)) {
                        using (System.IO.MemoryStream msOutput = new System.IO.MemoryStream()) {
                            bs.CopyTo(msOutput);
                            msOutput.Seek(0, System.IO.SeekOrigin.Begin);
                            using (StreamReader reader = new StreamReader(msOutput)) {
                                string result = reader.ReadToEnd();

                                return result;

                            }
                        }
                    }
                    */
                    return "";
                }
                else {
                    using (Stream stream = response.GetResponseStream()) {
                        // print("res" + response.StatusCode);
                        foreach (string e in response.Headers) {
                            // print("Head: " + e);
                        }
                        // print("LINK:" + response.GetResponseHeader("Set-Cookie"));
                        using (StreamReader reader = new StreamReader(stream)) {
                            string result = reader.ReadToEnd();
                            return result;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// WHEN DOWNLOADSTRING DOSNE'T WORK, BASILCY SAME THING, BUT CAN ALSO BE USED TO FORCE ENGLISH
        /// </summary>
        /// <param name="url"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetHTML(string url, bool en = true)
        {
            string html = string.Empty;

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                // List<string> heads = new List<string>(); // HEADERS
                /*
                heads = HeadAdd("");
                for (int i = 0; i < heads.Count; i++) {
                    try {
                        request.Headers.Add(HeadToRes(heads[i], 0), HeadToRes(heads[i], 1));
                        print("PRO:" + HeadToRes(heads[i], 0) + ": " + HeadToRes(heads[i], 1));

                    }
                    catch (Exception) {

                    }
                }
                */
                WebHeaderCollection myWebHeaderCollection = request.Headers;
                if (en) {
                    myWebHeaderCollection.Add("Accept-Language", "en;q=0.8");
                }
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.UserAgent = USERAGENT;
                request.Referer = url;
                //request.AddRange(1212416);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())

                using (StreamReader reader = new StreamReader(stream)) {
                    html = reader.ReadToEnd();
                }
                return html;
            }
            catch (Exception) {
                return "";
            }

        }
        static string ReadJson(string all, string inp)
        {
            try {
                int indexInp = all.IndexOf(inp);
                if (indexInp == -1) {
                    return "";
                }
                string newS = all.Substring(indexInp + (inp.Length + 3), all.Length - indexInp - (inp.Length + 3));

                string ns = newS.Substring(0, newS.IndexOf("\""));

                return ns;
            }
            catch (Exception) {
                return "";
            }
        }

        /// <summary>
        /// GET LOWHD MIRROR SERVER USED BY MOVIES123 AND PLACE THEM IN ACTIVEMOVIE
        /// </summary>
        /// <param name="f"></param>
        /// <param name="realMoveLink"></param>
        /// <param name="tempThred"></param>
        /// <param name="episode"></param>
        public static void GetLinkServer(int f, string realMoveLink, TempThred tempThred, int episode = 0)
        {
            TempThred minorTempThred = new TempThred();
            minorTempThred.typeId = 3; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            minorTempThred.Thread = new System.Threading.Thread(() => {
                try {
                    string jsn = GetWebRequest(realMoveLink + "?server=server_" + f);

                    if (!GetThredActive(minorTempThred)) { return; };  // ---- THREAD CANCELLED ----

                    while (jsn.Contains("http")) {
                        int _start = jsn.IndexOf("http");
                        jsn = jsn.Substring(_start, jsn.Length - _start);
                        int id = jsn.IndexOf("\"");
                        if (id != -1) {
                            string newM = jsn.Substring(0, id);
                            newM = newM.Replace("\\", "");
                            if (!LinkListContainsString(activeMovie.episodes[episode].links, newM)) {
                                if (!GetThredActive(minorTempThred)) { return; }; // ---- THREAD CANCELLED ----

                                if (CheckIfURLIsValid(newM)) {
                                    Episode ep = activeMovie.episodes[episode];
                                    if (ep.links == null) {
                                        activeMovie.episodes[episode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                    }
                                    activeMovie.episodes[episode].links.Add(new Link() { priority = 0, url = newM, name = "Mirror [MIRRORCOUNTER]" }); // [MIRRORCOUNTER] IS LATER REPLACED WITH A NUMBER TO MAKE IT EASIER TO SEPERATE THEM, CAN'T DO IT HERE BECAUSE IT MUST BE ABLE TO RUN SEPARETE THREADS AT THE SAME TIME
                                }
                            }
                        }
                        jsn = jsn.Substring(4, jsn.Length - 4);
                    }
                    // if (!GetThredActive(minorTempThred)) { minorTempThred.Progress = minorTempThred.Progress; return; }; // COPY UPDATE PROGRESS

                }
                finally {
                    activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode]); // ADDS ONE TO PROGRESS OF LINKS
                    JoinThred(minorTempThred);
                }
            });
            minorTempThred.Thread.Name = "Mirror Thread";
            minorTempThred.Thread.Start();
        }

        /// <summary>
        /// GET IF URL IS VALID, null and "" will return false
        /// </summary>
        /// <param name="uriName"></param>
        /// <returns></returns>
        public static bool CheckIfURLIsValid(string uriName)
        {
            if (uriName == null) return false;
            if (uriName == "") return false;

            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// RETURNS THE TRUE MX URL OF A MP4 UPLOAD
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        static string Getmp4UploadByFile(string result)
        {
            result = result.Replace("||||", "|");
            result = result.Replace("|||", "|");
            result = result.Replace("||", "|");
            result = result.Replace("||", "|");
            result = result.Replace("||", "|");

            string server = "s1";
            for (int i = 0; i < 100; i++) {
                if (result.Contains("|s" + i + "|")) {
                    server = "s" + i;
                }
            }

            for (int i = 0; i < 100; i++) {
                if (result.Contains("|www" + i + "|")) {
                    server = "www" + i;
                }
            }

            int pos = result.IndexOf("vid|mp4|download");
            int offset = 18;

            if (pos == -1) {
                offset = 9;
                pos = result.IndexOf("vid|mp4");
            }
            if (pos == -1) {
                pos = result.IndexOf("mp4|video");
                offset = 11;
            }

            if (pos == -1) {
                return "";
                /*
                if (_episode.Contains("This video is no longer available due to a copyright claim")) {
                    break;
                }
                */
            }

            string r = "-1";
            string allEp = result.Substring(pos + offset - 1, result.Length - pos - offset + 1);
            if ((allEp.Substring(0, 30).Contains("|"))) {
                string rez = allEp.Substring(0, allEp.IndexOf("p")) + "p";
                r = rez;
                allEp = allEp.Substring(allEp.IndexOf("p") + 2, allEp.Length - allEp.IndexOf("p") - 2);
            }
            string urlLink = allEp.Substring(0, allEp.IndexOf("|"));

            allEp = allEp.Substring(urlLink.Length + 1, allEp.Length - urlLink.Length - 1);
            string typeID = allEp.Substring(0, allEp.IndexOf("|"));

            string _urlLink = FindReverseHTML(result, "|" + typeID + "|", "|");

            string mxLink = "https://" + server + ".mp4upload.com:" + typeID + "/d/" + _urlLink + "/video.mp4"; //  282 /d/qoxtvtduz3b4quuorgvegykwirnmt3wm3mrzjwqhae3zsw3fl7ajhcdj/video.mp4

            string addRez = "";
            if (r != "-1") {
                addRez += " | " + r;
            }

            if (typeID != "282") {
                //Error
            }
            else {

            }
            return mxLink;

        }

        static string ReadDataMovie(string all, string inp)
        {
            try {
                string newS = all.Substring(all.IndexOf(inp) + (inp.Length + 2), all.Length - all.IndexOf(inp) - (inp.Length + 2));
                string ns = newS.Substring(0, newS.IndexOf("\""));
                return ns;
            }
            catch (Exception) {
                return "";
            }

        }
        public static string FindReverseHTML(string all, string first, string end, int offset = 0)
        {
            int x = all.IndexOf(first);
            all = all.Substring(0, x);
            int y = all.LastIndexOf(end) + end.Length;
            //  print(x + "|" + y);
            return all.Substring(y, all.Length - y);
        }

        /// <summary>
        /// REMOVES ALL SPECIAL CHARACTERS
        /// </summary>
        /// <param name="text"></param>
        /// <param name="toLower"></param>
        /// <param name="replaceSpace"></param>
        /// <returns></returns>
        public static string ToDown(string text, bool toLower = true, string replaceSpace = " ")
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            text = rgx.Replace(text, "");
            if (toLower) {
                text = text.ToLower();
            }
            text = text.Replace(" ", replaceSpace);
            return text;
        }

        static string ForceLetters(int inp, int letters = 2)
        {
            int added = letters - inp.ToString().Length;
            if (added > 0) {
                return MultiplyString("0", added) + inp.ToString();
            }
            else {
                return inp.ToString();
            }
        }

        public static string MultiplyString(string s, int times)
        {
            return String.Concat(Enumerable.Repeat(s, times));
        }

        /// <summary>
        /// NETFLIX like time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ConvertTimeToString(double time)
        {
            int sec = (int)Math.Round(time);
            int rsec = (sec % 60);
            int min = (int)Math.Ceiling((sec - rsec) / 60f);
            int rmin = min % 60;
            int h = (int)Math.Ceiling((min - rmin) / 60f);
            int rh = h;// h % 24;
            return (h > 0 ? ForceLetters(h) + ":" : "") + ((rmin >= 0 || h >= 0) ? ForceLetters(rmin) + ":" : "") + ForceLetters(rsec);
        }
        private static string GetWebRequest(string url)
        {
            string WEBSERVICE_URL = url;
            try {
                var __webRequest = System.Net.WebRequest.Create(WEBSERVICE_URL);
                if (__webRequest != null) {
                    __webRequest.Method = "GET";
                    __webRequest.Timeout = 12000;
                    __webRequest.ContentType = "application/json";
                    __webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

                    using (System.IO.Stream s = __webRequest.GetResponse().GetResponseStream()) {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s)) {
                            var jsonResponse = sr.ReadToEnd();
                            return jsonResponse.ToString();
                            // Console.WriteLine(String.Format("Response: {0}", jsonResponse));
                        }
                    }
                }
            }
            catch (System.Exception) { }

            return "";
        }

        /// <summary>
        /// GET GOMOSTEAM SITE MIRRORS
        /// </summary>
        /// <param name="url"></param>
        /// <param name="_tempThred"></param>
        /// <param name="episode"></param>
        static void DownloadGomoSteam(string url, TempThred _tempThred, int episode)
        {
            print("Downloading gomo: " + url);
            bool done = true;
            TempThred tempThred = new TempThred();
            tempThred.typeId = 3; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {
                    try {
                        string d = "";
                        // print(".." + url);
                        // Tries 5 times to connect
                        for (int i = 0; i < 5; i++) {
                            if (d == "") {
                                try {
                                    d = DownloadString(url, tempThred, false); if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                                }
                                catch (System.Exception) {
                                    debug("Error gogo");
                                    Thread.Sleep(1000);
                                }
                            }
                        }



                        if (d != "") { // If not failed to connect
                            debug("Passed gogo download site");

                            // ----- JS EMULATION, CHECK USED BY WEBSITE TO STOP WEB SCRAPE BOTS, DID NOT STOP ME >:) -----

                            string tokenCode = FindHTML(d, "var tc = \'", "'");
                            string _token = FindHTML(d, "_token\": \"", "\"");
                            string funct = "function _tsd_tsd_ds(" + FindHTML(d, "function _tsd_tsd_ds(", "</script>").Replace("\"", "'") + " log(_tsd_tsd_ds('" + tokenCode + "'))";
                            // print(funct);
                            if (funct == "function _tsd_tsd_ds( log(_tsd_tsd_ds(''))") {
                                debug(d); // ERROR IN LOADING JS
                            }
                            string realXToken = "";
                            var engine = new Engine()
                            .SetValue("log", new Action<string>((a) => { realXToken = a; }));

                            engine.Execute(@funct);
                            if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                            //GetAPI(realXToken, tokenCode, _token, tempThred, episode);


                            System.Uri myUri = new System.Uri("https://gomostream.com/decoding_v3.php"); // Can't DownloadString because of RequestHeaders (Anti-bot)
                            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);

                            // --- Headers ---

                            webRequest.Method = "POST";
                            webRequest.Headers.Add("x-token", realXToken);
                            webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                            webRequest.Headers.Add("DNT", "1");
                            webRequest.Headers.Add("Cache-Control", "max-age=0, no-cache");
                            webRequest.Headers.Add("TE", "Trailers");
                            webRequest.Headers.Add("Pragma", "Trailers");
                            webRequest.ContentType = "application/x-www-form-urlencoded";
                            done = false;


                            webRequest.BeginGetRequestStream(new AsyncCallback((IAsyncResult callbackResult) => {
                                HttpWebRequest _webRequest = (HttpWebRequest)callbackResult.AsyncState;
                                Stream postStream = _webRequest.EndGetRequestStream(callbackResult);

                                string requestBody = true ? ("tokenCode=" + tokenCode + "&_token=" + _token) : "type=epis&xny=hnk&id=" + tokenCode; // --- RequestHeaders ---

                                byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);

                                postStream.Write(byteArray, 0, byteArray.Length);
                                postStream.Close();

                                if (!GetThredActive(tempThred)) { return; };


                                // BEGIN RESPONSE

                                _webRequest.BeginGetResponse(new AsyncCallback((IAsyncResult _callbackResult) => {
                                    HttpWebRequest request = (HttpWebRequest)_callbackResult.AsyncState;
                                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(_callbackResult);
                                    using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream())) {
                                        if (!GetThredActive(tempThred)) { print(":("); return; };

                                        string result = httpWebStreamReader.ReadToEnd();

                                        try {
                                            if (result != "") {

                                                // --------------- GOT RESULT!!!!! ---------------


                                                WebClient client = new WebClient();
                                                //XbHP6duxDnD~1558891507~83.186.0.0~c5i0DlNs

                                                // --------------- MIRROR LINKS ---------------
                                                string veryURL = FindHTML(result, "https:\\/\\/verystream.com\\/e\\/", "\"");
                                                string gogoStream = FindHTML(result, "https:\\/\\/gomostream.com", "\"");
                                                string gunURL = "https://gounlimited.to/" + FindHTML(result, "https:\\/\\/gounlimited.to\\/", ".html") + ".html";
                                                string onlyURL = "https://onlystream.tv" + FindHTML(result, "https:\\/\\/onlystream.tv", "\"").Replace("\\", "");

                                                gogoStream = gogoStream.Replace(",,&noneemb", "").Replace("\\", "");

                                                // ------ EXAMPLE ------
                                                //["https:\/\/redirector.googlevideo.com\/videoplayback?id=3559ed25eabf374d&itag=22&source=picasa&begin=0&requiressl=yes&mm=30&mn=sn-4g5ednsy&ms=nxu&mv=u&pl=44&sc=yes&ei=oenhXN27O62S8gOI07awBA&susc=ph&app=fife&mime=video\/mp4&cnr=14&dur=7561.903&lmt=1557947360209526&mt=1558308859&ipbits=0&keepalive=yes&ratebypass=yes&ip=2a01:4f8:110:3447::2&expire=1558316481&sparams=ip,ipbits,expire,id,itag,source,requiressl,mm,mn,ms,mv,pl,sc,ei,susc,app,mime,cnr,dur,lmt&signature=9ABF4766E7C2573C0171F8D1C6F0761B289483F1B9704140A09090666F4EED83.25A52B55EF6070C25DB2608CFBF0994166D1CA477F85D0CD71994980976993C6&key=us0","
                                                //https:\/\/gomostream.com\/vid\/?v=eyJ0eXBlIjoibW92aWUiLCJpbWQiOiJ0dDAzNzE3NDYiLCJfIjoiMTI5NjUwODg1NTE3IiwidG9rZW4iOiI5MzExNTIifQ,,&noneemb",
                                                //"https:\/\/verystream.com\/e\/XbHP6duxDnD",
                                                //"https:\/\/hqq.tv\/player\/embed_player.php?vid=WDc5TjcvTkxXTFpBbXRLYzFSazFMUT09&autoplay=no",
                                                //"https:\/\/gounlimited.to\/embed-xjje1taiv02x.html",
                                                //"https:\/\/vcstream.to\/embed\/5cb01ff305468"]

                                                Episode ep = activeMovie.episodes[episode];
                                                if (ep.links == null) {
                                                    activeMovie.episodes[episode] = new Episode() { links = new List<Link>(), date = ep.date, description = ep.description, name = ep.name, posterUrl = ep.posterUrl, rating = ep.rating };
                                                }

                                                if (veryURL != "") {
                                                    try {
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        d = client.DownloadString("https://verystream.com/e/" + veryURL);
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        // print(d);
                                                        debug("-------------------- HD --------------------");
                                                        url = "https://verystream.com/gettoken/" + FindHTML(d, "videolink\">", "<");
                                                        debug(url);
                                                        if (url != "https://verystream.com/gettoken/") {
                                                            if (!LinkListContainsString(activeMovie.episodes[episode].links, url)) {
                                                                // print(activeMovie.episodes[episode].Progress);
                                                                activeMovie.episodes[episode].links.Add(new Link() { url = url, priority = 10, name = "HD Verystream" });
                                                            }
                                                        }

                                                        debug("--------------------------------------------");
                                                        debug("");
                                                    }
                                                    catch (System.Exception) {

                                                    }

                                                }
                                                else {
                                                    debug("HD Verystream Link error (Read api)");
                                                    debug("");
                                                }
                                                activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode]);


                                                if (gogoStream != "") {
                                                    try {
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        d = client.DownloadString("https://gomostream.com" + gogoStream);
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        //print(d);
                                                        // print("https://gomostream.com" + gogoStream);
                                                        //https://v16.viduplayer.com/vxokfmpswoalavf4eqnivlo2355co6iwwgaawrhe7je3fble4vtvcgek2jha/v.mp4
                                                        debug("-------------------- HD --------------------");
                                                        url = GetViduplayerUrl(d);
                                                        debug(url);
                                                        if (!url.EndsWith(".viduplayer.com/urlset/v.mp4") && !url.Contains("Error") && !url.EndsWith(".viduplayer.com/vplayer/v.mp4") && url != "https://d1.viduplayer.com/adb/v.mp4") {
                                                            if (!LinkListContainsString(activeMovie.episodes[episode].links, url)) {
                                                                //print(activeMovie.episodes[episode].Progress);
                                                                activeMovie.episodes[episode].links.Add(new Link() { url = url, priority = 9, name = "HD Viduplayer" });
                                                            }
                                                        }
                                                        debug("--------------------------------------------");
                                                        debug("");
                                                    }
                                                    catch (System.Exception) {
                                                    }

                                                }
                                                else {
                                                    debug("HD Viduplayer Link error (Read api)");
                                                    debug("");
                                                }
                                                activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode]);

                                                if (gunURL != "" && gunURL != "https://gounlimited.to/") {
                                                    try {
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        d = client.DownloadString(gunURL);
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        string mid = FindHTML(d, "mp4|", "|");
                                                        string server = FindHTML(d, mid + "|", "|");
                                                        url = "https://" + server + ".gounlimited.to/" + mid + "/v.mp4";
                                                        if (mid != "" && server != "") {
                                                            if (!LinkListContainsString(activeMovie.episodes[episode].links, url)) {
                                                                // print(activeMovie.episodes[episode].Progress);

                                                                activeMovie.episodes[episode].links.Add(new Link() { url = url, priority = 8, name = "HD Go Unlimited" });
                                                            }
                                                        }
                                                        debug("-------------------- HD --------------------");
                                                        debug(url);

                                                        debug("--------------------------------------------");
                                                        debug("");
                                                    }
                                                    catch (System.Exception) {

                                                    }

                                                }
                                                else {
                                                    debug("HD Go Link error (Read api)");
                                                    debug("");
                                                }
                                                activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode]);

                                                if (onlyURL != "" && onlyURL != "https://onlystream.tv") {
                                                    try {
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        d = client.DownloadString(onlyURL);
                                                        if (!GetThredActive(tempThred)) { return; };

                                                        string _url = FindHTML(d, "file:\"", "\"");
                                                        bool valid = false;
                                                        if (CheckIfURLIsValid(_url)) { // NEW USES JW PLAYER I THNIK, EASIER LINK EXTRACTION
                                                            url = _url; valid = true;
                                                        }
                                                        else { // OLD SYSTEM I THINK
                                                            string server = "";//FindHTML(d, "urlset|", "|");
                                                            string mid = FindHTML(d, "logo|", "|");

                                                            if (mid == "" || mid.Length < 10) {
                                                                mid = FindHTML(d, "mp4|", "|");
                                                            }

                                                            string prefix = FindHTML(d, "ostreamcdn|", "|");

                                                            url = "";
                                                            if (server != "") {
                                                                url = "https://" + prefix + ".ostreamcdn.com/" + server + "/" + mid + "/v/mp4"; // /index-v1-a1.m3u8 also works if you want the m3u8 file instead
                                                            }
                                                            else {
                                                                url = "https://" + prefix + ".ostreamcdn.com/" + mid + "/v/mp4";
                                                            }
                                                            if (mid != "" && prefix != "" && mid.Length > 10) {
                                                                valid = true;
                                                            }
                                                        }

                                                        if (valid) {
                                                            if (!LinkListContainsString(activeMovie.episodes[episode].links, url)) {
                                                                //  print(activeMovie.episodes[episode].Progress);

                                                                activeMovie.episodes[episode].links.Add(new Link() { url = url, priority = 7, name = "HD Onlystream" });
                                                            }
                                                        }
                                                        else {
                                                            debug(d);
                                                            debug("FAILED URL: " + url);
                                                        }

                                                        debug("-------------------- HD --------------------");
                                                        debug(url);

                                                        debug("--------------------------------------------");
                                                        debug("");
                                                    }
                                                    catch (System.Exception) {

                                                    }

                                                }
                                                else {
                                                    debug("HD Only Link error (Read api)");
                                                    debug("");
                                                }
                                                activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode]);
                                                //print(activeMovie.episodes[episode].Progress);
                                                done = true;
                                            }
                                            else {
                                                activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode].Progress + HD_MIRROR_COUNT, activeMovie.episodes[episode]);
                                                done = true;
                                                debug("DA FAILED");
                                            }
                                        }
                                        catch (Exception) {
                                            done = true;
                                        }

                                    }


                                }), _webRequest);


                            }), webRequest);
                        }
                        else {
                            activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode].Progress + 4, activeMovie.episodes[episode]);

                            debug("Dident get gogo");
                        }

                    }
                    catch (System.Exception) {
                        debug("Error");
                        activeMovie.episodes[episode] = SetEpisodeProgress(activeMovie.episodes[episode].Progress + 4, activeMovie.episodes[episode]);
                    }
                }
                finally {
                    while (!done) {
                        Thread.Sleep(20);
                    }
                    linksProbablyDone?.Invoke(null, activeMovie.episodes[episode]);
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "GomoSteam";
            tempThred.Thread.Start();




        }

        public static string PostRequest(string myUri, string referer = "", string _requestBody = "", TempThred? _tempThred = null)
        {
            try {

         
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);

            webRequest.Method = "POST";
            //  webRequest.Headers.Add("x-token", realXToken);
            webRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            webRequest.Headers.Add("DNT", "1");
            webRequest.Headers.Add("Cache-Control", "max-age=0, no-cache");
            webRequest.Headers.Add("TE", "Trailers");
            webRequest.Headers.Add("Pragma", "Trailers");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Referer = referer;
            webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            // webRequest.Headers.Add("Host", "trollvid.net");
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            webRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            bool done = false;
            string _res = "";
            webRequest.BeginGetRequestStream(new AsyncCallback((IAsyncResult callbackResult) => {
                HttpWebRequest _webRequest = (HttpWebRequest)callbackResult.AsyncState;
                Stream postStream = _webRequest.EndGetRequestStream(callbackResult);

                string requestBody = _requestBody;// --- RequestHeaders ---

                byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);

                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                if (_tempThred != null) {
                    TempThred tempThred = (TempThred)_tempThred;
                    if (!GetThredActive(tempThred)) { return; }
                }


                // BEGIN RESPONSE

                _webRequest.BeginGetResponse(new AsyncCallback((IAsyncResult _callbackResult) => {
                    HttpWebRequest request = (HttpWebRequest)_callbackResult.AsyncState;
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(_callbackResult);
                    if (_tempThred != null) {
                        TempThred tempThred = (TempThred)_tempThred;
                        if (!GetThredActive(tempThred)) { return; }
                    }
                    using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream())) {
                        if (_tempThred != null) {
                            TempThred tempThred = (TempThred)_tempThred;
                            if (!GetThredActive(tempThred)) { return; }
                        }
                        _res = httpWebStreamReader.ReadToEnd();
                        done = true;
                    }
                }), _webRequest);
            }), webRequest);


            for (int i = 0; i < 1000; i++) {
                Thread.Sleep(10);
                if (done) {
                    return _res;
                }
            }
            return _res;
            }
            catch (Exception) {

                return "";
            }
        }


        /// <summary>
        /// Returns the true mx url of Viduplayer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static string GetViduplayerUrl(string source)
        {
            source = source.Replace("||||", "|");
            source = source.Replace("|||", "|");
            source = source.Replace("||", "|");
            source = source.Replace("||", "|");
            source = source.Replace("||", "|");

            string inter = FindHTML(source, "|mp4|", "|");
            /*
            if(inter.Length < 5) {
                inter = FindHTML(_episode, "|srt|", "|");

            }
            if (inter.Length < 5) {
                inter = FindHTML(_episode, "|vvad|", "|");

            }*/

            string server = "";
            string[] serverStart = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            for (int s = 0; s < serverStart.Length; s++) {
                for (int i = 0; i < 100; i++) {
                    if (source.Contains("|" + serverStart[s] + i + "|")) {
                        server = serverStart[s] + i;
                    }
                }
            }

            for (int i = 0; i < 100; i++) {
                if (source.Contains("|www" + i + "|")) {
                    server = "www" + i;
                }
            }

            if (server == "") {
                return "Error, server not found";
            }
            if (inter == "") {
                return "Error, index not found";
            }
            if (inter.Length < 5) {
                inter = FindReverseHTML(source, "|" + server + "|", "|");
            }

            //https://v16.viduplayer.com/vxokfmpswoalavf4eqnivlo2355co6iwwgaawrhe7je3fble4vtvcgek2jha/v.mp4
            return "https://" + server + ".viduplayer.com/" + inter + "/v.mp4";
        }

        /// <summary>
        /// Do links contants inp
        /// </summary>
        /// <param name="links"></param>
        /// <param name="inp"></param>
        /// <returns></returns>
        public static bool LinkListContainsString(List<Link> links, string inp)
        {
            if (links == null) {
                return false;
            }
            else {
                try {
                    foreach (Link link in links) {
                        if (link.url == inp) { return true; }
                    }
                }
                catch {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Simple funct to download a sites fist page as string
        /// </summary>
        /// <param name="url"></param>
        /// <param name="UTF8Encoding"></param>
        /// <returns></returns>
        public static string DownloadString(string url, TempThred? tempThred = null, bool UTF8Encoding = true)
        {
            WebClient client = new WebClient();
            if (UTF8Encoding) {
                client.Encoding = Encoding.UTF8; // TO GET SPECIAL CHARACTERS ECT
            }

            try {


                // ANDROID DOWNLOADSTRING

                bool done = false;
                string _s = "";
                client.DownloadStringCompleted += (o, e) => {
                    done = true;
                    if (!e.Cancelled) {
                        if (e.Error == null) {
                            _s = e.Result;
                        }
                        else {
                            print(e.Error.Message + "|" + url);
                        }
                    }
                    else {
                        _s = "";
                    }
                };
                client.DownloadStringTaskAsync(url);
                for (int i = 0; i < 1000; i++) {
                    Thread.Sleep(10);

                    if (tempThred != null) {
                        if (!GetThredActive((TempThred)tempThred)) {
                            client.CancelAsync();
                            return "";
                        }
                    }

                    if (done) {
                        //print(_s);
                        print(">>" + i);
                        return _s;
                    }
                }
                client.CancelAsync();
                return _s;


                // return client.DownloadString(url);
            }
            catch (Exception) {
                return "";
            }
        }






        /// <summary>
        /// Makes first letter of all capital
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        static string ToTitle(string title)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(title.Replace("/", "").Replace("-", " "));
        }
        /// <summary>
        /// Used in while true loops to remove last used string
        /// </summary>
        /// <param name="d"></param>
        /// <param name="rem"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string RemoveOne(string d, string rem, int offset = 1)
        {
            return d.Substring(d.IndexOf(rem) + offset, d.Length - d.IndexOf(rem) - offset);
        }
        /// <summary>
        /// Used to find string in string, for example 123>Hello<132123, hello can be found using FindHTML(d,">","<");
        /// </summary>
        /// <param name="all"></param>
        /// <param name="first"></param>
        /// <param name="end"></param>
        /// <param name="offset"></param>
        /// <param name="readToEndOfFile"></param>
        /// <returns></returns>
        public static string FindHTML(string all, string first, string end, int offset = 0, bool readToEndOfFile = false)
        {
            if (all.IndexOf(first) == -1) {
                return "";
            }
            int x = all.IndexOf(first) + first.Length + offset;

            all = all.Substring(x, all.Length - x);
            int y = all.IndexOf(end);
            if (y == -1) {
                if (readToEndOfFile) {
                    y = all.Length;
                }
                else {
                    return "";
                }
            }
            //  print(x + "|" + y);
            return all.Substring(0, y);
        }
        public static void print(object o)
        {
#if DEBUG
            if (o != null) {
                System.Diagnostics.Debug.WriteLine(o.ToString());
            }
            else {
                System.Diagnostics.Debug.WriteLine("Null");
            }
#endif
        }
        public static void debug(object o)
        {
#if DEBUG
            if (o != null && DEBUG_WRITELINE) {
                System.Diagnostics.Debug.WriteLine(o.ToString());
            }
            else {
                System.Diagnostics.Debug.WriteLine("Null");
            }
#endif
        }

        // LICENSE
        //
        //   This software is dual-licensed to the public domain and under the following
        //   license: you are granted a perpetual, irrevocable license to copy, modify,
        //   publish, and distribute this file as you see fit.
        /// <summary>
        /// Does a fuzzy search for a pattern within a string.
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <returns>true if each character in pattern is found sequentially within stringToSearch; otherwise, false.</returns>
        public static bool FuzzyMatch(string stringToSearch, string pattern)
        {
            var patternIdx = 0;
            var strIdx = 0;
            var patternLength = pattern.Length;
            var strLength = stringToSearch.Length;

            while (patternIdx != patternLength && strIdx != strLength) {
                if (char.ToLower(pattern[patternIdx]) == char.ToLower(stringToSearch[strIdx]))
                    ++patternIdx;
                ++strIdx;
            }

            return patternLength != 0 && strLength != 0 && patternIdx == patternLength;
        }

        /// <summary>
        /// Does a fuzzy search for a pattern within a string, and gives the search a score on how well it matched.
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <param name="outScore">The score which this search received, if a match was found.</param>
        /// <returns>true if each character in pattern is found sequentially within stringToSearch; otherwise, false.</returns>
        public static bool FuzzyMatch(string stringToSearch, string pattern, out int outScore)
        {
            // Score consts
            const int adjacencyBonus = 5;               // bonus for adjacent matches
            const int separatorBonus = 10;              // bonus if match occurs after a separator
            const int camelBonus = 10;                  // bonus if match is uppercase and prev is lower

            const int leadingLetterPenalty = -3;        // penalty applied for every letter in stringToSearch before the first match
            const int maxLeadingLetterPenalty = -9;     // maximum penalty for leading letters
            const int unmatchedLetterPenalty = -1;      // penalty for every letter that doesn't matter


            // Loop variables
            var score = 0;
            var patternIdx = 0;
            var patternLength = pattern.Length;
            var strIdx = 0;
            var strLength = stringToSearch.Length;
            var prevMatched = false;
            var prevLower = false;
            var prevSeparator = true;                   // true if first letter match gets separator bonus

            // Use "best" matched letter if multiple string letters match the pattern
            char? bestLetter = null;
            char? bestLower = null;
            int? bestLetterIdx = null;
            var bestLetterScore = 0;

            var matchedIndices = new List<int>();

            // Loop over strings
            while (strIdx != strLength) {
                var patternChar = patternIdx != patternLength ? pattern[patternIdx] as char? : null;
                var strChar = stringToSearch[strIdx];

                var patternLower = patternChar != null ? char.ToLower((char)patternChar) as char? : null;
                var strLower = char.ToLower(strChar);
                var strUpper = char.ToUpper(strChar);

                var nextMatch = patternChar != null && patternLower == strLower;
                var rematch = bestLetter != null && bestLower == strLower;

                var advanced = nextMatch && bestLetter != null;
                var patternRepeat = bestLetter != null && patternChar != null && bestLower == patternLower;
                if (advanced || patternRepeat) {
                    score += bestLetterScore;
                    matchedIndices.Add((int)bestLetterIdx);
                    bestLetter = null;
                    bestLower = null;
                    bestLetterIdx = null;
                    bestLetterScore = 0;
                }

                if (nextMatch || rematch) {
                    var newScore = 0;

                    // Apply penalty for each letter before the first pattern match
                    // Note: Math.Max because penalties are negative values. So max is smallest penalty.
                    if (patternIdx == 0) {
                        var penalty = System.Math.Max(strIdx * leadingLetterPenalty, maxLeadingLetterPenalty);
                        score += penalty;
                    }

                    // Apply bonus for consecutive bonuses
                    if (prevMatched)
                        newScore += adjacencyBonus;

                    // Apply bonus for matches after a separator
                    if (prevSeparator)
                        newScore += separatorBonus;

                    // Apply bonus across camel case boundaries. Includes "clever" isLetter check.
                    if (prevLower && strChar == strUpper && strLower != strUpper)
                        newScore += camelBonus;

                    // Update pattern index IF the next pattern letter was matched
                    if (nextMatch)
                        ++patternIdx;

                    // Update best letter in stringToSearch which may be for a "next" letter or a "rematch"
                    if (newScore >= bestLetterScore) {
                        // Apply penalty for now skipped letter
                        if (bestLetter != null)
                            score += unmatchedLetterPenalty;

                        bestLetter = strChar;
                        bestLower = char.ToLower((char)bestLetter);
                        bestLetterIdx = strIdx;
                        bestLetterScore = newScore;
                    }

                    prevMatched = true;
                }
                else {
                    score += unmatchedLetterPenalty;
                    prevMatched = false;
                }

                // Includes "clever" isLetter check.
                prevLower = strChar == strLower && strLower != strUpper;
                prevSeparator = strChar == '_' || strChar == ' ';

                ++strIdx;
            }

            // Apply score for last match
            if (bestLetter != null) {
                score += bestLetterScore;
                matchedIndices.Add((int)bestLetterIdx);
            }

            outScore = score;
            return patternIdx == patternLength;
        }
    }
}
