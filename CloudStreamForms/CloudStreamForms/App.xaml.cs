using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using System.Net;
using System.IO;

namespace CloudStreamForms
{


    public partial class App : Application
    {
        public const string baseM3u8Name = @"mirrorlist.m3u8";
        public const string baseSubtitleName = @"subtitles.srt";

        public interface IPlatformDep
        {
            void PlayVlc(string url, string name, string subtitleLoc);
            void PlayVlc(List<string> url, List<string> name, string subtitleLoc);
            void ShowToast(string message, double duration);
            string DownloadFile(string file, string fileName, bool mainPath, string extraPath);
            string DownloadUrl(string url, string fileName, bool mainPath, string extraPath, string toast = "");
            bool DeleteFile(string path);
            void DownloadUpdate(string update);
            string GetDownloadPath(string path, string extraFolder);
            StorageInfo GetStorageInformation(string path = "");

            string GetExternalStoragePath();
        }
        public class StorageInfo
        {
            public long TotalSpace = 0;
            public long AvailableSpace = 0;
            public long FreeSpace = 0;
            public long UsedSpace { get { return TotalSpace - AvailableSpace; } }
            /// <summary>
            /// From 0-1
            /// </summary>
            public double UsedProcentage { get { return ConvertBytesToGB(UsedSpace,4) / ConvertBytesToGB(TotalSpace,4); } } 
        }

        public static void OnDownloadProgressChanged(string path, DownloadProgressChangedEventArgs progress)
        {
           // Main.print("PATH: " + path + " | Progress:" + progress.ProgressPercentage);
        }


        public static IPlatformDep platformDep;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

   
        public static StorageInfo GetStorage()
        {
            return platformDep.GetStorageInformation();
        }

        public static double ConvertBytesToGB(long bytes, int digits = 2)
        {
            return ConvertBytesToAny(bytes, digits, 3);
        }

        public static double ConvertBytesToAny(long bytes, int digits = 2,int steps = 3)
        {
            int div = GetSizeOfJumpOnSystem();
            return Math.Round((bytes / Math.Pow(div, steps)), digits);
        }

        public static int GetSizeOfJumpOnSystem()
        {
            return Device.RuntimePlatform == Device.UWP ? 1024 : 1000; 
        }


        public static bool DeleteFile(string path)
        {
            return platformDep.DeleteFile(path);
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

        public static string GetBuildNumber()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return v.Major + "." + v.Minor + "." + v.Build;
        }

        public static void DownloadNewGithubUpdate(string update)
        {
            platformDep.DownloadUpdate(update);
        }

        public static string GetDownloadPath(string path, string extraFolder)
        {
            return platformDep.GetDownloadPath(path, extraFolder);
        }

        public static void PlayVLCWithSingleUrl(List<string> url, List<string> name, string subtitleLoc = "")
        {
            //PlayVlc?.Invoke(null, url);
            platformDep.PlayVlc(url, name, subtitleLoc);
        }

        static string GetKeyPath(string folder, string name = "")
        {
            string _s = ":" + folder + "-";
            if (name != "") {
                _s += name + ":";
            }
            return _s;
        }

        public static void SetKey(string folder, string name, object value)
        {
            string path = GetKeyPath(folder, name);
            if (Current.Properties.ContainsKey(path)) {
                Main.print("CONTAINS KEY");
                Current.Properties[path] = value;
            }
            else {
                Main.print("ADD KEY");

                Current.Properties.Add(path, value);
            }
        }

        public static T GetKey<T>(string folder, string name, T defVal)
        {
            string path = GetKeyPath(folder, name);
            return GetKey<T>(path, defVal);
        }

        public static void RemoveFolder(string folder)
        {
            List<string> keys = App.GetKeysPath(folder);
            for (int i = 0; i < keys.Count; i++) {
                RemoveKey(keys[i]);
            }
        }

        public static T GetKey<T>(string path, T defVal)
        {
            if (Current.Properties.ContainsKey(path)) {
                return (T)Current.Properties[path];
            }
            else {
                return defVal;
            }
        }

        public static List<T> GetKeys<T>(string folder)
        {
            List<string> keyNames = GetKeysPath(folder);

            List<T> allKeys = new List<T>();
            foreach (var key in keyNames) {
                allKeys.Add((T)Current.Properties[key]);
            }

            return allKeys;
        }

        public static int GetKeyCount(string folder)
        {
            return GetKeysPath(folder).Count;
        }

        public static List<string> GetKeysPath(string folder)
        {
            List<string> keyNames = Current.Properties.Keys.Where(t => t.StartsWith(GetKeyPath(folder))).ToList();
            return keyNames;
        }

        public static bool KeyExists(string folder, string name)
        {
            string path = GetKeyPath(folder, name);
            return KeyExists(path);
        }
        public static bool KeyExists(string path)
        {
            return (Current.Properties.ContainsKey(path));
        }
        public static void RemoveKey(string folder, string name)
        {
            string path = GetKeyPath(folder, name);
            RemoveKey(path);
        }
        public static void RemoveKey(string path)
        {
            if (Current.Properties.ContainsKey(path)) {
                Current.Properties.Remove(path);
            }
        }
        public static ImageSource GetImageSource(string inp)
        {
            return ImageSource.FromResource("CloudStreamForms.Resource." + inp, Assembly.GetExecutingAssembly());
        }

        public static string DownloadUrl(string url, string fileName, bool mainPath = true, string extraPath = "")
        {
            return platformDep.DownloadUrl(url, fileName, mainPath, extraPath);

        }
        public static string DownloadFile(string file, string fileName, bool mainPath = true, string extraPath = "")
        {
            return platformDep.DownloadFile(file, fileName, mainPath, extraPath);
        }

        public static string ConvertPathAndNameToM3U8(List<string> path, List<string> name, bool isSubtitleEnabled = false, string beforePath = "")
        {
            string _s = "#EXTM3U";
            if (isSubtitleEnabled) {
                _s += "\n#EXTVLCOPT:sub-file=" + beforePath + baseSubtitleName;
            }
            for (int i = 0; i < path.Count; i++) {
                _s += "\n#EXTINF:" + ", " + name[i].Replace("-", "").Replace("  ", " ") + "\n" + path[i];
            }
            return _s;
        }

        public static byte[] ConvertPathAndNameToM3U8Bytes(List<string> path, List<string> name, bool isSubtitleEnabled = false, string beforePath = "")
        {
            return Encoding.ASCII.GetBytes(ConvertPathAndNameToM3U8(path, name, isSubtitleEnabled, beforePath));
        }

        public static void OpenBrowser(string url)
        {
            Main.print("Trying to open: " + url);
            if (Main.CheckIfURLIsValid(url)) {
                try {
                    Launcher.OpenAsync(new Uri(url));
                }
                catch (Exception) {
                    Main.print("BROWSER LOADED ERROR, SHOULD NEVER HAPPEND!!");
                }
            }
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
