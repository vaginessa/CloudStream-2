using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.MainPage;
using static CloudStreamForms.CloudStreamCore;
using CloudStreamForms.Models;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        const int POSTER_HIGHT = 98;
        const int POSTER_WIDTH = 67;
        public MainEpisodeView epView;
        public List<IMDbTopList> iMDbTopList = new List<IMDbTopList>();
        readonly List<string> genres = new List<string>() { "", "action", "adventure", "animation", "biography", "comedy", "crime", "drama", "family", "fantasy", "film-noir", "history", "horror", "music", "musical", "mystery", "romance", "sci-fi", "sport", "thriller", "war", "western" };
        readonly List<string> genresNames = new List<string>() { "Any", "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Drama", "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western" };

        readonly List<string> recomendationTypes = new List<string> { "Recommended", "Top 100" };



        public bool IsRecommended { get { return ImdbTypePicker.SelectedIndex == 0; } }

        public int currentImageCount = 0;
        public void LoadMoreImages(bool setHeight = true)
        {
            if (!Settings.Top100Enabled) return;
            Device.BeginInvokeOnMainThread(() => {
                int count = 10;//PosterAtScreenHight * PosterAtScreenWith * 3
                for (int i = 0; i < count; i++) {
                    if (currentImageCount >= iMDbTopList.Count) {
                        if (!Fething && !IsRecommended) {
                            GetFetch(currentImageCount + 1);
                        }
                        return;
                        //Feth more data
                    }
                    else {
                        try {
                            IMDbTopList x = iMDbTopList[currentImageCount];
                            bool add = true;
                            int selGen = MovieTypePicker.SelectedIndex - 1;
                            if (selGen != -1 && IsRecommended) {

                                if (!iMDbTopList[currentImageCount].contansGenres.Contains(selGen)) {
                                    add = false;
                                }
                            }
                            if (add) {

                                string img = ConvertIMDbImagesToHD(iMDbTopList[currentImageCount].img, IsRecommended ? 76 : 67, IsRecommended ? 113 : 98, 4);

                                AddEpisode(new EpisodeResult() { Description = x.descript, Title = (x.place > 0 ? (x.place + ". ") : "") + x.name + " | ★ " + x.rating.Replace(",", "."), Id = x.place, PosterUrl = img, extraInfo = "Id=" + x.id + "|||Name=" + x.name + "|||" }, false);
                            }

                        }
                        catch (Exception) {

                        }

                        // ItemGrid.Children.Add(cachedImages[currentImageCount]);
                        //SetChashedImagePos(ItemGrid.Children.Count - 1);
                    }
                    currentImageCount++;

                }
                if (setHeight) {
                    SetHeight();
                }
            });
        }


        public void GetFetchRecomended()
        {
            if (!Settings.Top100Enabled) return;

            if (!Fething) {
                Fething = true;
                TempThred tempThred = new TempThred();
                tempThred.typeId = 21; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        var f = FetchRecomended(bookmarkPosters.Select(t => t.id).ToList());
                        if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                        iMDbTopList.AddRange(f);
                        LoadMoreImages();
                    }
                    finally {
                        Fething = false;
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "GetFetchRecomended";
                tempThred.Thread.Start();
            }
        }

        bool _fething = false;
        public bool Fething
        {
            set {
                if ((value && epView.MyEpisodeResultCollection.Count == 0) || !value) {
                    Device.BeginInvokeOnMainThread(() => { LoadingIndicator.IsVisible = value; LoadingIndicator.IsEnabled = value; LoadingIndicator.IsRunning = value; });
                }
                _fething = value;
            }
            get { return _fething; }
        }
        public void GetFetch(int start = 1)
        {
            if (!Settings.Top100Enabled) return;

            Fething = true;
            TempThred tempThred = new TempThred();
            tempThred.typeId = 21; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread( () => {
                try {
                    var f = FetchTop100(new List<string>() { genres[MovieTypePicker.SelectedIndex] }, start);
                    if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    iMDbTopList.AddRange(f);
                    /*  Device.BeginInvokeOnMainThread(() => {

                          for (int i = 0; i < iMDbTopList.Count; i++) {

                              string img = ConvertIMDbImagesToHD(iMDbTopList[i].img, 67, 98, 4);
                              IMDbTopList x = iMDbTopList[i];

                              AddEpisode(new EpisodeResult() { Description = x.descript, Title = x.name + " | ★ " + x.rating.Replace(",", "."), Id = x.place, PosterUrl = img, extraInfo = "Id="+x.id+"|||Name="+x.name+"|||" }, false);
                          }

                    //  LoadMoreImages(false);
                    // LoadMoreImages();

                });*/
                    //if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS
                    LoadMoreImages();
                }
                finally {
                    Fething = false;
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "FethTop100";
            tempThred.Thread.Start();


        }
        private void episodeView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            episodeView.SelectedItem = null;
            //EpisodeResult episodeResult = ((EpisodeResult)((ListView)sender).BindingContext);
            //PlayEpisode(episodeResult);

        }
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            PushPageFromUrlAndName(FindShortend(episodeResult.extraInfo, "Id"), FindShortend(episodeResult.extraInfo, "Name"));

            //  PlayEpisode(episodeResult);
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = (EpisodeResult)(((ViewCell)sender).BindingContext);
            PushPageFromUrlAndName(FindShortend(episodeResult.extraInfo, "Id"), FindShortend(episodeResult.extraInfo, "Name"));

            // EpsodeShow(episodeResult);
            //EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            //App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls[0], episodeResult.Title);
            //episodeView.SelectedItem = null;
        }

        List<FFImageLoading.Forms.CachedImage> play_btts = new List<FFImageLoading.Forms.CachedImage>();
        private void Image_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {

            FFImageLoading.Forms.CachedImage image = ((FFImageLoading.Forms.CachedImage)sender);

            if (play_btts.Where(t => t.Id == image.Id).Count() == 0) {
                play_btts.Add(image);
                //image.Source = ImageSource.FromResource("CloudStreamForms.Resource.playBtt.png", Assembly.GetExecutingAssembly());
                /*
                if (Device.RuntimePlatform == Device.Android) {
                    image.Scale = 0.5f;
                }
                else {
                    image.Scale = 0.3f;
                }*/
            }

        }

        public Home()
        {
            InitializeComponent();
            epView = new MainEpisodeView();
            BindingContext = epView;

            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            MovieTypePicker.ItemsSource = genresNames;
            ImdbTypePicker.ItemsSource = recomendationTypes;
            MovieTypePicker.SelectedIndex = 0;
            UpdateBookmarks();

            ImdbTypePicker.SelectedIndexChanged += (o, e) => {
                ClearEpisodes();
                PurgeThreds(21);
                Fething = false;
                if (IsRecommended) {
                    GetFetchRecomended();
                }
                else {
                    GetFetch();
                }
            };
            MovieTypePicker.SelectedIndexChanged += (o, e) => {
                ClearEpisodes(!IsRecommended);
                if (IsRecommended) {
                    CloudStreamCore.Shuffle(iMDbTopList);
                    LoadMoreImages();
                }
                else {
                    PurgeThreds(21);
                    Fething = false;
                    GetFetch();
                }
                //GetFetchRecomended
                //  print(MovieTypePicker.SelectedIndex + "<<Selected");
            };
            ImdbTypePicker.SelectedIndex = bookmarkPosters.Count > 0 ? 0 : 1;

            episodeView.Scrolled += (o, e) => {
                double maxY = episodeView.HeightRequest - episodeView.Height;
                //print(maxY);
                if (e.ScrollY >= maxY - 200) {
                    LoadMoreImages();
                }
            };


            if (Device.RuntimePlatform == Device.UWP) {
                BlueSeperator.IsVisible = false;
                BlueSeperator.IsEnabled = false;
                OffBar.IsVisible = false;
                OffBar.IsEnabled = false;
            }
            else {
                OffBar.Source = App.GetImageSource("gradient.png"); OffBar.HeightRequest = 3; OffBar.HorizontalOptions = LayoutOptions.Fill; OffBar.ScaleX = 100; OffBar.Opacity = 0.3; OffBar.TranslationY = 9;
            }

            episodeView.VerticalScrollBarVisibility = Settings.ScrollBarVisibility;

            /*
            ImageScroller.Scrolled += (o, e) => {
                double maxY = ImageScroller.ContentSize.Height - ImageScroller.Height;
                if (e.ScrollY >= maxY - 200) {
                    LoadMoreImages();
                }

            };*/

            // MovieTypePicker.IsEnabled = false;
            //MovieTypePicker.IsVisible = false;
        }

        static string FindShortend(string d, string key)
        {
            return FindHTML(d, key + "=", "|||");
        }


        async Task AddEpisodeAsync(EpisodeResult episodeResult, bool setHeight = true, int delay = 10, bool setH = false)
        {
            AddEpisode(episodeResult, setHeight, setH);
            await Task.Delay(delay);
        }

        void AddEpisode(EpisodeResult episodeResult, bool setHeight = true, bool setH = false, bool addtoGrid = false)
        {
            epView.MyEpisodeResultCollection.Add(episodeResult);


            /*
           
            // Device.BeginInvokeOnMainThread(() => {
            var ff = new FFImageLoading.Forms.CachedImage {
                Source = episodeResult.PosterUrl,
                HeightRequest = POSTER_HIGHT,
                WidthRequest = POSTER_WIDTH,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Start,
                Transformations = {
                                new FFImageLoading.Transformations.RoundedTransformation(10,1,1.5,10,"#303F9F")
                            },
                InputTransparent = true,
            };

            cachedImages.Add(ff);*/
            if (addtoGrid) {
                /*
                ItemGrid.Children.Add(ff);
                if (setH) {
                    SetChashedImagePos(ItemGrid.Children.Count - 1);
                }*/
            }

            if (setHeight) {
                SetHeight();
            }

            //}); 


        }

        void ClearEpisodes(bool clearData = true)
        {
            //ItemGrid.Children.Clear();
            epView.MyEpisodeResultCollection.Clear();
            cachedImages.Clear();
            currentImageCount = 0;
            SetHeight();
            if (clearData) {

                iMDbTopList.Clear();
            }
        }

        public int PosterAtScreenWith { get { return (int)(currentWidth / (double)POSTER_WIDTH); } }
        public int PosterAtScreenHight { get { return (int)(currentWidth / (double)POSTER_HIGHT); } }
        List<FFImageLoading.Forms.CachedImage> cachedImages = new List<FFImageLoading.Forms.CachedImage>();

        void SetHeight()
        {
            /*
            ItemGrid.RowSpacing = POSTER_HIGHT / 2;

            for (int i = 0; i < cachedImages.Count; i++) {
                SetChashedImagePos(i);
            }*/

            Device.BeginInvokeOnMainThread(() => { episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight + 200; });

        }

        void SetChashedImagePos(int pos)
        {
            int x = pos % PosterAtScreenWith;
            int y = (int)(pos / PosterAtScreenWith);
            Grid.SetColumn(cachedImages[pos], x);
            Grid.SetRow(cachedImages[pos], y);
        }

        bool hasAppered = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!hasAppered) {
                /*
                Application.Current.MainPage.SizeChanged += (o, e) => {
                    SetHeight();
                };*/
            }
            UpdateBookmarks();
            Top100Stack.IsEnabled = Settings.Top100Enabled;
            Top100Stack.IsVisible = Settings.Top100Enabled;
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            hasAppered = true;
        }

        List<BookmarkPoster> bookmarkPosters = new List<BookmarkPoster>();
        void UpdateBookmarks()
        {
            double multi = 1.2;
            int height = 100;
            int width = 65;
            if (Device.RuntimePlatform == Device.UWP) {
                height = 130;
                width = 85;
            }

            height = (int)Math.Round(height * multi);
            width = (int)Math.Round(width * multi);

            Bookmarks.HeightRequest = height;
            List<string> keys = App.GetKeys<string>("BookmarkData");
            List<string> data = new List<string>();
            bookmarkPosters = new List<BookmarkPoster>();
            Bookmarks.Children.Clear();
            for (int i = 0; i < keys.Count; i++) {
                string name = FindHTML(keys[i], "Name=", "|||");
                print("BOOKMARK:" + name);
                string posterUrl = FindHTML(keys[i], "PosterUrl=", "|||");
                string id = FindHTML(keys[i], "Id=", "|||");
                if (name != "" && posterUrl != "" && id != "") {
                    if (CheckIfURLIsValid(posterUrl)) {
                        Grid stackLayout = new Grid();
                        Button imageButton = new Button() { HeightRequest = height, WidthRequest = width, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Start };
                        var ff = new FFImageLoading.Forms.CachedImage {
                            Source = posterUrl,
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

                        stackLayout.Children.Add(ff);
                        stackLayout.Children.Add(imageButton);
                        bookmarkPosters.Add(new BookmarkPoster() { button = imageButton, id = id, name = name, posterUrl = posterUrl });
                        Grid.SetColumn(stackLayout, Bookmarks.Children.Count);
                        Bookmarks.Children.Add(stackLayout);

                        // --- RECOMMENDATIONS CLICKED -----
                        imageButton.Clicked += (o, _e) => {
                            for (int z = 0; z < bookmarkPosters.Count; z++) {
                                if (((Button)o).Id == bookmarkPosters[z].button.Id) {
                                    PushPageFromUrlAndName(bookmarkPosters[z].id, bookmarkPosters[z].name);
                                }
                            }
                        };
                    }
                }
                // data.Add(App.GetKey("BookmarkData"))
            }

            MScroll.HeightRequest = keys.Count > 0 ? 130 : 0;

        }
        public double currentWidth { get { return Application.Current.MainPage.Width; } }

    }
    public class MainEpisode100View
    {
        private ObservableCollection<EpisodeResult> _MyEpisodeResultCollection;
        public ObservableCollection<EpisodeResult> MyEpisodeResultCollection { set { Added?.Invoke(null, null); _MyEpisodeResultCollection = value; } get { return _MyEpisodeResultCollection; } }

        public event EventHandler Added;

        public MainEpisode100View()
        {
            MyEpisodeResultCollection = new ObservableCollection<EpisodeResult>();
        }
    }

}