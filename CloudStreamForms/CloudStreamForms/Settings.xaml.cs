using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;



namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public const string errorEpisodeToast = "No Links Found";
        public static int loadingMiliSec = 5000;

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


        public string MainColor { get { return "#303F9F"; } }

        public Settings()
        {
            InitializeComponent();
            BindingContext = this;
            StarMe.Clicked += (o, e) => {
                App.OpenBrowser("https://github.com/LagradOst/CloudStream-2");
            };
            BuildNumber.Text = "Build Version: " + App.GetBuildNumber();

            ViewHistoryToggle.OnChanged += (o, e) => {
                ViewHistory = e.Value;
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
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            SubtitesToggle.On = SubtitlesEnabled;
            BlackBgToggle.On = BlackBg;
            DubToggle.On = DefaultDub;
            ViewHistoryToggle.On = ViewHistory;

        }

    }
}