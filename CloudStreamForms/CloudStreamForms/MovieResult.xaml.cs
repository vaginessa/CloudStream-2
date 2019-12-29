using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.CloudStreamCore;
using System.Reflection;
using System.Collections.ObjectModel;
using CloudStreamForms.Models;
using Xamarin.Essentials;
using CloudStreamForms;
using static CloudStreamForms.App;
using static CloudStreamForms.MainPage;
using static CloudStreamForms.Settings;
using System.Threading;
using Acr.UserDialogs;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieResult : ContentPage
    {
        public int SmallFontSize { get; set; } = 11;
        public int WithSize { get; set; } = 50;


        
        public Poster mainPoster;
        public string trailerUrl = "";
        List<Button> recBtts = new List<Button>();

        public static List<Movie> lastMovie;
        List<Poster> RecomendedPosters { set { currentMovie.title.recomended = value; } get { return currentMovie.title.recomended; } }  //= new List<Poster>();

        bool loadedTitle = false;
        int currentSeason = 0;
        //ListView episodeView;
        public const int heightRequestPerEpisode = 120;
        public const int heightRequestAddEpisode = 40;
        public const int heightRequestAddEpisodeAndroid = 0;

        bool isMovie = false;
        Movie currentMovie = new Movie();
        bool isDub = true;
        bool RunningWindows { get { return DeviceInfo.Platform == DevicePlatform.UWP; } }
        string CurrentMalLink
        {
            get {

                try {
                    string s = currentMovie.title.MALData.seasonData[currentSeason].malUrl;
                    if (s != "https://myanimelist.net") {
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
        }


        List<Episode> currentEpisodes { set { currentMovie.episodes = value; } get { return currentMovie.episodes; } }

        protected override bool OnBackButtonPressed()
        {
            if (lastMovie != null) {
                if (lastMovie.Count > 0) {
                    activeMovie = lastMovie[lastMovie.Count - 1];
                    lastMovie.RemoveAt(lastMovie.Count - 1);
                }
            }
            if (setKey) {
                App.RemoveKey("BookmarkData", currentMovie.title.id);
            }
            const bool animate = true;
            if (animate) {
                return base.OnBackButtonPressed();
            }
            else {
                Navigation.PopModalAsync(false);
                return true;
            }

        }

        void SetRows()
        {
            MainThread.BeginInvokeOnMainThread(() => {

                //  Grid.SetRow(RowSeason, SeasonPicker.IsVisible ? 3 - ((DubPicker.IsVisible ? 0 : 1) + (MALBtt.IsVisible ? 0 : 1)) : 0);
                //Grid.SetRow(RowDub, SeasonPicker.IsVisible ? (1 - (MALBtt.IsVisible ? 0 : 1)) : 0);
                //Grid.SetRow(RowMal, MALBtt.IsVisible ? 0 : 0);

                // MALBtt.IsVisible = MALBtt.IsVisible;
            });

        }
        public MainEpisodeView epView;

        bool setKey = false;
        void SetKey()
        {
            App.SetKey("BookmarkData", currentMovie.title.id, "Name=" + currentMovie.title.name + "|||PosterUrl=" + currentMovie.title.hdPosterUrl + "|||Id=" + currentMovie.title.id + "|||TypeId=" + ((int)currentMovie.title.movieType) + "|||ShortEpView=" + currentMovie.title.shortEpView + "|||=EndAll");
            setKey = false;
        }

        private void StarBttClicked(object sender, EventArgs e)
        {

            //bool SetValue = !App.GetKey("Bookmark", currentMovie.title.id, false);
            //  App.SetKey("Bookmark", currentMovie.title.id, SetValue);
            bool keyExists = App.KeyExists("BookmarkData", currentMovie.title.id);
            if (keyExists) {
                App.RemoveKey("BookmarkData", currentMovie.title.id);
            }
            else {
                if (currentMovie.title.name == null) {
                    App.SetKey("BookmarkData", currentMovie.title.id, "Name=" + currentMovie.title.name + "|||Id=" + currentMovie.title.id + "|||");

                    setKey = true;
                }
                else {
                    SetKey();
                }
            }

            ChangeStar(!keyExists);
        }

        private void SubtitleBttClicked(object sender, EventArgs e)
        {
            // bool SetValue = !App.GetKey("Settings", "Subtitles", true);
            // App.SetKey("Settings", "Subtitles", SetValue);
            Settings.SubtitlesEnabled = !Settings.SubtitlesEnabled;
            ChangeSubtitle(SubtitlesEnabled);
        }
        private void ShareBttClicked(object sender, EventArgs e)
        {
            if (currentMovie.title.id != "" && currentMovie.title.name != "") {
                Share();
            }
        }

        async void Share()
        {
            List<string> actions = new List<string>() { "Everything", "CloudStream Link", "IMDb Link", "Title", "Title and Description" };
            if (CurrentMalLink != "") {
                actions.Insert(3, "MAL Link");
            }
            if (trailerUrl != "") {
                actions.Insert(actions.Count - 2, "Trailer Link");
            }
            string a = await DisplayActionSheet("Copy", "Cancel", null, actions.ToArray());
            string copyTxt = "";
            if (a == "CloudStream Link") {
                string _s = CloudStreamCore.ShareMovieCode(currentMovie.title.id + "Name=" + currentMovie.title.name + "=EndAll");
                if (_s != "") {
                    copyTxt = _s;
                }
            }
            else if (a == "IMDb Link") {
                copyTxt = "https://www.imdb.com/title/" + currentMovie.title.id;
            }
            else if (a == "Title") {
                copyTxt = currentMovie.title.name + "\n" + currentMovie.title.description;
            }
            else if (a == "MAL Link") {
                copyTxt = CurrentMalLink;
            }
            else if (a == "Title and Description") {
                copyTxt = currentMovie.title.name + "\n" + currentMovie.title.description;
            }
            else if (a == "Trailer Link") {
                copyTxt = trailerUrl;
            }
            else if (a == "Everything") {
                copyTxt = currentMovie.title.name + " | " + RatingLabel.Text + "\n" + currentMovie.title.description;

                string _s = CloudStreamCore.ShareMovieCode(currentMovie.title.id + "Name=" + currentMovie.title.name + "=EndAll");
                if (_s != "") {
                    copyTxt = copyTxt + "\nCloudStream: " + _s;
                }
                copyTxt = copyTxt + "\nIMDb: " + "https://www.imdb.com/title/" + currentMovie.title.id;
                if (CurrentMalLink != "") {
                    copyTxt = copyTxt + "\nMAL: " + CurrentMalLink;
                }
                if (trailerUrl != "") {
                    copyTxt = copyTxt + "\nTrailer: " + trailerUrl;
                }
            }
            if (a != "Cancel" && copyTxt != "") {
                await Clipboard.SetTextAsync(copyTxt);
                App.ShowToast("Copied " + a + " to Clipboard");
            }

        }

        void ChangeStar(bool? overrideBool = null, string key = null)
        {
            bool keyExists = false;
            if (key == null) {
                key = currentMovie.title.id;
            }
            if (overrideBool == null) {
                keyExists = App.KeyExists("BookmarkData", key);
                print("KEYEXISTS:" + keyExists + "|" + currentMovie.title.id);
            }
            else {
                keyExists = (bool)overrideBool;
            }
            StarBtt.Source = GetImageSource((keyExists ? "bookmarkedBtt.png" : "notBookmarkedBtt.png"));
            Device.BeginInvokeOnMainThread(() => {
                StarBtt.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(keyExists ? DARK_BLUE_COLOR : LIGHT_BLACK_COLOR)) };
            });

        }
        void ChangeSubtitle(bool? overrideBool = null)
        {
            bool res = false;
            if (overrideBool == null) {
                res = SubtitlesEnabled;
            }
            else {
                res = (bool)overrideBool;
                //SubtitlesEnabled = res;
            }

            Device.BeginInvokeOnMainThread(() => {
                SubtitleBtt.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(res ? DARK_BLUE_COLOR : LIGHT_BLACK_COLOR)) };
            });
        }

        public void SetChromeCast(bool enabled)
        {
            ChromeCastBtt.IsVisible = enabled;
            ChromeCastBtt.IsEnabled = enabled;
            ImgChromeCastBtt.IsVisible = enabled;
            ImgChromeCastBtt.IsEnabled = enabled;
            if (enabled) {
                ImgChromeCastBtt.Source = GetImageSource(MainChrome.CurrentImageSource);
            }
            NameLabel.Margin = new Thickness((enabled ? 50 : 10), 10, 10, 10);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetChromeCast(MainChrome.IsChromeDevicesOnNetwork);
        }

        private void ChromeCastBtt_Clicked(object sender, EventArgs e)
        {
            WaitChangeChromeCast();
        }

        private void OpenChromecastView(object sender, EventArgs e)
        {
            if (sender != null) {
                ChromeCastPage.isActive = false;
            }
            if (!ChromeCastPage.isActive) {
                Page p = new ChromeCastPage() { episodeResult = chromeResult, chromeMovieResult = chromeMovieResult };
                Navigation.PushModalAsync(p, false);
            }
        }
        public static EpisodeResult chromeResult;
        public static Movie chromeMovieResult;
        async void WaitChangeChromeCast()
        {
            List<string> names = MainChrome.GetChromeDevicesNames();
            //UserDialogs.Instance.Alert("Hello", "Hello World", "OkText");

            string a = await DisplayActionSheet("Cast to", "Cancel", MainChrome.IsConnectedToChromeDevice ? "Disconnect" : null, names.ToArray());
            if (a != "Cancel") {
                MainChrome.ConnectToChromeDevice(a);
            }
            //string[] inputs = new string[names.Count + add];

            /*                 string action = "";
       int add = (MainChrome.IsConnectedToChromeDevice ? 1 : 0);

            var _c = new List<ActionSheetOption>();



            if (MainChrome.IsConnectedToChromeDevice) {
                //  inputs[0] = crossChar + "Disconnect";
                _c.Add(new ActionSheetOption("Disconnect") { Action = () => { MainChrome.ConnectToChromeDevice("Disconnect"); }, ItemIcon = "round_close_white_18.png" });
            }
            for (int i = 0; i < names.Count; i++) {
                //inputs[i + add] = tvChar + names[i];
                string _name = names[i].ToString();

                _c.Add(new ActionSheetOption(names[i]) {
                    Action = () => { MainChrome.ConnectToChromeDevice(_name); },
                    ItemIcon = "round_tv_white_18.png"
                });
            }*/
            // action = "";//(await DisplayActionSheet("Cast To", "Cancel", null, inputs)).Replace(crossChar, "").Replace(tvChar, "");

            /*
            UserDialogs.Instance.ActionSheet(//"Cast to","Cancel","",null,inputs

               new ActionSheetConfig() {
                   Title = "Cast To",
                   //Cancel = new ActionSheetOption("da") { Text = "Cancel" }, 
                   // ItemIcon = "da",
                   Options = _c,
                   Destructive = new ActionSheetOption("Cancel"),
                   //Destructive = MainChrome.IsConnectedToChromeDevice ? new ActionSheetOption("Disconnect", () => { MainChrome.ConnectToChromeDevice("Disconnect"); }) : null,
               });
               */


            // MainChrome.ConnectToChromeDevice(action);
        }

        void SetIsCasting(bool e)
        {
            ChromeRow.IsVisible = e;
            ChromeRow.IsEnabled = e;
            Grid.SetRow(SecChromeRow, e ? 5 : 4);
        }

        public static ImageSource GetGradient()
        {
            return GetImageSource(BlackBg ? "gradient.png" : "gradientGray.png");
        }

        public MovieResult()
        {
            InitializeComponent();

            mainPoster = Search.mainPoster;

            Gradient.Source = GetGradient();
            IMDbBtt.Source = GetImageSource("imdbIcon.png");
            MALBtt.Source = GetImageSource("MALIcon.png");
            ShareBtt.Source = GetImageSource("round_reply_white_48dp_inverted.png");
            StarBtt.Source = GetImageSource("notBookmarkedBtt.png");
            SubtitleBtt.Source = GetImageSource("round_subtitles_white_48dp.png");

            // -------------- CHROMECASTING THINGS --------------

            if (Device.RuntimePlatform == Device.UWP) {
                ImgChromeCastBtt.TranslationX = 0;
                ImgChromeCastBtt.TranslationY = 0;
                OffBar.IsVisible = false;
                OffBar.IsEnabled = false;
            }
            else {
                OffBar.Source = App.GetImageSource("gradient.png"); OffBar.HeightRequest = 3; OffBar.HorizontalOptions = LayoutOptions.Fill; OffBar.ScaleX = 100; OffBar.Opacity = 0.3; OffBar.TranslationY = 9;
            }

            ChromeCastQQ.Source = GetImageSource("round_cast_white_48dp2_4.png");//MainChrome.GetSourceFromInt());
            SetIsCasting(MainChrome.IsCastingVideo);

            MainChrome.OnChromeImageChanged += (o, e) => {
                ImgChromeCastBtt.Source = GetImageSource(e);
                ImgChromeCastBtt.Transformations.Clear();
                if (MainChrome.IsConnectedToChromeDevice) {
                    // ImgChromeCastBtt.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation("#303F9F")) };
                }
            };

            MainChrome.OnChromeDevicesFound += (o, e) => {
                SetChromeCast(MainChrome.IsChromeDevicesOnNetwork);
            };

            MainChrome.OnVideoCastingChanged += (o, e) => {
                SetIsCasting(e);
                if (e) {
                    OpenChromecastView(null, null);
                }
            };

            if (!MainChrome.IsConnectedToChromeDevice) {
                MainChrome.GetAllChromeDevices();
            }


            //ViewToggle.Source = GetImageSource("viewOnState.png");
            ChangeViewToggle();
            ChangeSubtitle();

            //NameLabel.Text = activeMovie.title.name;
            NameLabel.Text = mainPoster.name;
            RatingLabel.Text = mainPoster.year;

            titleLoaded += MovieResult_titleLoaded;
            trailerLoaded += MovieResult_trailerLoaded;
            episodeLoaded += MovieResult_epsiodesLoaded;


            // TrailerBtt.Clicked += TrailerBtt_Clicked;
            Gradient.Clicked += TrailerBtt_Clicked;
            linkAdded += MovieResult_linkAdded;
            linksProbablyDone += MovieResult_linksProbablyDone;

            fishingDone += MovieFishingDone;
            /*
            movie123FishingDone += MovieFishingDone;
            yesmovieFishingDone += MovieFishingDone;
            watchSeriesFishingDone += MovieFishingDone;
            fmoviesFishingDone += MovieFishingDone;*/

            if (Device.RuntimePlatform == Device.UWP) {
                //QuickMenu.WidthRequest = 500;
            }
            if (Settings.BlackBg) {
                BackgroundColor = Color.Black;
            }
            // Gradient.Opacity = BlackBg ? 1 : 0.5;

            MALBtt.IsVisible = false;
            MALBtt.IsEnabled = false;
            MALTxt.IsVisible = false;
            MALTxt.IsEnabled = false;

            epView = new MainEpisodeView();
            SetHeight();

            if (Device.RuntimePlatform == Device.UWP) {
               // DubPicker.TranslationY = 13.5;
            }

            BindingContext = epView;

            //episodeView.ItemAppearing += EpisodeView_ItemAppearing;
            SizeChanged += MainPage_SizeChanged;
            episodeView.VerticalScrollBarVisibility = Settings.ScrollBarVisibility;
            MScroll.HorizontalScrollBarVisibility = Settings.ScrollBarVisibility;
            ReloadAllBtt.Clicked += (o, e) => {
                App.RemoveKey("CacheImdb", currentMovie.title.id);
                App.RemoveKey("CacheMAL", currentMovie.title.id);
                Navigation.PopModalAsync(false);
                PushPageFromUrlAndName(currentMovie.title.id, currentMovie.title.name);
            };
            ReloadAllBtt.Source = GetImageSource("round_refresh_white_48dp.png");
            GetImdbTitle(mainPoster);
            currentMovie.title.id = mainPoster.url.Replace("https://imdb.com/title/", "");
            ChangeStar();
        }

        private void MainPage_SizeChanged(object sender, EventArgs e)
        {
            // WaitScale();
        }
        List<Grid> grids = new List<Grid>();
        List<ProgressBar> progressBars = new List<ProgressBar>();


        public void SetColor(EpisodeResult episodeResult)
        {
            string id = GetId(episodeResult);
            if (id != "") {
                List<string> hexColors = new List<string>() { "#ffffff", LIGHT_BLUE_COLOR, "#e5e598" };
                List<string> darkHexColors = new List<string>() { "#808080", DARK_BLUE_COLOR, "#d3c450" };
                int color = 0;
                if (App.KeyExists("ViewHistory", id)) {
                    color = 1;
                }
                if (App.KeyExists("Download", id)) {
                    color = 2;
                }

                episodeResult.MainTextColor = hexColors[color];
                episodeResult.MainDarkTextColor = darkHexColors[color];
                //print(App.KeyExists("ViewHistory", id) + "|" + id);
            }
        }

        public void AddEpisode(EpisodeResult episodeResult)
        {
            episodeResult.ogTitle = Title;
            SetColor(episodeResult);
            if (episodeResult.Rating != "") {
                episodeResult.Title += " | ★ " + episodeResult.Rating;
            }

            if (episodeResult.PosterUrl == "") {
                if (activeMovie.title.posterUrl != "") {
                    string posterUrl = "";
                    try {
                        if (activeMovie.title.trailers.Count > 0) {
                            if (activeMovie.title.trailers[0].posterUrl != null) {
                                posterUrl = activeMovie.title.trailers[0].posterUrl;
                            }
                        }
                    }
                    catch (Exception) {

                    }
                    episodeResult.PosterUrl = posterUrl;
                }
            }
            //"_V1_UY126_UX224_AL_"
            double mMulti = 2;
            int pwidth = 224;
            int pheight = 126;
            pheight = (int)Math.Round(pheight * mMulti * posterRezMulti);
            pwidth = (int)Math.Round(pwidth * mMulti * posterRezMulti);
            episodeResult.PosterUrl = episodeResult.PosterUrl.Replace(",126,224_AL", "," + pwidth + "," + pheight + "_AL").Replace("UY126", "UY" + pheight).Replace("UX224", "UX" + pwidth);
            //print(episodeResult.PosterUrl);

            epView.MyEpisodeResultCollection.Add(episodeResult);
            SetHeight();
        }

        public void ClearEpisodes()
        {
            episodeView.ItemsSource = null;
            epView.MyEpisodeResultCollection.Clear();
            episodeView.ItemsSource = epView.MyEpisodeResultCollection;
            // episodeView.HeightRequest = 0;

            play_btts = new List<FFImageLoading.Forms.CachedImage>();
            grids = new List<Grid>();
            progressBars = new List<ProgressBar>();
            //  grids.Clear();
            // gridsSize.Clear();
            SetHeight();
        }

        /*
        private void ViewCell_SizeChanged(object sender, EventArgs e)
        {
          //  print("VIEWCELL CHANGED");


            if (sender is Grid) {
                Grid grid = (Grid)sender;
                EpisodeResult episodeResult = ((EpisodeResult)((Grid)sender).BindingContext);

                // ProgressBar progressBar = (ProgressBar)( (Grid)((Grid)((Grid)grid.Children.ElementAt(0)).Children.ElementAt(0)).Children.ElementAt(2)).Children.ElementAt(0);
                //  progressBars = new List<ProgressBar>(epView.MyEpisodeResultCollection.Count);
                if (counter < epView.MyEpisodeResultCollection.Count) {
                    grids.Add(grid);

                }
                // print(">>>>> cc" + counter + "/" +);
                counter++;
                //  if (counter >= (epView.MyEpisodeResultCollection.Count)) {
                WaitScale();
                //}
            }
        }
        */
        /*
        async void WaitScale()
        {
            await Task.Delay(30);
            totalHeight = 0;
            for (int i = 0; i < grids.Count; i++) {
                Grid grid = grids[i];
                totalHeight += grid.Height;
                totalHeight += grid.Margin.Top;
                totalHeight += grid.Margin.Bottom;
            }
            // Device.BeginInvokeOnMainThread(() => episodeView.HeightRequest = totalHeight + 50);
           Device.BeginInvokeOnMainThread(() => episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight);
        }*/

        void SetHeight()
        {
            Device.BeginInvokeOnMainThread(() => episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight + 20);
        }


        private void EpisodeView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            // print("SPACING;: " + RText.Y + "|" + RText.AnchorY + "|" + RText.TranslationY + "|" + episodeView.Height + "|" + e.ItemIndex + "|" + episodeView.Y + "|" + episodeView.TranslationY + "|" + episodeView.Bounds.Height);
            //  placeIt.IsVisible = true;
            //  episodeView.Footer = placeIt;
            //placeIt.IsVisible = false;

            //  SLay.HeightRequest = 100000;
            //  print("A---- :" + episodeView.RowHeight * epView.MyEpisodeResultCollection.Count + "--: " + episodeView.);
            //  SLay.HeightRequest = episodeView.RowHeight * epView.MyEpisodeResultCollection.Count;
        }



        private void MovieFishingDone(object sender, Movie e)
        {
            if (!SameAsActiveMovie()) return;
            currentMovie = e;
        }

        bool SameAsActiveMovie()
        {
            //print(currentMovie.title.id + " || " + activeMovie.title.id);
            return currentMovie.title.id == activeMovie.title.id;
        }

        private void MovieResult_linksProbablyDone(object sender, Episode e)
        {
            /*
            foreach (var item in currentEpisodes) {
                if (item.name == e.name) {
                    List<Link> links = e.links.OrderBy(l => -l.priority).ToList();
                    for (int i = 0; i < links.Count; i++) {
                        print(links[i].name + " | " + links[i].url);
                    }
                    try {
                        PlayVLCWithSingleUrl(links[0].url);

                    }
                    catch (Exception) {

                    }
                    return;
                }
            }*/

        }


        private void MovieResult_linkAdded(object sender, Link e)
        {
            if (!SameAsActiveMovie()) return;


            if (currentMovie.episodes[0].name + currentMovie.episodes[0].description == activeMovie.episodes[0].name + activeMovie.episodes[0].description && epView.MyEpisodeResultCollection.Count > 0) {

            }
            else {
                return;
            }
            MainThread.BeginInvokeOnMainThread(() => {

                currentMovie = activeMovie;

                if (currentEpisodes != null) {
                    for (int i = 0; i < currentEpisodes.Count; i++) {
                        if (currentEpisodes[i].links != null) {

                            if (currentEpisodes[i].links.Count > 0) {

                                List<Link> links = currentEpisodes[i].links;
                                try {
                                    links = links.OrderBy(l => -l.priority).ToList();
                                }
                                catch (Exception) {

                                }
                                if (links.Count != 0) {
                                    // print("LINK ADDED" + links.Count + "|" + links[links.Count - 1].name);
                                }

                                epView.MyEpisodeResultCollection[i].epVis = true;
                                List<string> mirrors = new List<string>();
                                List<string> mirrorsUrls = new List<string>();
                                int mirrorCounter = 0;
                                // myEpisodeResultCollection[i].Mirros.Clear();
                                for (int f = 0; f < links.Count; f++) {
                                    try {
                                        Link link = links[f];

                                        if (CheckIfURLIsValid(link.url)) {
                                            string name = link.name;
                                            if (name.Contains("[MIRRORCOUNTER]")) {
                                                mirrorCounter++;
                                                name = name.Replace("[MIRRORCOUNTER]", mirrorCounter.ToString());
                                            }
                                            mirrors.Add(name);
                                            mirrorsUrls.Add(link.url);
                                            //    myEpisodeResultCollection[i].Mirros.Add(currentEpisodes[i].links[f].name);
                                        }
                                    }
                                    catch (Exception) {

                                    }
                                }

                                if (mirrors.Count > epView.MyEpisodeResultCollection[i].Mirros.Count) {
                                    //EpisodeResult epRes = epView.MyEpisodeResultCollection[i];
                                    epView.MyEpisodeResultCollection[i].mirrosUrls = mirrorsUrls;
                                    epView.MyEpisodeResultCollection[i].epVis = mirrors.Count > 0;
                                    epView.MyEpisodeResultCollection[i].Mirros = mirrors;// = new EpisodeResult() { mirros = mirrors, Description = epRes.Description, epVis = mirrors.Count > 0, Id = epRes.Id, mirrosUrls = mirrorsUrls, PosterUrl = epRes.PosterUrl, progress = epRes.progress, Rating = epRes.Rating, subtitles = epRes.subtitles, Title = epRes.Title };
                                }
                            }
                        }
                    }
                }
            });
            //print(e + "|" + activeMovie.episodes[1].maxProgress);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!SameAsActiveMovie()) return;

            // EpisodeResult episodeResult = ((MainEpisodeView)((ListView)sender).BindingContext).MyEpisodeResultCollection[e.ItemIndex];


        }

        private void TrailerBtt_Clicked(object sender, EventArgs e)
        {
            if (trailerUrl != null) {
                if (trailerUrl != "") {
                    App.PlayVLCWithSingleUrl(trailerUrl, currentMovie.title.name + " - Trailer");
                }
            }
        }


        private void MovieResult_titleLoaded(object sender, Movie e)
        {
            if (loadedTitle) return;
            if (e.title.name != mainPoster.name) return;

            loadedTitle = true;
            isMovie = (e.title.movieType == MovieType.Movie || e.title.movieType == MovieType.AnimeMovie);
            currentMovie = e;
            if (setKey) {
                SetKey();
            }
            print("Title loded" + " | " + mainPoster.name);
            MainThread.BeginInvokeOnMainThread(() => {
                try {
                    string souceUrl = e.title.trailers.First().posterUrl;
                    if (CheckIfURLIsValid(souceUrl)) {

                        TrailerBtt.Source = souceUrl;
                    }
                    else {
                        TrailerBtt.Source = ImageSource.FromResource("CloudStreamForms.Resource.gradient.png", Assembly.GetExecutingAssembly());
                    }
                }
                catch (Exception) {
                    TrailerBtt.Source = ImageSource.FromResource("CloudStreamForms.Resource.gradient.png", Assembly.GetExecutingAssembly());
                }
                ChangeStar();

                if (!RunningWindows) {
                    //Gradient.IsVisible = false;
                }

                string extra = "";
                bool haveSeasons = e.title.seasons != 0;

                if (haveSeasons) {
                    extra = e.title.seasons + " Season" + (e.title.seasons == 1 ? "" : "s") + " | ";
                }

                string rYear = mainPoster.year;
                if (rYear == null || rYear == "") {
                    rYear = e.title.year;
                }
                RatingLabel.Text = (rYear + " | " + e.title.runtime + " | " + extra + "★ " + e.title.rating).Replace("|  |", "|");
                DescriptionLabel.Text = Settings.MovieDecEnabled ? e.title.description.Replace("\\u0027", "\'") : "";
                if (e.title.description == "") {
                    DescriptionLabel.HeightRequest = 0;
                }

                // ---------------------------- SEASONS ----------------------------

                // currentMovie.title.movieType == MovieType.Anime;

                SeasonPicker.IsVisible = haveSeasons;
                SeasonPicker.SelectedIndexChanged += SeasonPicker_SelectedIndexChanged;
                DubPicker.SelectedIndexChanged += DubPicker_SelectedIndexChanged;
                if (haveSeasons) {
                    SeasonPicker.Items.Clear();
                    for (int i = 1; i <= e.title.seasons; i++) {
                        SeasonPicker.Items.Add("Season " + i);
                    }
                    int selIndex = App.GetKey<int>("SeasonIndex", activeMovie.title.id, 0);
                    try {
                        SeasonPicker.SelectedIndex = Math.Min(selIndex, SeasonPicker.Items.Count - 1);
                    }
                    catch (Exception) {
                        SeasonPicker.SelectedIndex = 0; // JUST IN CASE
                    }

                    currentSeason = SeasonPicker.SelectedIndex+1;
                    GetImdbEpisodes(currentSeason);
                }
                else {
                    currentSeason = 0; // MOVIES
                    GetImdbEpisodes();
                }

                // ---------------------------- RECOMMENDATIONS ----------------------------
                foreach (var item in Recommendations.Children) { // SETUP
                    Grid.SetColumn(item, 0);
                    Grid.SetRow(item, 0);
                }
                Recommendations.Children.Clear();

                double multi = 1.2;
                int height = 100;
                int width = 65;
                if (Device.RuntimePlatform == Device.UWP) {
                    height = 130;
                    width = 85;
                }

                height = (int)Math.Round(height * multi);
                width = (int)Math.Round(width * multi);

                int pheight = (int)Math.Round(height * 4 * posterRezMulti);
                int pwidth = (int)Math.Round(width * 4 * posterRezMulti);
                Recommendations.HeightRequest = height;
                for (int i = 0; i < RecomendedPosters.Count; i++) {
                    Poster p = e.title.recomended[i];
                    string posterURL = p.posterUrl.Replace(",76,113_AL", "," + pwidth + "," + pheight + "_AL").Replace("UY113", "UY" + pheight).Replace("UX76", "UX" + pwidth);
                    if (CheckIfURLIsValid(posterURL)) {

                        Grid stackLayout = new Grid();
                        Button imageButton = new Button() { HeightRequest = height, WidthRequest = width, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Start };
                        var ff = new FFImageLoading.Forms.CachedImage {
                            Source = posterURL,
                            HeightRequest = height,
                            WidthRequest = width,
                            BackgroundColor = Color.Transparent,
                            VerticalOptions = LayoutOptions.Start,
                            Transformations = {
                                new FFImageLoading.Transformations.RoundedTransformation(10,1,1.5,10,"#303F9F")
                            },
                            InputTransparent = true,
                        };

                        //Source = p.posterUrl
                        recBtts.Add(imageButton);

                        stackLayout.Children.Add(ff);
                        stackLayout.Children.Add(imageButton);

                        Recommendations.Children.Add(stackLayout);

                    }
                }
                RecomendationLoaded.IsVisible = false;
                for (int i = 0; i < recBtts.Count; i++) {

                    // --- TOAST ---
                    recBtts[i].Pressed += (o, _e) => {
                        for (int z = 0; z < recBtts.Count; z++) {
                            if (((Button)o).Id == recBtts[z].Id) {
                                guidIdRecomendations = recBtts[z].Id;
                                extraZRecomend = z;
                                extraScrollPos = MScroll.ScrollX;
#pragma warning disable
                                WaitFor(500, new Action(() => {
                                    if (recBtts[extraZRecomend].Id == guidIdRecomendations && Math.Abs(MScroll.ScrollX - extraScrollPos) < 10) {
                                        App.ShowToast(RecomendedPosters[extraZRecomend].name);
                                    }
                                }));
#pragma warning restore
                            }
                        }
                    };
                    recBtts[i].Released += (o, _e) => {
                        guidIdRecomendations = new Guid();
                    };

                    // --- RECOMMENDATIONS CLICKED -----
                    recBtts[i].Clicked += (o, _e) => {
                        for (int z = 0; z < recBtts.Count; z++) {
                            if (((Button)o).Id == recBtts[z].Id) {
                                if (lastMovie == null) {
                                    lastMovie = new List<Movie>();
                                }
                                lastMovie.Add(activeMovie);
                                Search.mainPoster = RecomendedPosters[z];
                                Page p = new MovieResult();// { mainPoster = mainPoster };
                                Navigation.PushModalAsync(p);
                            }
                        }
                    };
                }

                for (int i = 0; i < Recommendations.Children.Count; i++) { // GRID
                    Grid.SetColumn(Recommendations.Children[i], i);
                }

                SetRows();
            });



        }
        Guid guidIdRecomendations = new Guid();
        int extraZRecomend;
        double extraScrollPos = 0;
        async Task WaitFor(int miliSec, Action a)
        {
            await Task.Delay(miliSec);
            a();
        }



        private void DubPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                isDub = "Dub" == DubPicker.Items[DubPicker.SelectedIndex];
                SetDubExist();
            }
            catch (Exception) {

            }
        }

        private void SeasonPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearEpisodes();
            currentSeason = SeasonPicker.SelectedIndex + 1;
            App.SetKey("SeasonIndex", activeMovie.title.id, SeasonPicker.SelectedIndex);

            GetImdbEpisodes(currentSeason);
            // myEpisodeResultCollection.Clear();
        }

        private void MovieResult_epsiodesLoaded(object sender, List<Episode> e)
        {
            if (!SameAsActiveMovie()) return;
            print("episodes loaded");

            currentMovie = activeMovie;

            currentMovie.episodes = e;
            MainThread.BeginInvokeOnMainThread(() => {
                currentEpisodes = e;
                ClearEpisodes();
                bool isLocalMovie = false;
                bool isAnime = currentMovie.title.movieType == MovieType.Anime;


                if (currentMovie.title.movieType != MovieType.Movie && currentMovie.title.movieType != MovieType.AnimeMovie) {
                    if (currentMovie.title.movieType != MovieType.Anime) {
                        for (int i = 0; i < currentEpisodes.Count; i++) {
                            AddEpisode(new EpisodeResult() { Title = (i + 1) + ". " + currentEpisodes[i].name, Id = i, Description = currentEpisodes[i].description.Replace("\n", "").Replace("  ", ""), PosterUrl = currentEpisodes[i].posterUrl, Rating = currentEpisodes[i].rating, Progress = 0, epVis = false, subtitles = new List<string>() { "None" }, Mirros = new List<string>() });
                            //print("ADDEPU" + currentEpisodes[i].name);
                        }
                    }
                }
                else {
                    AddEpisode(new EpisodeResult() { Title = currentMovie.title.name, Description = currentMovie.title.description, Id = 0, PosterUrl = "", Progress = 0, Rating = "", epVis = false, subtitles = new List<string>() { "None" }, Mirros = new List<string>() });
                    isLocalMovie = true;
                }

                //episodeView.HeightRequest = myEpisodeResultCollection.Count * heightRequestPerEpisode + (RunningWindows ? heightRequestAddEpisode : heightRequestAddEpisodeAndroid);

                DubPicker.Items.Clear();

                if (isAnime) {
                    bool dubExists = false;
                    bool subExists = false;
                    try {

                        for (int q = 0; q < currentMovie.title.MALData.seasonData[currentSeason].seasons.Count; q++) {
                            MALSeason ms = currentMovie.title.MALData.seasonData[currentSeason].seasons[q];

                            if (ms.dubExists) {
                                dubExists = true;
                            }
                            if (ms.subExists) {
                                subExists = true;
                            }
                        }
                    }
                    catch (Exception) {

                    }

                    isDub = dubExists;

                    if (Settings.DefaultDub) {
                        if (dubExists) {
                            DubPicker.Items.Add("Dub");
                        }
                    }

                    if (subExists) {
                        DubPicker.Items.Add("Sub");
                    }
                    if (!Settings.DefaultDub) {
                        if (dubExists) {
                            DubPicker.Items.Add("Dub");
                        }
                    }

                    if (DubPicker.Items.Count > 0) {
                        DubPicker.SelectedIndex = 0;
                    }
                    SetDubExist();
                }
                else {

                }

                DubPicker.IsVisible = DubPicker.Items.Count > 0;
                print(DubPicker.IsVisible + "ENABLED");
                bool enabled = CurrentMalLink != "";
                MALBtt.IsVisible = enabled;
                MALBtt.IsEnabled = enabled;
                MALTxt.IsVisible = enabled;
                MALTxt.IsEnabled = enabled;
                
                SetRows();

            });
        }

        void SetDubExist()
        {
            if (!SameAsActiveMovie()) return;

            // string dstring = "";
            List<string> baseUrls = GetAllEpsFromAnime(currentMovie, currentSeason, isDub);

            if (baseUrls.Count > 0) {

                TempThred tempThred = new TempThred();
                tempThred.typeId = 6; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        int max = 0;
                        activeMovie.title.MALData.currentActiveMaxEpsPerSeason = new List<int>();

                        for (int i = 0; i < baseUrls.Count; i++) {
                            string dstring = baseUrls[i]; dstring = dstring.Replace("-dub", "") + (isDub ? "-dub" : "");
                            string d = DownloadString("https://www9.gogoanime.io/category/" + dstring);
                            if (d == "") {
                                return;
                            }
                            if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                            string subMax = FindHTML(d, "class=\"active\" ep_start = \'", ">");
                            string maxEp = FindHTML(subMax, "ep_end = \'", "\'");//FindHTML(d, "<a href=\"#\" class=\"active\" ep_start = \'0\' ep_end = \'", "\'");
                            print(i + "MAXEP" + maxEp);
                            print(baseUrls[i]);
                            int _epCount = (int)Math.Floor(decimal.Parse(maxEp));
                            max += _epCount;
                            if (!SameAsActiveMovie()) return;
                            try {
                                activeMovie.title.MALData.currentActiveMaxEpsPerSeason.Add(_epCount);

                            }
                            catch (Exception) {

                            }
                        }

                        MainThread.BeginInvokeOnMainThread(() => {
                            ClearEpisodes();
                            for (int i = 0; i < max; i++) {
                                try {
                                    AddEpisode(new EpisodeResult() { Title = (i + 1) + ". " + currentEpisodes[i].name, Id = i, Description = currentEpisodes[i].description.Replace("\n", "").Replace("  ", ""), PosterUrl = currentEpisodes[i].posterUrl, Rating = currentEpisodes[i].rating, Progress = 0, epVis = false, subtitles = new List<string>() { "None" }, Mirros = new List<string>() });

                                }
                                catch (Exception) {
                                    AddEpisode(new EpisodeResult() { Title = (i + 1) + ". " + "Episode #" + (i + 1), Id = i, Description = "", PosterUrl = "", Rating = "", Progress = 0, epVis = false, subtitles = new List<string>() { "None" }, Mirros = new List<string>() });

                                }
                            }
                            //episodeView.HeightRequest = myEpisodeResultCollection.Count * heightRequestPerEpisode + heightRequestAddEpisode;
                            SetRows();
                        });


                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Gogoanime Download";
                tempThred.Thread.Start();


            }
        }


        private void MovieResult_trailerLoaded(object sender, string e)
        {
            if (!SameAsActiveMovie()) return;

            trailerUrl = e;
            /*
            MainThread.BeginInvokeOnMainThread(() => {
                Trailer trailer = activeMovie.title.trailers.First();
                trailerUrl = trailer.url;
                print(trailer.posterUrl);
                TrailerBtt.Source = trailer.posterUrl;//ImageSource.FromUri(new System.Uri(trailer.posterUrl));

            });*/

        }



        private void IMDb_Clicked(object sender, EventArgs e)
        {
            if (!SameAsActiveMovie()) return;
            App.OpenBrowser("https://www.imdb.com/title/" + mainPoster.url);
        }
        private void MAL_Clicked(object sender, EventArgs e)
        {
            if (!SameAsActiveMovie()) return;
            App.OpenBrowser(CurrentMalLink);
        }


        List<FFImageLoading.Forms.CachedImage> play_btts = new List<FFImageLoading.Forms.CachedImage>();
        List<FFImageLoading.Forms.CachedImage> gray_images = new List<FFImageLoading.Forms.CachedImage>();
        private void Image_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            FFImageLoading.Forms.CachedImage image = ((FFImageLoading.Forms.CachedImage)sender);
            if (play_btts.Where(t => t.Id == image.Id).Count() == 0) {
                play_btts.Add(image);
                image.Source = ImageSource.FromResource("CloudStreamForms.Resource.playBtt.png", Assembly.GetExecutingAssembly());
                if (Device.RuntimePlatform == Device.Android) {
                    image.Scale = 0.5f;
                }
                else {
                    image.Scale = 0.3f;
                }
            }
        }
        private void ImageGetGradient(object sender, PropertyChangingEventArgs e)
        {
            FFImageLoading.Forms.CachedImage image = ((FFImageLoading.Forms.CachedImage)sender);
            if (play_btts.Where(t => t.Id == image.Id).Count() == 0) {
                play_btts.Add(image);
                image.Source = GetGradient();
            }
        }

        async void SetProgress(int sec, int sender)
        {
            EpisodeResult ep = epView.MyEpisodeResultCollection[sender];

            ep.LoadedLinks = false;

            double add = 0;
            for (int i = 0; i < 100 * sec; i++) {
                await Task.Delay(10);
                add = ((double)i / (double)sec) / (double)100;
                MainThread.BeginInvokeOnMainThread(() => {

                    //  ep.Progress = add;
                });

                print(add + "|" + ep.Progress);
            }
            ep.LoadedLinks = true;
        }

        private void ImageButton_Clicked(object sender, EventArgs e) // LOAD
        {
            if (!SameAsActiveMovie()) return;
            //print(activeMovie.title.movies123MetaData.seasonData.Count);
            // activeMovie = currentMovie;
            //ProgressBar progressBar = (ProgressBar)((Grid)((Grid)((ImageButton)sender).Children.ElementAt(0)).Children.ElementAt(0)).Children.ElementAt(2)).Children.ElementAt(0);

            EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            string hasDownloadedFile = App.GetKey("Download", GetId(episodeResult), "");
            if (hasDownloadedFile != "") {
                Download.PlayFile(hasDownloadedFile, episodeResult.Title);
            }
            else {
                LoadLinksForEpisode(episodeResult);
            }
            episodeView.SelectedItem = null;
            //MainThread.BeginInvokeOnMainThread(() => {
            //   epView.MyEpisodeResultCollection[((EpisodeResult)((ImageButton)sender).BindingContext).Id].Progress = 1;


            // print(epView.MyEpisodeResultCollection[((EpisodeResult)((ImageButton)sender).BindingContext).Id].Progress + "-->><<");
            //});

        }

        bool loadingLinks = false;

        async Task<EpisodeResult> LoadLinksForEpisode(EpisodeResult episodeResult, bool autoPlay = true, bool overrideLoaded = false)
        {
            if (loadingLinks) return episodeResult;

            if (episodeResult.LoadedLinks && !overrideLoaded) {
                print("OPEN : " + episodeResult.Title);
                /*
                if (!CheckIfURLIsValid(episodeResult.loadResult.url)) {
                    try {
                        //LoadResult cl = episodeResult.loadResult;

                      //  episodeResult.loadResult = new LoadResult() { url = episodeResult.mirrosUrls[0], loadSelection = cl.loadSelection, subtitleUrl = cl.subtitleUrl };
                    }
                    catch (Exception) {
                    }

                }*/
                if (episodeResult.mirrosUrls.Count > 0) {

                    if (autoPlay) { PlayEpisode(episodeResult); }
                }
                else {
                    episodeView.SelectedItem = null;

                    App.ShowToast(errorEpisodeToast);
                }
                //PlayEpisode(episodeResult);

                //  App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls.First(), episodeResult.Mirros.First());

                /*
                if (CheckIfURLIsValid(episodeResult.loadResult.url)) {
                        PlayVLCWithSingleUrl(episodeResult.loadResult.url, episodeResult.Title);
                    }
                    else {
                        // VALID URL ERROR
                    }
*/

            }
            else {
                GetEpisodeLink(isMovie ? -1 : (episodeResult.Id + 1), currentSeason, isDub: isDub);

                await Device.InvokeOnMainThreadAsync(async () => {
                    /* progressBars[episodeResult.Id].Progress = 0.01f;
                     progressBars[episodeResult.Id].IsVisible = true;
                     await progressBars[episodeResult.Id].ProgressTo(1, 3000, Easing.SinIn);
                     progressBars[episodeResult.Id].Progress = 1;
                     progressBars[episodeResult.Id].IsVisible = false;*/
                    //  LoadingStack.IsEnabled = true;
                    NormalStack.IsEnabled = false;
                    // LoadingStack.IsVisible = true;
                    // NormalStack.IsVisible = false;
                    //  NormalStack.Opacity = 0.3f;
                    loadingLinks = true;

                    UserDialogs.Instance.ShowLoading("Loading links...", MaskType.Gradient);

                    //UserDialogs.Instance.load
                    await Task.Delay(LoadingMiliSec);
                    UserDialogs.Instance.HideLoading();

                    loadingLinks = false;

                    if (SameAsActiveMovie()) {
                        currentMovie = activeMovie;
                    }

                    //    LoadingStack.IsEnabled = false;
                    NormalStack.IsEnabled = true;
                    //      LoadingStack.IsVisible = false;
                    NormalStack.Opacity = 1f;

                    //NormalStack.IsVisible = true;
                    //   print("MAINOS");

                    if (episodeResult == null) {
                        print("NULLEP"); episodeView.SelectedItem = null;

                        App.ShowToast(errorEpisodeToast);

                    }
                    else {
                        if (episodeResult.mirrosUrls == null) {
                            print("NULLE2");
                            episodeView.SelectedItem = null;

                            App.ShowToast(errorEpisodeToast);

                        }
                        else {
                            print("NULLEP3");

                            print("LINKCOUNT: " + episodeResult.mirrosUrls.Count);
                            if (episodeResult.mirrosUrls.Count > 0) {

                                if (autoPlay) { PlayEpisode(episodeResult); }
                                episodeResult.LoadedLinks = true;
                            }
                            else {
                                print("NULL3P3");
                                episodeView.SelectedItem = null;

                                App.ShowToast(errorEpisodeToast);
                            }
                        }


                    }

                });
                /*
                if (progressBars[episodeResult.Id].Progress == 0) {
                    GetEpisodeLink(isMovie ? -1 : (episodeResult.Id + 1), currentSeason, isDub: isDub);

                    Device.InvokeOnMainThreadAsync(async () => {
                        progressBars[episodeResult.Id].Progress = 0.01f;
                        progressBars[episodeResult.Id].IsVisible = true;
                        await progressBars[episodeResult.Id].ProgressTo(1, 3000, Easing.SinIn);
                        progressBars[episodeResult.Id].Progress = 1;
                        progressBars[episodeResult.Id].IsVisible = false;
                        if (episodeResult.mirrosUrls.Count > 0) {
                            App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls, episodeResult.Mirros);
                            episodeResult.LoadedLinks = true;
                        }
                    });
                }*/

                // SetProgress(3, ((EpisodeResult)((ImageButton)sender).BindingContext).Id);

            }
            return episodeResult;
        }

        void ToggleEpisode(EpisodeResult episodeResult)
        {
            string id = GetId(episodeResult);
            if (id != "") {
                if (App.KeyExists("ViewHistory", id)) {
                    App.RemoveKey("ViewHistory", id);
                }
                else {
                    App.SetKey("ViewHistory", id, true);
                }
            }
            //episodeResult.MainTextColor = App.KeyExists("ViewHistory", id) ? primaryLightColor : "#ffffff";
            SetColor(episodeResult);
            ForceUpdate();
        }

        void PlayEpisode(EpisodeResult episodeResult)
        {
            string id = GetId(episodeResult);
            if (id != "") {
                if (ViewHistory) {
                    App.SetKey("ViewHistory", id, true);
                    SetColor(episodeResult);
                    // episodeResult.MainTextColor = primaryLightColor;

                    // FORCE UPDATE
                    ForceUpdate();
                }
            }


            string _sub = "";
            if (currentMovie.subtitles != null) {
                if (currentMovie.subtitles.Count > 0) {
                    _sub = currentMovie.subtitles[0].data;
                }
            }
            App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls, episodeResult.Mirros, _sub);
        }

        void ForceUpdate()
        {

            var _e = epView.MyEpisodeResultCollection.ToList();
            Device.BeginInvokeOnMainThread(() => {

                epView.MyEpisodeResultCollection.Clear();

                for (int i = 0; i < _e.Count; i++) {
                    // print("Main::" + _e[i].MainTextColor);
                    EpisodeResult e = _e[i];
                    epView.MyEpisodeResultCollection.Add(new EpisodeResult() { Title = e.Title, Description = e.Description, MainTextColor = e.MainTextColor, MainDarkTextColor = e.MainDarkTextColor, Rating = e.Rating, epVis = e.epVis, Id = e.Id, LoadedLinks = e.LoadedLinks, Mirros = e.Mirros, mirrosUrls = e.mirrosUrls, ogTitle = e.ogTitle, PosterUrl = e.PosterUrl, Progress = e.Progress, subtitles = e.subtitles, subtitlesUrls = e.subtitlesUrls }); // , loadResult = e.loadResult
                }
            });

        }

        private void ProgressBar_BindingContextChanged(object sender, EventArgs e)
        {
            // print("CONTEXT CHANGED");
            /*
             ProgressBar pBar = (ProgressBar)sender;
             if (pBar.BindingContext is EpisodeResult) {
                 int id = ((EpisodeResult)pBar.BindingContext).Id;
                 if (id >= progressBars.Count) {
                     progressBars.Add(pBar);
                 }
                 else {
                     progressBars[id] = pBar;
                 }
             }*/
        }

        // public string ChromeColor { set; get; }//{ get { return (MainChrome.IsPendingConnection || MainChrome.IsCastingVideo) ? "#303F9F" : "#ffffff"; } set { } }

        async void EpisodeSettings(EpisodeResult episodeResult)
        {

            if (!episodeResult.LoadedLinks) {
                try {
                    await LoadLinksForEpisode(episodeResult, false);

                }
                catch (Exception) {

                }

            }
            if (loadingLinks) {
                await Task.Delay(LoadingMiliSec + 40);
            }
            if (!episodeResult.LoadedLinks) {
                App.ShowToast(errorEpisodeToast); episodeView.SelectedItem = null;

                return;
            }

            string action = "";

            bool hasDownloadedFile = App.KeyExists("Download", GetId(episodeResult));
            string downloadKeyData = "";

            List<string> actions = new List<string>() { "Play", "Download", "Download Subtitles", "Copy Link", "Reload" };

            if (hasDownloadedFile) {
                downloadKeyData = App.GetKey("Download", GetId(episodeResult), "");
                actions.Add("Play Downloaded File"); actions.Add("Delete Downloaded File");
            }
            if (MainChrome.IsConnectedToChromeDevice) {
                actions.Insert(0, "Chromecast");
            }

            action = await DisplayActionSheet(episodeResult.Title, "Cancel", null, actions.ToArray());

            if (action == "Chromecast") {
                print("STAARTYCHROMECAST");
                chromeResult = episodeResult;
                chromeMovieResult = currentMovie;
                bool succ = false;
                int count = -1;
                episodeView.SelectedItem = null;

                while (!succ) {
                    count++;

                    if (count >= episodeResult.Mirros.Count) {
                        succ = true;
                    }
                    else {
                        succ = await MainChrome.CastVideo(episodeResult.mirrosUrls[count], episodeResult.Mirros[count]);
                    }
                }
                ChromeCastPage.currentSelected = count;

                print("CASTOS");
                /*
                string download = await DisplayActionSheet("Download", "Cancel", null, episodeResult.Mirros.ToArray());
                for (int i = 0; i < episodeResult.Mirros.Count; i++) {
                    if (episodeResult.Mirros[i] == download) {
                        MainChrome.CastVideo(episodeResult.mirrosUrls[i], episodeResult.Mirros[i]);
                    }
                }*/
            }

            if (action == "Play") {
                PlayEpisode(episodeResult);
            }
            else if (action == "Copy Link") {
                string copy = await DisplayActionSheet("Copy Link", "Cancel", null, episodeResult.Mirros.ToArray());
                for (int i = 0; i < episodeResult.Mirros.Count; i++) {
                    if (episodeResult.Mirros[i] == copy) {
                        await Clipboard.SetTextAsync(episodeResult.mirrosUrls[i]);
                        App.ShowToast("Copied Link to Clipboard");
                    }
                }
            }
            else if (action == "Download") {
                string download = await DisplayActionSheet("Download", "Cancel", null, episodeResult.Mirros.ToArray());
                for (int i = 0; i < episodeResult.Mirros.Count; i++) {
                    if (episodeResult.Mirros[i] == download) {
                        string s = episodeResult.mirrosUrls[i];
                        Thread t = new Thread(() => {
                            UserDialogs.Instance.ShowLoading("Checking link...", MaskType.Gradient);
                            //UserDialogs.Instance.load
                            double fileSize = CloudStreamCore.GetFileSize(s);
                            UserDialogs.Instance.HideLoading();
                            if (fileSize > 1) {
                                string dpath = App.DownloadUrl(s, episodeResult.Title + ".mp4", true, "/" + GetPathFromType(),"Download complete!",true,episodeResult.Title);
                                //  string ppath = App.DownloadUrl(episodeResult.PosterUrl, "epP" + episodeResult.Title + ".jpg", false, "/Posters");
                                // string mppath = App.DownloadUrl(currentMovie.title.hdPosterUrl, "hdP" + episodeResult.Title + ".jpg", false, "/TitlePosters");
                                string mppath = currentMovie.title.hdPosterUrl;
                                string ppath = episodeResult.PosterUrl;
                                string key = "_dpath=" + dpath + "|||_ppath=" + ppath + "|||_mppath=" + mppath + "|||_descript=" + episodeResult.Description + "|||_maindescript=" + currentMovie.title.description + "|||_epCounter=" + episodeResult.Id + "|||_epId=" + GetId(episodeResult) + "|||_movieId=" + currentMovie.title.id + "|||_title=" + episodeResult.Title + "|||_movieTitle=" + currentMovie.title.name + "|||=EndAll";
                                print("DKEY: " + key);
                                App.SetKey("Download", GetId(episodeResult), key);
                                App.ShowToast("Download Started - " + fileSize + "MB");
                                App.SetKey("DownloadSize", GetId(episodeResult), fileSize);
                                SetColor(episodeResult);
                                ForceUpdate();
                            }
                            else {
                                EpisodeSettings(episodeResult);
                                App.ShowToast("Download Failed");
                                ForceUpdate();
                            }

                        }) { Name = "DownloadThread" };
                        t.Start();
                    }
                }
            }
            else if (action == "Reload") {
                try {
                    await LoadLinksForEpisode(episodeResult, false, true);
                }
                catch (Exception) {
                }

                await Task.Delay(LoadingMiliSec + 40);

                if (!episodeResult.LoadedLinks) {
                    episodeView.SelectedItem = null;
                    App.ShowToast(errorEpisodeToast);
                    return;
                }
                EpisodeSettings(episodeResult);
            }
            else if (action == "Play Downloaded File") {
                Download.PlayFile(downloadKeyData, episodeResult.Title);
            }
            else if (action == "Delete Downloaded File") {
                Download.DeleteFileFromFolder(downloadKeyData, "Download", GetId(episodeResult));
                SetColor(episodeResult);
                ForceUpdate();
            }
            else if (action == "Download Subtitles") {

                TempThred tempThred = new TempThred();
                tempThred.typeId = 4; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {

                        string id = GetId(episodeResult);
                        if (id.Replace(" ", "") == "") { App.ShowToast("Id not found"); return; }

                        string s = DownloadSubtitle(id);
                        if (s == "") {
                            App.ShowToast("No Subtitles Found");
                            return;
                        }
                        else {
                            App.DownloadFile(s, episodeResult.Title + ".srt", true, "/Subtitles");
                            App.ShowToast("Subtitles Downloaded");
                        }

                        //if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Subtitle Download";
                tempThred.Thread.Start();


            }
            episodeView.SelectedItem = null;

        }

        public string GetId(EpisodeResult episodeResult)
        {
            //  print(episodeResult.Id + "|" + currentMovie.episodes.Count);

            try {
                return (currentMovie.title.movieType == MovieType.TVSeries || currentMovie.title.movieType == MovieType.Anime) ? currentMovie.episodes[episodeResult.Id].id : currentMovie.title.id;

            }
            catch (Exception) {
                return episodeResult.Id + "Extra=" + ToDown(episodeResult.Title) + "=EndAll";
            }
        }

        string GetPathFromType()
        {
            string path = "Movies";
            if (currentMovie.title.movieType == MovieType.Anime) {
                path = "Anime";
            }
            else if (currentMovie.title.movieType == MovieType.TVSeries) {
                path = "TVSeries";
            }
            return path;
        }

        private void ViewCell_Tapped(object sender, EventArgs e) // MORE INFO HERE
        {
            EpisodeResult episodeResult = ((EpisodeResult)(((ViewCell)sender).BindingContext));

            if (toggleViewState) {
                ToggleEpisode(episodeResult);
                episodeView.SelectedItem = null;
            }
            else {
                EpisodeSettings(episodeResult);
            }
            // episodeView.SelectedItem = null;

        }
        bool toggleViewState = false;
        private void ViewToggle_Clicked(object sender, EventArgs e)
        {
            toggleViewState = !toggleViewState;
            ChangeViewToggle();
        }

        void ChangeViewToggle()
        {
            ViewToggle.Source = GetImageSource((toggleViewState ? "viewOffIcon.png" : "viewOnIcon.png"));
            ViewToggle.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(toggleViewState ? DARK_BLUE_COLOR : LIGHT_BLACK_COLOR)) };
        }

    }

}

public class MainEpisodeView
{
    private ObservableCollection<EpisodeResult> _MyEpisodeResultCollection;
    public ObservableCollection<EpisodeResult> MyEpisodeResultCollection { set { Added?.Invoke(null, null); _MyEpisodeResultCollection = value; } get { return _MyEpisodeResultCollection; } }

    public event EventHandler Added;

    public MainEpisodeView()
    {
        MyEpisodeResultCollection = new ObservableCollection<EpisodeResult>();
    }
}



