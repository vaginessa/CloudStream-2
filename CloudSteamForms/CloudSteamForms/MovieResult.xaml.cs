using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudSteamForms.Main;
using System.Reflection;
using System.Collections.ObjectModel;
using CloudSteamForms.Models;
using Xamarin.Essentials;

namespace CloudSteamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieResult : ContentPage
    {
        public Poster mainPoster;
        public ObservableCollection<EpisodeResult> myEpisodeResultCollection;
        public string trailerUrl = "";
        List<ImageButton> recBtts = new List<ImageButton>();
        List<Poster> recomendedPosters { set { currentMovie.title.recomended = value; } get { return currentMovie.title.recomended; } }  //= new List<Poster>();
        bool loadedTitle = false;
        int currentSeason = 0;
        ListView listView;
        const int heightRequestPerEpisode = 120;
        const int heightRequestAddEpisode = 40;
        const int heightRequestAddEpisodeAndroid = 0;

        bool isMovie = false;
        Movie currentMovie = new Movie();
        bool isDub = true;

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

            //Navigation.PopModalAsync();
            // return true;
            return base.OnBackButtonPressed();
        }

        void SetRows()
        {
            MainThread.BeginInvokeOnMainThread(() => {

                //  Grid.SetRow(RowSeason, SeasonPicker.IsVisible ? 3 - ((DubPicker.IsVisible ? 0 : 1) + (MALBtt.IsVisible ? 0 : 1)) : 0);
                Grid.SetRow(RowDub, SeasonPicker.IsVisible ? (1 - (MALBtt.IsVisible ? 0 : 1)) : 0);
                Grid.SetRow(RowMal, MALBtt.IsVisible ? 0 : 0);

                // MALBtt.IsVisible = MALBtt.IsVisible;
            });

        }

        public MovieResult()
        {

            InitializeComponent();
            myEpisodeResultCollection = new ObservableCollection<EpisodeResult>() { };

            mainPoster = Search.mainPoster;

           Gradient.Source = ImageSource.FromResource("CloudSteamForms.Resource.gradient.png");

            //NameLabel.Text = activeMovie.title.name;
            NameLabel.Text = mainPoster.name;
            RatingLabel.Text = mainPoster.year;

            titleLoaded += MovieResult_titleLoaded;
            trailerLoaded += MovieResult_trailerLoaded;
            epsiodesLoaded += MovieResult_epsiodesLoaded;


            TrailerBtt.Clicked += TrailerBtt_Clicked;
            Gradient.Clicked += TrailerBtt_Clicked;
            linkAdded += MovieResult_linkAdded;
            linksProbablyDone += MovieResult_linksProbablyDone;
            //  myEpisodeResultCollection;

            //  FakePlayBtt.Source = ImageSource.FromUri(new System.Uri("https://m.media-amazon.com/images/M/MV5BMjEyNzQ0MjE2OF5BMl5BanBnXkFtZTcwMTkyNjE5Ng@@._V1_CR0,60,640,360_AL_UX477_CR0,0,477,268_AL_.jpg"));
            //FakePlayBtt.Source = ImageSource.FromResource("CloudSteamForms.Resource.playBtt.png");
            BackgroundColor = Color.Black;
            listView = new ListView {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TranslationY = heightRequestAddEpisode / 2f,
                // Source of data items.
                ItemsSource = myEpisodeResultCollection,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() => {

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
                    //nameLabel.SetBinding(Label.d, "Extra");
                    /*
                    Label birthdayLabel = new Label();
                    birthdayLabel.SetBinding(Label.TextProperty,
                        new Binding("Birthday", BindingMode.OneWay,
                            null, null, "Born {0:d}"));

                    BoxView boxView = new BoxView();
                    boxView.SetBinding(BoxView.ColorProperty, "FavoriteColor");*/

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
                                            progressBar,
                                            //birthdayLabel
                                        }
                                        }
                                }
                        }
                    };
                })
            };
            listView.ItemTapped += ListView_ItemTapped;
            listView.HeightRequest = 0;
            MALBtt.IsVisible = false;

            // listView.HeightRequest = 100;
            // starPng.Source = 
            //MGRID.Children.Add(listView);
            //Grid.SetRow(listView, 6);
            //  EpisodeView = listView;

            this.Content = new ScrollView {
                Content = new StackLayout() {
                    Children =
                    {
                        XGRID,
                    MGRID,
                    listView,
                    RText,
                    MScroll,
                    }
                }
            };


            //  Grid.SetRow(RowSeason, 0);
            Grid.SetRow(RowDub, 0);
            Grid.SetRow(RowMal, 0);
            // print(mainPoster.name + "|" + mainPoster.url + "|" + mainPoster.year);
            GetImdbTitle(mainPoster);
        }

        bool SameAsActiveMovie()
        {
            print(currentMovie.title.id + " || " + activeMovie.title.id);
            return currentMovie.title.id == activeMovie.title.id;
        }

        private void MovieResult_linksProbablyDone(object sender, Episode e)
        {
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
            }

        }


        private void MovieResult_linkAdded(object sender, int e)
        {
            //print(e + "|" + activeMovie.episodes[1].maxProgress);
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!SameAsActiveMovie()) return;
            activeMovie = currentMovie;
            GetEpisodeLink(isMovie ? -1 : (e.ItemIndex + 1), currentSeason);
        }

        private void TrailerBtt_Clicked(object sender, EventArgs e)
        {
            if (trailerUrl != null) {
                if (trailerUrl != "") {
                    PlayVLCWithSingleUrl(trailerUrl);
                }
            }
        }


        private void MovieResult_titleLoaded(object sender, Movie e)
        {
            if (loadedTitle) return;

            loadedTitle = true;
            isMovie = (e.title.movieType == MovieType.Movie || e.title.movieType == MovieType.AnimeMovie);
            currentMovie = e;
            print("Title loded" + " | " + mainPoster.name);
            MainThread.BeginInvokeOnMainThread(() => {
                try {
                    string souceUrl = e.title.trailers.First().posterUrl;
                    if(CheckIfURLIsValid(souceUrl)) {

                    TrailerBtt.Source = souceUrl;
                    }
                    else {
                        TrailerBtt.Source = ImageSource.FromResource("CloudSteamForms.Resource.gradient.png");
                    }
                }
                catch (Exception) {
                    TrailerBtt.Source = ImageSource.FromResource("CloudSteamForms.Resource.gradient.png");
                }

                if(!MainPage.RUNNING_WINDOWS) {
                    Gradient.IsVisible = false;
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
                DescriptionLabel.Text = e.title.description.Replace("\\u0027", "\'");

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

                // ---------------------------- RECOMENDATIONS ----------------------------

                foreach (var item in Recommendations.Children) { // SETUP
                    Grid.SetColumn(item, 0);
                    Grid.SetRow(item, 0);
                }
                Recommendations.Children.Clear();
                for (int i = 0; i < recomendedPosters.Count; i++) {
                    Poster p = e.title.recomended[i];
                    if (CheckIfURLIsValid(p.posterUrl)) {
                        ImageButton imageButton = new ImageButton() { HeightRequest = 100, Source = p.posterUrl, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Start };
                        recBtts.Add(imageButton);
                        Recommendations.Children.Add(recBtts[i]);

                    }


                }


                for (int i = 0; i < recBtts.Count; i++) { // CLICKED
                    recBtts[i].Clicked += (o, _e) => {
                        for (int z = 0; z < recBtts.Count; z++) {
                            if (((ImageButton)o).Id == recBtts[z].Id) {
                                Search.mainPoster = recomendedPosters[z];
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
                myEpisodeResultCollection.Clear();
                bool isLocalMovie = false;
                bool isAnime = currentMovie.title.movieType == MovieType.Anime;


                if (currentMovie.title.movieType != MovieType.Movie && currentMovie.title.movieType != MovieType.AnimeMovie) {
                    if (currentMovie.title.movieType != MovieType.Anime) {
                        for (int i = 0; i < currentEpisodes.Count; i++) {
                            myEpisodeResultCollection.Add(new EpisodeResult() { Title = (i + 1) + ". " + currentEpisodes[i].name, Id = i, Description = currentEpisodes[i].description, PosterUrl = currentEpisodes[i].posterUrl, Rating = currentEpisodes[i].rating, Progress = 0 });
                        }
                    }
                }
                else {
                    myEpisodeResultCollection.Add(new EpisodeResult() { Title = currentMovie.title.name, Description = currentMovie.title.description, Id = 0, PosterUrl = "", Progress = 0, Rating = "" });
                    isLocalMovie = true;
                }

                listView.HeightRequest = myEpisodeResultCollection.Count * heightRequestPerEpisode + (MainPage.RUNNING_WINDOWS ? heightRequestAddEpisode : heightRequestAddEpisodeAndroid);

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

                    if (dubExists) {
                        DubPicker.Items.Add("Dub");
                        print("BBBBBBBBBBBBBBBBBB");
                    }
                    if (subExists) {
                        DubPicker.Items.Add("Sub");
                        print("AAAAAAAAAAAAAAA");
                    }
                    if (DubPicker.Items.Count > 0) {
                        DubPicker.SelectedIndex = 0;
                    }
                    SetDubExist();
                }
                else {

                }

                DubPicker.IsVisible= DubPicker.Items.Count > 0;
                print(DubPicker.IsVisible + "ENABLED");
                MALBtt.IsVisible = CurrentMalLink != "";
                SetRows();

            });
        }

        void SetDubExist()
        {
            // string dstring = "";
            List<string> baseUrls = new List<string>();

            try {
                for (int q = 0; q < currentMovie.title.MALData.seasonData[currentSeason].seasons.Count; q++) {
                    MALSeason ms = currentMovie.title.MALData.seasonData[currentSeason].seasons[q];

                    if ((ms.dubExists && isDub) || (ms.subExists && !isDub)) {
                        //  dstring = ms.baseUrl;
                        string burl = ms.baseUrl.Replace("-dub", "");
                        if (!baseUrls.Contains(burl)) {
                            baseUrls.Add(burl);
                        }
                        //print("BASEURL " + ms.baseUrl);
                    }
                }
            }
            catch (Exception) {
            }

            if (baseUrls.Count > 0) {

                TempThred tempThred = new TempThred();
                tempThred.typeId = 6; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        int max = 0;
                        for (int i = 0; i < baseUrls.Count; i++) {
                            string dstring = baseUrls[i]; dstring = dstring.Replace("-dub", "") + (isDub ? "-dub" : "");
                            string d = DownloadString("https://www9.gogoanime.io/category/" + dstring);
                            if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                            string subMax = FindHTML(d, "class=\"active\" ep_start = \'", ">");
                            string maxEp = FindHTML(subMax, "ep_end = \'", "\'");//FindHTML(d, "<a href=\"#\" class=\"active\" ep_start = \'0\' ep_end = \'", "\'");
                            print("MAXEP" + maxEp);
                            max += int.Parse(maxEp);
                        }



                        MainThread.BeginInvokeOnMainThread(() => {
                            myEpisodeResultCollection.Clear();

                            for (int i = 0; i < max; i++) {
                                try {
                                    myEpisodeResultCollection.Add(new EpisodeResult() { Title = (i + 1) + ". " + currentEpisodes[i].name, Id = i, Description = currentEpisodes[i].description, PosterUrl = currentEpisodes[i].posterUrl, Rating = currentEpisodes[i].rating, Progress = 0 });

                                }
                                catch (Exception) {
                                    myEpisodeResultCollection.Add(new EpisodeResult() { Title = (i + 1) + ". " + "Episode #" + (i + 1), Id = i, Description = "", PosterUrl = "", Rating = "", Progress = 0 });

                                }
                            }
                            listView.HeightRequest = myEpisodeResultCollection.Count * heightRequestPerEpisode + heightRequestAddEpisode;
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
            print("trailer loaded");
            print(e);
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

            OpenBrowser("https://www.imdb.com/title/" + mainPoster.url);
        }
        private void MAL_Clicked(object sender, EventArgs e)
        {
            if (!SameAsActiveMovie()) return;
            print("da");
            OpenBrowser(CurrentMalLink);
        }
    }
}