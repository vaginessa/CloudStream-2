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

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public MainEpisodeView epView;
        public List<IMDbTopList> iMDbTopList;
        readonly List<string> genres = new List<string>() { "action", "adventure", "animation", "biography", "comedy", "crime", "drama", "family", "fantasy", "film-noir", "history", "horror", "music", "musical", "mystery", "romance", "sci-fi", "sport", "thriller", "war", "western" };
        readonly List<string> genresNames = new List<string>() { "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Drama", "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western" };

        public async void GetFetch()
        {
            ClearEpisodes();
            print(genres[MovieTypePicker.SelectedIndex] + "<<>>>");
            iMDbTopList = (await FetchTop100(new List<string>() { genres[MovieTypePicker.SelectedIndex] }));
            for (int i = 0; i < 50; i++) {

                double mMulti = 4;
                int pwidth = 67;
                int pheight = 98;
                pheight = (int)Math.Round(pheight * mMulti * posterRezMulti);
                pwidth = (int)Math.Round(pwidth * mMulti * posterRezMulti);
                string x1 = "67"; string y1 = "98";
                string img = iMDbTopList[i].img.Replace(","+ x1 + ","+y1 + "_AL", "," + pwidth + "," + pheight + "_AL").Replace("UY"+y1, "UY" + pheight).Replace("UX"+x1, "UX" + pwidth);//@._V1_UY67_CR0,0,45,67_AL_.jpg
                print("IMG:" + img);
                IMDbTopList x = iMDbTopList[i];

                AddEpisode(new EpisodeResult() { Description = x.descript, Title = x.name + " | ★ " + x.rating.Replace(",", "."), Id = x.place, PosterUrl = img, extraInfo = x.id }, false);
            }
            SetHeight();
        }
        private void episodeView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //EpisodeResult episodeResult = ((EpisodeResult)((ListView)sender).BindingContext);
            //PlayEpisode(episodeResult);

        }
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            //  PlayEpisode(episodeResult);
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = (EpisodeResult)(((ViewCell)sender).BindingContext);
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
            MovieTypePicker.SelectedIndexChanged += (o, e) => {
                GetFetch();
                print(MovieTypePicker.SelectedIndex + "<<Selected");
            };
            MovieTypePicker.SelectedIndex = 0;


            // MovieTypePicker.IsEnabled = false;
            //MovieTypePicker.IsVisible = false;
        }
        void AddEpisode(EpisodeResult episodeResult, bool setHeight = true)
        {
            epView.MyEpisodeResultCollection.Add(episodeResult);
            if (setHeight) {
                SetHeight();
            }
        }

        void ClearEpisodes()
        {
            epView.MyEpisodeResultCollection.Clear();
            SetHeight();
        }

        void SetHeight()
        {
            Device.BeginInvokeOnMainThread(() => episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight + 20);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateBookmarks();
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
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