using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CloudStreamForms
{


    public partial class App : Application
    {
        public const string baseM3u8Name = @"sample.m3u8";
        public const string baseSubtitleName = @"sample.srt";

        public interface IPlatformDep
        {
            void PlayVlc(string url, string name, string subtitleLoc);
            void PlayVlc(List<string> url, List<string> name, string subtitleLoc);

            void ShowToast(string message, double duration);
           // void OBrowser(string url);
        }
        public static IPlatformDep platformDep;

       // public static event EventHandler<string> PlayVlc;
        public static event EventHandler<string> OBrowser;
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
        public static void PlayVLCWithSingleUrl(string url, string name = "", string subtitleLoc = "")
        {
            //PlayVlc?.Invoke(null, url);
            platformDep.PlayVlc(url, name, subtitleLoc);
        }

        public static void ShowToast(string message, double duration = 2.5)
        {
            platformDep.ShowToast(message, duration);
        }

        public static void PlayVLCWithSingleUrl(List<string> url, List<string> name, string subtitleLoc = "")
        {
            //PlayVlc?.Invoke(null, url);
            platformDep.PlayVlc(url, name, subtitleLoc);
        }


        public static string ConvertPathAndNameToM3U8(List<string> path, List<string> name, bool isSubtitleEnabled = false, string beforePath = "")
        {
            string _s = "#EXTM3U";
            if(isSubtitleEnabled) {
                _s += "\n#EXTVLCOPT:sub-file=" + beforePath+ baseSubtitleName;
            }
            for (int i = 0; i < path.Count; i++) {
                _s += "\n#EXTINF:" + ", " + name[i].Replace("-","").Replace("  "," ") + "\n" + path[i];
            }
            return _s;
        }

        public static byte[] ConvertPathAndNameToM3U8Bytes(List<string> path, List<string> name, bool isSubtitleEnabled = false, string beforePath = "")
        {
            return Encoding.ASCII.GetBytes(ConvertPathAndNameToM3U8(path,name,isSubtitleEnabled,beforePath));
        }

        public static void OpenBrowser(string url)
        {
            //platformDep.OBrowser(url);

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
