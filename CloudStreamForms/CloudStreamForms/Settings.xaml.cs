using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using System.Net;
using static CloudStreamForms.CloudStreamCore;
using static CloudStreamForms.MainPage;

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
        public static int LoadingChromeSec
        {
            set {
                App.SetKey("Settings", nameof(LoadingChromeSec), value);
            }
            get {
                return App.GetKey("Settings", nameof(LoadingChromeSec), 30);
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

        public static bool CacheData
        {
            set {
                App.SetKey("Settings", nameof(CacheData), value);
            }
            get {
                return App.GetKey("Settings", nameof(CacheData), true);
            }
        }

        public static bool Top100Enabled
        {
            set {
                App.SetKey("Settings", nameof(Top100Enabled), value);
            }
            get {
                return App.GetKey("Settings", nameof(Top100Enabled), true);
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

        public static bool HasScrollBar
        {
            get {
                return Device.RuntimePlatform == Device.UWP;
            }
        }

        public static ScrollBarVisibility ScrollBarVisibility
        {
            get { return HasScrollBar ? ScrollBarVisibility.Default : ScrollBarVisibility.Never; }
        }

        public static bool CacheImdb { get { return CacheData; } }
        public static bool CacheMAL { get { return CacheData; } }

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
            CacheToggle.OnChanged += (o, e) => {
                CacheData = e.Value;
            };
            TopToggle.OnChanged += (o, e) => {
                Top100Enabled = e.Value;
            };

            UpdateBtt.Clicked += (o, e) => {
                if (NewGithubUpdate) {
                    App.DownloadNewGithubUpdate(githubUpdateTag);
                }
            };
            if (Device.RuntimePlatform != Device.Android) {
                for (int i = 0; i < SettingsTable.Count; i++) {
                    if (i >= SettingsTable.Count) break;
                    if (SettingsTable[i][0] is TextCell) {
                        if (((TextCell)(SettingsTable[i][0])).DetailColor == Color.Transparent) {
                            SettingsTable.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }
        const double MAX_LOADING_TIME = 30000;
        const double MIN_LOADING_TIME = 1000;
        const double MIN_LOADING_CHROME = 5;
        const double MAX_LOADING_CHROME = 60;

        const int ROUND_LOADING_DECIMALES = 2;
        const int ROUND_LOADING_CHROME_DECIMALES = 0;
        void Apper()
        {
            Device.BeginInvokeOnMainThread(() => {
                SetSliderTime();
                SetSliderChromeTime();
                LoadingSlider.Value = ((LoadingMiliSec - MIN_LOADING_TIME) / (MAX_LOADING_TIME-MIN_LOADING_TIME));
                CastSlider.Value = ((LoadingChromeSec - MIN_LOADING_CHROME) / (MAX_LOADING_CHROME-MIN_LOADING_CHROME));
                SubtitesToggle.On = SubtitlesEnabled;
                DubToggle.On = DefaultDub;
                ViewHistoryToggle.On = !ViewHistory;
                DecToggle.On = MovieDecEnabled;
                EpsDecToggle.On = EpDecEnabled;
                SearchToggle.On = SearchEveryCharEnabled;
                CacheToggle.On = CacheData;
                TopToggle.On = Top100Enabled;

                if (Device.RuntimePlatform == Device.UWP) {
                    BlackBgToggle.IsEnabled = false;
                    BlackBgToggle.On = true;
                }
                else {
                    BlackBgToggle.On = BlackBg;
                }
                /*
                if (Device.RuntimePlatform == Device.UWP) {
                    DataTxt.Detail = DataTxt.Detail.Replace("|" + FindHTML(DataTxt.Text, "|", "|") + "|", "");
                    DataTxt2.Detail = DataTxt2.Detail.Replace("|" + FindHTML(DataTxt2.Text, "|", "|") + "|", "");
                }
                else {
                    DataTxt2.Detail = DataTxt2.Detail.Replace("|", "");
                    DataTxt.Detail = DataTxt2.Detail.Replace("|", "");
                }*/


            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Apper();


            if (Device.RuntimePlatform == Device.Android) {
                UpdateBtt.IsEnabled = NewGithubUpdate;
                UpdateBtt.IsVisible = NewGithubUpdate;
                UpdateBtt.Text = "Update " + App.GetBuildNumber() + " to " + githubUpdateTag.Replace("v", "") + " · " + githubUpdateText;
                BackgroundColor = Color.FromHex(Settings.MainBackgroundColor);
            }

        }


        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            LoadingMiliSec = (int)(Math.Round((Math.Round(((Slider)sender).Value * (MAX_LOADING_TIME-MIN_LOADING_TIME)) + MIN_LOADING_TIME)/Math.Pow(10, ROUND_LOADING_DECIMALES))* Math.Pow(10, ROUND_LOADING_DECIMALES));
            SetSliderTime();
        }
        private void Slider_DragCompleted2(object sender, EventArgs e)
        {
            LoadingChromeSec = (int)(Math.Round((Math.Round(((Slider)sender).Value * (MAX_LOADING_CHROME - MIN_LOADING_CHROME)) + MIN_LOADING_CHROME) / Math.Pow(10, ROUND_LOADING_CHROME_DECIMALES)) * Math.Pow(10, ROUND_LOADING_CHROME_DECIMALES));
            SetSliderChromeTime();
        }

        void SetSliderTime()
        {
            string s = Math.Round(LoadingMiliSec / 1000.0, 1).ToString();
            if (!s.Contains(".")) {
                s += ".0";
            }
            LoadingTime.Text = "Loading Time: " + s + "s";
        }

        void SetSliderChromeTime()
        {
            CastTime.Text = "Chromecast skip time: " + LoadingChromeSec + "s";
        }

        private void TextCell_Tapped(object sender, EventArgs e)
        {
            ClearHistory();
        }
        private void TextCell_Tapped2(object sender, EventArgs e)
        {
            ClearBookmarks();
        }
        private void TextCell_Tapped3(object sender, EventArgs e)
        {
            ClearCache();
        }
        private void TextCell_Tapped4(object sender, EventArgs e)
        {
            ResetToDef();
        }

        async void ClearBookmarks()
        {
            bool action = await DisplayAlert("Clear bookmarks", "Are you sure that you want to remove all bookmarks" + " (" + App.GetKeyCount("BookmarkData") + " items)", "Yes", "Cancel");
            if (action) {
                App.RemoveFolder("BookmarkData");
            }
        }

        async void ClearHistory()
        {
            bool action = await DisplayAlert("Clear watch history", "Are you sure that you want to clear all watch history" + " (" + App.GetKeyCount("ViewHistory") + " items)", "Yes", "Cancel");
            if (action) {
                App.RemoveFolder("ViewHistory");
            }
        }

        async void ClearCache()
        {
            bool action = await DisplayAlert("Clear cached data", "Are you sure that you want to clear all cached data" + " (" + (App.GetKeyCount("CacheMAL") + App.GetKeyCount("CacheImdb")) + " items)", "Yes", "Cancel");
            if (action) {
                App.RemoveFolder("CacheMAL");
                App.RemoveFolder("CacheImdb");
            }
        }
        async void ResetToDef()
        {
            bool action = await DisplayAlert("Reset settings to default", "Are you sure that you want to reset settings to default", "Yes", "Cancel");
            if (action) {
                App.RemoveFolder("Settings");
                Apper();
            }
        }
    }
}