using CloudStreamForms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.Main;
using static CloudStreamForms.MainPage;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Download : ContentPage
    {
        public MainEpisodeView epView;

        public Download()
        {
            InitializeComponent();
            epView = new MainEpisodeView();
            BindingContext = epView;

            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            UpdateDownloads();
        }
        List<DownloadPoster> downloadposter = new List<DownloadPoster>();
        struct DownloadPoster
        {
            public Button button;
            public int id;
            public string moviePath;
            public string name;
        }

        void AddEpisode(EpisodeResult episodeResult)
        {
            epView.MyEpisodeResultCollection.Add(episodeResult);
            SetHeight();
        }
        void SetHeight()
        {
            Device.BeginInvokeOnMainThread(() => episodeView.HeightRequest = epView.MyEpisodeResultCollection.Count * episodeView.RowHeight + 20);
        }


        void UpdateDownloads()
        {
            List<string> keys = App.GetKeys<string>("Download");
            List<string> keysPaths = App.GetKeysPath("Download");
            foreach (var item in keysPaths) {
                print("KEYPATH:" + item);
            }
            List<string> data = new List<string>();
            downloadposter = new List<DownloadPoster>();
            // Downloads.Children.Clear();
            epView.MyEpisodeResultCollection.Clear();

            for (int i = 0; i < keys.Count; i++) {
                string moviePath = FindHTML(keys[i], "_dpath=", "|||");
                string posterUrl = FindHTML(keys[i], "_ppath=", "|||");
                string movieUrl = FindHTML(keys[i], "_mppath=", "|||");
                string episodeDescript = FindHTML(keys[i], "_descript=", "|||");
                string movieDescript = FindHTML(keys[i], "_maindescript=", "|||");
                string id = FindHTML(keys[i], "_epId=", "|||");
                string movieId = FindHTML(keys[i], "_movieId=", "|||");
                string episodeTitle = FindHTML(keys[i], "_title=", "|||");
                string movieTitle = FindHTML(keys[i], "_movieTitle=", "|||");
                string epCounter = FindHTML(keys[i], "_epCounter=", "|||");
                print("KEY:" + keys[i]);
                const double height = 80;
                const double width = 126;
                if (moviePath != "") {
                    double currentProgress = GetFileSizeOnSystem(moviePath);
                    double maxProgress = App.GetKey("DownloadSize", id, -1.0);
                    double dprogress = currentProgress / maxProgress;
                    if (currentProgress == -1 || maxProgress == -1) {
                        dprogress = 1;
                    }
                    string extra = "";
                    bool downloadDone = false;
                    if(dprogress != -1) {
                        downloadDone = dprogress > 0.98;
                        if(!downloadDone) {

                        extra = " | " + currentProgress + " Mb - " + maxProgress + " Mb";
                        }
                        else {
                            extra = " | " + maxProgress + " Mb";
                        }
                    }

                    AddEpisode(new EpisodeResult() {
                        Description = episodeDescript,
                        PosterUrl = posterUrl,
                        Id = i,
                        Title = episodeTitle + extra,
                        ExtraProgress = dprogress,
                        DownloadNotDone = !downloadDone,
                        Mirros = new List<string>() { "Download" },
                        mirrosUrls = new List<string>() { moviePath },
                        extraInfo = "KeyPath=" + keysPaths[i] + "|||_mppath=" + movieUrl + "|||_dpath=" + moviePath + "|||_ppath=" + posterUrl + "|||_movieId=" + movieId + "|||_movieTitle=" + movieTitle + "|||=EndAll"
                    });
                    /*
                    Grid stackLayout = new Grid();
                    Button imageButton = new Button() { HeightRequest = height, WidthRequest = width, BackgroundColor = Color.Transparent, VerticalOptions = LayoutOptions.Start };
                    var ff = new FFImageLoading.Forms.CachedImage {
                        Source = posterUrl,
                        HeightRequest = height,
                        WidthRequest = width,
                        BackgroundColor = Color.Transparent,
                        VerticalOptions = LayoutOptions.Start,
                        Transformations = {
                                new FFImageLoading.Transformations.RoundedTransformation(10,2.5,1,10,"#303F9F")
                            },
                        InputTransparent = true,
                    };
                    var epTit = new Label() { Text = episodeTitle };
                    var epDesc = new Label() { Text = episodeDescript };
                    //Source = p.posterUrl

                    stackLayout.Children.Add(ff);
                    stackLayout.Children.Add(imageButton);
                    stackLayout.Children.Add(epTit);
                //    stackLayout.Children.Add(epDesc);
                    //stackLayout.WidthRequest = 0;
                    var c = new ColumnDefinition(); c.Width = new GridLength(1, GridUnitType.Auto);
                    stackLayout.ColumnDefinitions = new ColumnDefinitionCollection() { c, c, c };


                    Grid.SetColumn(epTit, 1);
                   // Grid.SetColumn(epDesc, 1);
                    downloadposter.Add(new DownloadPoster() { button = imageButton, id = i, moviePath = moviePath, name = episodeTitle });
                 //   Grid.SetRow(stackLayout, Downloads.Children.Count);
                   // Downloads.Children.Add(stackLayout);

                    // --- RECOMMENDATIONS CLICKED -----
                    imageButton.Clicked += (o, _e) => {
                        for (int z = 0; z < downloadposter.Count; z++) {
                            if (((Button)o).Id == downloadposter[z].button.Id) {
                                App.PlayVLCWithSingleUrl(downloadposter[z].moviePath, downloadposter[z].name);
                                // PushPageFromUrlAndName(bookmarkPosters[z].id, bookmarkPosters[z].name);
                            }
                        }
                    };
                }*/

                }
            }
        }

        private void episodeView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //EpisodeResult episodeResult = ((EpisodeResult)((ListView)sender).BindingContext);
            //PlayEpisode(episodeResult);

        }

        void PlayEpisode(EpisodeResult episodeResult)
        {
            App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls[0], episodeResult.Title);
            episodeView.SelectedItem = null;
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = (EpisodeResult)(((ViewCell)sender).BindingContext);
            EpsodeShow(episodeResult);
            //EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            //App.PlayVLCWithSingleUrl(episodeResult.mirrosUrls[0], episodeResult.Title);
            //episodeView.SelectedItem = null;
        }

        async void EpsodeShow(EpisodeResult episodeResult)
        {
            string action = await DisplayActionSheet(episodeResult.Title, "Cancel", null, "Play", "Delete File", "Open Source");
            if (action == "Play") {
                PlayEpisode(episodeResult);
            }
            if (action == "Delete File") {
                string moviePath = FindHTML(episodeResult.extraInfo, "_dpath=", "|||");
                string keyPath = FindHTML(episodeResult.extraInfo, "KeyPath=", "|||");
                //string posterUrl = FindHTML(episodeResult.extraInfo, "_ppath=", "|||");
                // string movieUrl = FindHTML(episodeResult.extraInfo, "_mppath=", "|||");
                //App.DeleteFile(movieUrl);
                //App.DeleteFile(posterUrl);
                DeleteFile(moviePath, keyPath);
            }
            if (action == "Open Source") {
                string title = FindHTML(episodeResult.extraInfo, "_movieTitle=", "|||");
                string movieId = FindHTML(episodeResult.extraInfo, "_movieId=", "|||");
                PushPageFromUrlAndName(movieId, title);
            }
            UpdateDownloads();
        }

        public static void DeleteFile(string keyPath)
        {
            string keyData = App.GetKey(keyPath, "");
            string moviePath = FindHTML(keyData, "_dpath=", "|||");
            DeleteFile(moviePath, keyPath);
        }
        public static void DeleteFileFromFolder(string keyData, string keyFolder, string keyId)
        {
            string moviePath = FindHTML(keyData, "_dpath=", "|||");
            DeleteFile(moviePath, keyFolder, keyId);
        }

        public static void DeleteFile(string moviePath, string keyPath)
        {
            if (App.DeleteFile(moviePath)) {
                App.RemoveKey(keyPath);
            }
        }

        public static void DeleteFile(string moviePath, string keyFolder, string keyId)
        {
            if (App.DeleteFile(moviePath)) {
                App.RemoveKey(keyFolder, keyId);
            }
        }


        public static void PlayFile(string keyData, string title = "")
        {
            string moviePath = FindHTML(keyData, "_dpath=", "|||");
            App.PlayVLCWithSingleUrl(moviePath, title);
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

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            EpisodeResult episodeResult = ((EpisodeResult)((ImageButton)sender).BindingContext);
            PlayEpisode(episodeResult);
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

}