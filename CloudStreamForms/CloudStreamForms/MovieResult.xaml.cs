using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.Main;
using System.Reflection;
using System.Collections.ObjectModel;
using CloudStreamForms.Models;
using Xamarin.Essentials;
using CloudStreamForms;
using static CloudStreamForms.App;
using static CloudStreamForms.MainPage;
using static CloudStreamForms.Settings;
using System.Threading;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieResult : ContentPage
    {

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
            //Navigation.PopModalAsync();
            // return true;
            return base.OnBackButtonPressed();
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



        private void StarBttClicked(object sender, EventArgs e)
        {
            //bool SetValue = !App.GetKey("Bookmark", currentMovie.title.id, false);
            //  App.SetKey("Bookmark", currentMovie.title.id, SetValue);
            bool keyExists = App.KeyExists("BookmarkData", currentMovie.title.id);
            if (keyExists) {
                App.RemoveKey("BookmarkData", currentMovie.title.id);
            }
            else {
                App.SetKey("BookmarkData", currentMovie.title.id, "Name=" + currentMovie.title.name + "|||PosterUrl=" + currentMovie.title.hdPosterUrl + "|||Id=" + currentMovie.title.id + "|||TypeId=" + ((int)currentMovie.title.movieType) + "|||ShortEpView=" + currentMovie.title.shortEpView + "|||=EndAll");
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
                string _s = Main.ShareMovieCode(currentMovie.title.id + "Name=" + currentMovie.title.name + "=EndAll");
                if (_s != "") {
                    Clipboard.SetTextAsync(_s);

                    App.ShowToast("Copied Link to Clipboard");
                }
            }
        }



        void ChangeStar(bool? overrideBool = null)
        {

            bool keyExists = false;
            if (overrideBool == null) {
                keyExists = App.KeyExists("BookmarkData", currentMovie.title.id);
            }
            else {
                keyExists = (bool)overrideBool;
            }
            StarBtt.Source = GetImageSource((keyExists ? "bookmarkedBtt.png" : "notBookmarkedBtt.png"));
            Device.BeginInvokeOnMainThread(() => {
                StarBtt.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(keyExists ? primaryColor : defColor)) };
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
                SubtitleBtt.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(res ? primaryColor : defColor)) };
            });
        }

        public MovieResult()
        {

            InitializeComponent();

            mainPoster = Search.mainPoster;

            Gradient.Source = GetImageSource("gradient.png");
            IMDbBtt.Source = GetImageSource("imdbIcon.png");
            MALBtt.Source = GetImageSource("MALIcon.png");
            ShareBtt.Source = GetImageSource("shareIcon.png");
            StarBtt.Source = GetImageSource("notBookmarkedBtt.png");
            SubtitleBtt.Source = GetImageSource("subtitleIcon.png");
            //ViewToggle.Source = GetImageSource("viewOnState.png");
            ChangeViewToggle();
            ChangeSubtitle();

            //NameLabel.Text = activeMovie.title.name;
            NameLabel.Text = mainPoster.name;
            RatingLabel.Text = mainPoster.year;

            titleLoaded += MovieResult_titleLoaded;
            trailerLoaded += MovieResult_trailerLoaded;
            epsiodesLoaded += MovieResult_epsiodesLoaded;


            // TrailerBtt.Clicked += TrailerBtt_Clicked;
            Gradient.Clicked += TrailerBtt_Clicked;
            linkAdded += MovieResult_linkAdded;
            linksProbablyDone += MovieResult_linksProbablyDone;
            movie123FishingDone += MovieResult_movie123FishingDone;

            if (Device.RuntimePlatform == Device.UWP) {
                //QuickMenu.WidthRequest = 500;
            }

            //  myEpisodeResultCollection;

            //  FakePlayBtt.Source = ImageSource.FromUri(new System.Uri("https://m.media-amazon.com/images/M/MV5BMjEyNzQ0MjE2OF5BMl5BanBnXkFtZTcwMTkyNjE5Ng@@._V1_CR0,60,640,360_AL_UX477_CR0,0,477,268_AL_.jpg"));
            //FakePlayBtt.Source = ImageSource.FromResource("CloudStreamForms.Resource.playBtt.png");
            BackgroundColor = Color.Black;
            #region notUsed
            /*
            DataTemplate dataTemplate = new ListViewDataTemplateSelector();
            
            episodeView = new ListView {
                //VerticalOptions = LayoutOptions.Start,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                // TranslationY = heightRequestAddEpisode / 2f,
                // Source of data items.
                ItemsSource = MyEpisodeResultCollection,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = dataTemplate, /*new DataTemplate(() => {

                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    Label desLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Title");
                    desLabel.SetBinding(Label.TextProperty, "Description");
                    desLabel.FontSize = nameLabel.FontSize / 1.2f;
                    
                    desLabel.TextColor = Color.Gray;
                    nameLabel.TranslationX = 5;
                    desLabel.TranslationX = 5;
                    ProgressBar progressBar = new ProgressBar();
                    progressBar.IsVisible = false;
                    progressBar.SetBinding(ProgressBar.ProgressProperty, "Progress");

                    Picker linkPicker = new Picker();
                    // linkPicker.Items.Add("Mirror 1");
                    //  linkPicker.Items.Add("Mirror 2");
                    // linkPicker.Items.Add("Mirror 3");
                    linkPicker.SetBinding(Picker.ItemsSourceProperty, "Mirros");


                    Picker subPicker = new Picker();
                    subPicker.SetBinding(Picker.ItemsSourceProperty, "Subtitles");

                    
                    //   subPicker.Items.Add("English");
                    //   subPicker.Items.Add("Swedish");

                    Picker exePicker = new Picker();
                    exePicker.Items.Add("Play");
                    exePicker.Items.Add("Download");
                    exePicker.Items.Add("Copy Link");
                    exePicker.Items.Add("Copy Subtitle Link");

                    // Button playBtt = new Button() { Text="Play" };

                    Grid grid = new Grid();
                    grid.Children.Add(linkPicker);
                    grid.Children.Add(subPicker);
                    grid.Children.Add(exePicker);
                    //   grid.Children.Add(playBtt);

                    Grid.SetColumn(subPicker, 1);
                    Grid.SetColumn(exePicker, 2);

                    grid.SetBinding(Grid.IsVisibleProperty, "EpVis");

                    try {
                        exePicker.SelectedIndex = 0;
                        subPicker.SelectedIndex = 0;
                        linkPicker.SelectedIndex = 0;
                    }
                    catch (Exception) {

                    }
                    //grid.IsVisible = true;
                    //    Grid.SetColumn(playBtt, 3);

                    //nameLabel.SetBinding(Label.d, "Extra");


                    // Return an assembled ViewCell.
                    return new ViewCell {
                        View = new StackLayout {
                            // Padding = new Thickness(0, 5),
                            HeightRequest = heightRequestPerEpisode,
                            MinimumHeightRequest = heightRequestPerEpisode,
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Start,
                            Children =
                            {
                                //boxView,
                                new StackLayout
                                {
                                    VerticalOptions = LayoutOptions.Start,
                                    Spacing = 0,
                                    Children =
                                    {
                                        nameLabel,
                                        desLabel,
                                        grid,
                                        progressBar,
                                        //birthdayLabel
                                    }
                                }
                            }
                        }
                    };
                })    star/

            };
            episodeView.ItemTapped += ListView_ItemTapped;
       //     episodeView.HeightRequest = 0;
            episodeView.MinimumHeightRequest = 0;
            episodeView.VerticalOptions = LayoutOptions.Start;
            episodeView.HasUnevenRows = true;*/
            #endregion
            MALBtt.IsVisible = false;
            MALTxt.IsVisible = false;
            epView = new MainEpisodeView();
            SetHeight();

            if (Device.RuntimePlatform == Device.UWP) {
                DubPicker.TranslationY = 13.5;
            }

            BindingContext = epView;

            // listView.HeightRequest = 100;
            // starPng.Source = 
            //MGRID.Children.Add(listView);
            //Grid.SetRow(listView, 6);
            //  EpisodeView = listView;
            /*
            this.Content = new ScrollView {
                Content = new StackLayout() {
                    Children =
                    {
                        XGRID,
                    MGRID,
                   // episodeView,
                   SLay,
                    RText,
                    MScroll,
                    }
                }
            };
            */
            //  Grid.SetRow(RowSeason, 0);
            episodeView.ItemAppearing += EpisodeView_ItemAppearing;
            SizeChanged += MainPage_SizeChanged;
            // Grid.SetRow(RowDub, 0);
            //  Grid.SetRow(RowMal, 0);

            // episodeView.HeightRequest = 0;

            //episodeView.HeightRequest = 10000;
            // print(mainPoster.name + "|" + mainPoster.url + "|" + mainPoster.year);
            GetImdbTitle(mainPoster);
            //  episodeView.HeightRequest = 0;
            //  AbsoluteLayout.SetLayoutFlags(episodeView, AbsoluteLayoutFlags.PositionProportional);
            //   AbsoluteLayout.SetLayoutBounds(episodeView, new Rectangle(0f, 0f, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));






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
                List<string> hexColors = new List<string>() { "#ffffff", primaryLightColor, "#e5e598" };
                List<string> darkHexColors = new List<string>() { "#808080", primaryColor, "#d3c450" };
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



        //This sends the episodes to the void, hence why it's "public void"
        public void ClearEpisodes()
        {
            episodeView.ItemsSource = null;
            epView.MyEpisodeResultCollection.Clear();
            episodeView.ItemsSource = epView.MyEpisodeResultCollection;
            // episodeView.HeightRequest = 0;
            totalHeight = 0;
            counter = 0;
            play_btts = new List<FFImageLoading.Forms.CachedImage>();
            grids = new List<Grid>();
            progressBars = new List<ProgressBar>();
            //  grids.Clear();
            // gridsSize.Clear();
            SetHeight();
        }
        double totalHeight = 0;
        int counter = 0;

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



        private void MovieResult_movie123FishingDone(object sender, Movie e)
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


        private void MovieResult_linkAdded(object sender, int e)
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
                RatingLabel.Text = rYear + " | " + e.title.runtime + " | " + extra + "★ " + e.title.rating;
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
                    SeasonPicker.SelectedIndex = 0;
                    currentSeason = 1;
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
                MALBtt.IsVisible = CurrentMalLink != "";
                MALTxt.IsVisible = MALBtt.IsVisible;
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

                            activeMovie.title.MALData.currentActiveMaxEpsPerSeason.Add(_epCount);
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
            Main.OpenBrowser("https://www.imdb.com/title/" + mainPoster.url);
        }
        private void MAL_Clicked(object sender, EventArgs e)
        {
            if (!SameAsActiveMovie()) return;
            Main.OpenBrowser(CurrentMalLink);
        }


        List<FFImageLoading.Forms.CachedImage> play_btts = new List<FFImageLoading.Forms.CachedImage>();
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
                    NormalStack.Opacity = 0.3f;
                    loadingLinks = true;
                    await Task.Delay(LoadingMiliSec);
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
                        App.ShowToast(errorEpisodeToast);

                    }
                    else {
                        if (episodeResult.mirrosUrls == null) {
                            App.ShowToast(errorEpisodeToast);

                        }
                        else {
                            print("LINKCOUNT: " + episodeResult.mirrosUrls.Count);
                            if (episodeResult.mirrosUrls.Count > 0) {

                                if (autoPlay) { PlayEpisode(episodeResult); }
                                episodeResult.LoadedLinks = true;
                            }
                            else {
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

        async void EpisodeSettings(EpisodeResult episodeResult)
        {
            episodeView.SelectedItem = null;

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
                App.ShowToast(errorEpisodeToast);
                return;
            }

            string action = "";

            bool hasDownloadedFile = App.KeyExists("Download", GetId(episodeResult));
            string downloadKeyData = "";
            if (hasDownloadedFile) {
                downloadKeyData = App.GetKey("Download", GetId(episodeResult), "");
                action = await DisplayActionSheet(episodeResult.Title, "Cancel", null, "Play", "Download", "Download Subtitles", "Copy Link", "Reload", "Play Downloaded File", "Delete Downloaded File");
            }
            else {
                action = await DisplayActionSheet(episodeResult.Title, "Cancel", null, "Play", "Download", "Download Subtitles", "Copy Link", "Reload");
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

                        double fileSize = Main.GetFileSize(episodeResult.mirrosUrls[i]);
                        if (fileSize > 1) {

                            string dpath = App.DownloadUrl(episodeResult.mirrosUrls[i], episodeResult.Title + ".mp4", true, "/" + GetPathFromType());
                            //  string ppath = App.DownloadUrl(episodeResult.PosterUrl, "epP" + episodeResult.Title + ".jpg", false, "/Posters");
                            // string mppath = App.DownloadUrl(currentMovie.title.hdPosterUrl, "hdP" + episodeResult.Title + ".jpg", false, "/TitlePosters");
                            string mppath = currentMovie.title.hdPosterUrl;
                            string ppath = episodeResult.PosterUrl;
                            string key = "_dpath=" + dpath + "|||_ppath=" + ppath + "|||_mppath=" + mppath + "|||_descript=" + episodeResult.Description + "|||_maindescript=" + currentMovie.title.description + "|||_epCounter=" + episodeResult.Id + "|||_epId=" + GetId(episodeResult) + "|||_movieId=" + currentMovie.title.id + "|||_title=" + episodeResult.Title + "|||_movieTitle=" + currentMovie.title.name + "|||=EndAll";
                            print("DKEY: " + key);
                            App.SetKey("Download", GetId(episodeResult), key);
                            App.ShowToast("Download Started - " + fileSize + "MB");
                            SetColor(episodeResult);
                            ForceUpdate();
                        }
                        else {
                            EpisodeSettings(episodeResult);
                            App.ShowToast("Download Failed"); 
                            ForceUpdate();
                        }
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
            episodeView.SelectedItem = null;

            if (toggleViewState) {
                ToggleEpisode(episodeResult);
            }
            else {
                EpisodeSettings(episodeResult);
            }

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
            ViewToggle.Transformations = new List<FFImageLoading.Work.ITransformation>() { (new FFImageLoading.Transformations.TintTransformation(toggleViewState ? primaryColor : defColor)) };
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



