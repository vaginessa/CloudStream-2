using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudStreamForms
{
    public partial class App : Application
    {
        public static event EventHandler<string> PlayVlc;
        public static event EventHandler<string> OBrowser;
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        public static void PlayVLCWithSingleUrl(string url, string name = "")
        {
            PlayVlc?.Invoke(null, url);
        }
        public static void OpenBrowser(string url)
        {
            OBrowser?.Invoke(null, url);
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
