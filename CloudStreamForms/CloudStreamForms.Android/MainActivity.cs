using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using static CloudStreamForms.Main;

namespace CloudStreamForms.Droid
{
    [Activity(Label = "CloudStream 2", Icon = "@drawable/bicon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation), IntentFilter(new[] { Intent.ActionView }, DataScheme = "cloudstreamforms", Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        MainDroid mainDroid;
        public static Activity activity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Window window = this.Window;
            window.AddFlags(WindowManagerFlags.Fullscreen); // REMOVES STATUS BAR

            base.OnCreate(savedInstanceState);
            string data = Intent?.Data?.EncodedAuthority;

            try {
                MainPage.intentData = data;
            }
            catch (Exception) { }

            // int intHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            //int intWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            LoadApplication(new App());
            activity = this;

            mainDroid = new MainDroid();
            mainDroid.Awake();

            if (Intent.DataString != null) {
                if (Intent.DataString != "") {
                    Main.PushPageFromUrlAndName(Intent.DataString);
                }
            }
            RequestPermission(this);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static int REQUEST_WRITE_STORAGE = 112;
        private static void RequestPermission(Activity context)
        {
            bool hasPermission = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == Permission.Granted);
            if (!hasPermission) {
                ActivityCompat.RequestPermissions(context,
                   new string[] { Manifest.Permission.WriteExternalStorage },
                 REQUEST_WRITE_STORAGE);
            }

        }


    }
    public class MainDroid : App.IPlatformDep
    {
        public static void OpenPathAsVideo(string path, string name, string subtitleLoc)
        {
            OpenPathsAsVideo(new List<string>() { path }, new List<string>() { name }, subtitleLoc);
        }


        public static Java.IO.File WriteFile(string name, string basePath, string write)
        {
            try {
                File.Delete(basePath + "/" + name);
            }
            catch (System.Exception) {

            }
            //name = Regex.Replace(name, @"[^A-Za-z0-9\.]+", String.Empty);
            //name.Replace(" ", "");
          //  name = name.ToLower();

            Java.IO.File file = new Java.IO.File(basePath, name);
            Java.IO.File _file = new Java.IO.File(basePath);
            Main.print("PATH: " + basePath + "/" + name);
            _file.Mkdirs();
            file.CreateNewFile();
            Java.IO.FileWriter writer = new Java.IO.FileWriter(file);
            // Writes the content to the file
            writer.Write(write);
            writer.Flush();
            writer.Close();
            return file;
        }
   

        public static async Task OpenPathsAsVideo(List<string> path, List<string> name, string subtitleLoc)
        {
            string absolutePath = Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads;
            Main.print("AVS: " + absolutePath);

            bool subtitlesEnabled = subtitleLoc != "";
            string writeData = CloudStreamForms.App.ConvertPathAndNameToM3U8(path, name);//,subtitlesEnabled, "file://" + absolutePath + "/");
            Java.IO.File subFile = null;
            WriteFile(CloudStreamForms.App.baseM3u8Name, absolutePath, writeData);
            if (subtitlesEnabled) {
                subFile = WriteFile(CloudStreamForms.App.baseSubtitleName, absolutePath, subtitleLoc);
            }

            // await Task.Delay(5000);

            Device.BeginInvokeOnMainThread(() => {
                // OpenPathAsVideo(path.First(), name.First(), "");
                OpenVlcIntent(absolutePath + "/" + CloudStreamForms.App.baseM3u8Name, absolutePath + "/" + App.baseSubtitleName);
            });
        }




        public static void OpenVlcIntent(string path, string subfile = "") //Java.IO.File subFile)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(path);

            Intent intent = new Intent(Intent.ActionView).SetDataAndType(uri, "video/*");
            intent.SetPackage("org.videolan.vlc");
            Main.print("Da_ " + Android.Net.Uri.Parse(subfile));

            if (subfile != "") {
                intent.PutExtra("subtitles_location", Android.Net.Uri.Parse(subfile));//Android.Net.Uri.FromFile(subFile));
                                                                                      // intent.PutExtra("subtitles_location", );//Android.Net.Uri.FromFile(subFile));
            }

            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
            intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);
            intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);

            intent.AddFlags(ActivityFlags.NewTask);


            // Android.App.Application.Context.ApplicationContext.start
            //Android.App.Application.Context.StartService(intent);
            Android.App.Application.Context.StartActivity(intent);
        }

        public void PlayVlc(string url, string name, string subtitleLoc)
        {
            try {
                MainDroid.OpenPathAsVideo(url, name, subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url);
            }
        }
        public void PlayVlc(List<string> url, List<string> name, string subtitleLoc)
        {
            try {
                MainDroid.OpenPathsAsVideo(url, name, subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url.First());
            }

        }
        public void Awake()
        {
            App.platformDep = this;
        }

        public void ShowToast(string message, double duration)
        {
            Device.BeginInvokeOnMainThread(() => {
                ToastLength toastLength = ToastLength.Short;
                if (duration >= 3) {
                    toastLength = ToastLength.Long;
                }
                Toast.MakeText(Android.App.Application.Context, message, toastLength).Show();
            });
     
        }

        public string GetPath(bool mainPath, string extraPath)
        {
            return (mainPath ? (Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads) : AppDomain.CurrentDomain.BaseDirectory) + extraPath;
        }
        public void DownloadFile(string file, string fileName, bool mainPath, string extraPath)
        {
            WriteFile(fileName, GetPath(mainPath,extraPath), file );
        }

        public void DownloadUrl(string url, string fileName, bool mainPath, string extraPath)
        {
            try {

                TempThred tempThred = new TempThred();
                tempThred.typeId = 4; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() =>
                {
                    try {
                        WebClient webClient = new WebClient();
                        string basePath = GetPath(mainPath, extraPath);
                        Main.print(basePath);
                        Java.IO.File _file = new Java.IO.File(basePath);

                        _file.Mkdirs();
                        basePath += "/" + fileName;
                        Main.print(basePath);
                        webClient.DownloadFile(url, basePath);

                        //if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Download Thread";
                tempThred.Thread.Start();

              
            }
            catch (Exception) {
                App.ShowToast("Download Failed");
            }
        }

        public string GetBuildNumber()
        {
            var context = Android.App.Application.Context;
            var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
            return BuildNumber + " " + VersionNumber;
        }
    }
}
