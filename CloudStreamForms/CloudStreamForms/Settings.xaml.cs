using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using System.Net;
using static CloudStreamForms.Main;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public const string errorEpisodeToast = "No Links Found";
        public static int LoadingMiliSec
        {
            set {
                App.SetKey("Settings", nameof(LoadingMiliSec), value);
            }
            get {
                return App.GetKey("Settings", nameof(LoadingMiliSec), 5000);
            }
        }

        public static bool DefaultDub
        {
            set {
                App.SetKey("Settings", nameof(DefaultDub), value);
            }
            get {
                return App.GetKey("Settings", nameof(DefaultDub), true);
            }
        }

        public static bool SubtitlesEnabled
        {
            set {
                App.SetKey("Settings", nameof(SubtitlesEnabled), value);
            }
            get {
                return App.GetKey("Settings", nameof(SubtitlesEnabled), true);
            }
        }


        public static bool BlackBg
        {
            set {
                App.SetKey("Settings", nameof(BlackBg), value);
            }
            get {
                return App.GetKey("Settings", nameof(BlackBg), false);
            }
        }

        public static bool ViewHistory
        {
            set {
                App.SetKey("Settings", nameof(ViewHistory), value);
            }
            get {
                return App.GetKey("Settings", nameof(ViewHistory), true);
            }
        }

        public static bool EpDecEnabled
        {
            set {
                App.SetKey("Settings", nameof(EpDecEnabled), value);
            }
            get {
                return App.GetKey("Settings", nameof(EpDecEnabled), true);
            }
        }
        public static bool MovieDecEnabled
        {
            set {
                App.SetKey("Settings", nameof(MovieDecEnabled), value);
            }
            get {
                return App.GetKey("Settings", nameof(MovieDecEnabled), true);
            }
        }
        public static bool SearchEveryCharEnabled
        {
            set {
                App.SetKey("Settings", nameof(SearchEveryCharEnabled), value);
            }
            get {
                return App.GetKey("Settings", nameof(SearchEveryCharEnabled), true);
            }
        }


        public string MainColor { get { return Device.RuntimePlatform == Device.UWP ? "#303F9F" : "#515467"; } }

        public static string MainBackgroundColor
        {
            get {
                if (BlackBg) {
                    return "#000000";
                }
                string color = "#111111";
                if (Device.RuntimePlatform == Device.UWP) {
                    return "#000000";
                    color = "#000811";
                }

                return color;
            }
        }


        public Settings()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            if (Device.RuntimePlatform == Device.UWP) {
                OffBar.IsVisible = false;
                OffBar.IsEnabled = false;
            }
            else {
                OffBar.Source = App.GetImageSource("gradient.png"); OffBar.HeightRequest = 3; OffBar.HorizontalOptions = LayoutOptions.Fill; OffBar.ScaleX = 100; OffBar.Opacity = 0.3; OffBar.TranslationY = 9;
            }

            //Main.print("COLOR: "+ BlackBgToggle.OnColor);
            //  if (Device.RuntimePlatform == Device.UWP) {
            BindingContext = this;
            // }
            StarMe.Clicked += (o, e) => {
                App.OpenBrowser("https://github.com/LagradOst/CloudStream-2");
            };
            BuildNumber.Text = "Build Version: " + App.GetBuildNumber();
            Apper();

            ViewHistoryToggle.OnChanged += (o, e) => {
                ViewHistory = !e.Value;
            };

            DubToggle.OnChanged += (o, e) => {
                DefaultDub = e.Value;
            };

            BlackBgToggle.OnChanged += (o, e) => {
                BlackBg = e.Value;
            };

            SubtitesToggle.OnChanged += (o, e) => {
                SubtitlesEnabled = e.Value;
            };
            EpsDecToggle.OnChanged += (o, e) => {
                EpDecEnabled = e.Value;
            };
            DecToggle.OnChanged += (o, e) => {
                MovieDecEnabled = e.Value;
            };
            SearchToggle.OnChanged += (o, e) => {
                SearchEveryCharEnabled = e.Value;
            };
            UpdateBtt.Clicked += (o, e) => {
                if (NewGithubUpdate) {
                    App.DownloadNewGithubUpdate(Main.githubUpdateTag);
                }
            };

        }

        void Apper()
        {
            Device.BeginInvokeOnMainThread(() => {
                LoadingTime.Text = "Loading Time: " + LoadingMiliSec + "ms";
                LoadingSlider.Value = ((LoadingMiliSec - 1000.0) / 9000.0);
                SubtitesToggle.On = SubtitlesEnabled;
                DubToggle.On = DefaultDub;
                ViewHistoryToggle.On = !ViewHistory;
                DecToggle.On = MovieDecEnabled;
                EpsDecToggle.On = EpDecEnabled;
                SearchToggle.On = SearchEveryCharEnabled;
                if (Device.RuntimePlatform == Device.UWP) {
                    BlackBgToggle.IsEnabled = false;
                    BlackBgToggle.On = true;
                }
                else {
                    BlackBgToggle.On = BlackBg;
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Apper();

   
            if (Device.RuntimePlatform == Device.Android) {
                UpdateBtt.IsEnabled = Main.NewGithubUpdate;
                UpdateBtt.IsVisible = Main.NewGithubUpdate;
                UpdateBtt.Text = "Update " + App.GetBuildNumber() + " to " + githubUpdateTag + " · " + githubUpdateText;
                BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            }

        }


        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            LoadingMiliSec = (int)Math.Round(((Slider)sender).Value * 9000) + 1000;
            LoadingTime.Text = "Loading Time: " + LoadingMiliSec + "ms";
        }

        private void TextCell_Tapped(object sender, EventArgs e)
        {
            ClearHistory();
        }
        private void TextCell_Tapped2(object sender, EventArgs e)
        {
            ClearBookmarks();
        }
        async void ClearBookmarks()
        {
            bool action = await DisplayAlert("Clear bookmarks", "Are you sure that you want to remove all bookmarks","Yes", "Cancel");
            if (action) {
                App.RemoveFolder("BookmarkData");
            }
        }
        async void ClearHistory()
        {
            
            bool action = await DisplayAlert("Clear watch history", "Are you sure that you want to clear all watch history", "Yes", "Cancel");
            if (action) {
                App.RemoveFolder("ViewHistory");
            }
        }
    }
}