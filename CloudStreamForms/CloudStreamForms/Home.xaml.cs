using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.MainPage;
using static CloudStreamForms.Main;
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
            Device.BeginInvokeOnMainThread(() => {
                int count = 10;//PosterAtScreenHight * PosterAtScreenWith * 3
                for (int i = 0; i < count; i++) {
                    if (currentImageCount >= iMDbTopList.Count) {
                        if (!fething && !IsRecommended) {
                            GetFetch(currentImageCount + 1);
                        }
                        return;
                        //Feth more data
                    }
                    else {
                        IMDbTopList x = iMDbTopList[currentImageCount];

                        int selGen = MovieTypePicker.SelectedIndex - 1;
                        if (selGen != -1 && IsRecommended) {
                            if(!x.contansGenres.Contains(selGen)) {
                                continue;
                            }
                        }

                        string img = ConvertIMDbImagesToHD(iMDbTopList[currentImageCount].img, IsRecommended ? 76 : 67 , IsRecommended ? 113 : 98, 4);

                        AddEpisode(new EpisodeResult() { Description = x.descript, Title = (x.place > 0 ? (x.place + ". ") : "") + x.name + " | ★ " + x.rating.Replace(",", "."), Id = x.place, PosterUrl = img, extraInfo = "Id=" + x.id + "|||Name=" + x.name + "|||" }, false);

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
            fething = true;
            TempThred tempThred = new TempThred();
            tempThred.typeId = 0; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(() => {
                try {

                    iMDbTopList.AddRange(FetchRecomended(bookmarkPosters.Select(t => t.id).ToList()));
                    LoadMoreImages();
                }
                finally {
                    fething = false;
                    JoinThred(tempThred);
                }
            });
            tempThred.Thread.Name = "FethTop100";
            tempThred.Thread.Start();

        }

        bool fething = false;
        public async void GetFetch(int start = 1)
        {
            fething = true;
            TempThred tempThred = new TempThred();
            tempThred.typeId = 0; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
            tempThred.Thread = new System.Threading.Thread(async () => {
                try {

                    iMDbTopList.AddRange(await FetchTop100(new List<string>() { genres[MovieTypePicker.SelectedIndex] }, start));
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
                    fething = false;
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
            ImdbTypePicker.SelectedIndex = 0;
            ImdbTypePicker.SelectedIndexChanged += (o, e) => {
                ClearEpisodes();
                if (IsRecommended) {
                    GetFetchRecomended();
                }
                else {
                    GetFetch();
                }
            };
            MovieTypePicker.SelectedIndexChanged += (o, e) => {
                ClearEpisodes();
                if(IsRecommended) {
                    LoadMoreImages();
                }
                else {
                    GetFetch();
                }
                //GetFetchRecomended
                //  print(MovieTypePicker.SelectedIndex + "<<Selected");
            };

            MovieTypePicker.SelectedIndex = 0;
            episodeView.Scrolled += (o, e) => {
                double maxY = episodeView.HeightRequest - episodeView.Height;
                //print(maxY);
                if (e.ScrollY >= maxY - 200) {
                    LoadMoreImages();
                }
            };
      

            OffBar.Source = App.GetImageSource("gradient.png"); OffBar.HeightRequest = 3; OffBar.HorizontalOptions = LayoutOptions.Fill; OffBar.ScaleX = 100; OffBar.Opacity = 0.3; OffBar.TranslationY = 9;
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

        void ClearEpisodes()
        {
            //ItemGrid.Children.Clear();
            epView.MyEpisodeResultCollection.Clear();
            cachedImages.Clear();
            currentImageCount = 0;
            SetHeight(); iMDbTopList.Clear();
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

            Device.BeginInvokeOnMainThread(() => { episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight + 20; }
);
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
                string posterUrl = FindHTML(keys[i], "PosterUrl=", "|||");
                string id = FindHTML(keys[i], "Id=", "|||");
                if (name != "" && posterUrl != "" && id != "") {
                    if (CheckIfURLIsValid(posterUrl)) {
                        print(">>>>" + posterUrl);
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
                print(keys[i] + "<<KEy");
                // data.Add(App.GetKey("BookmarkData"))
            }
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