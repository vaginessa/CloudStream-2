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

namespace CloudStreamForms.Droid
{
    [Activity(Label = "CloudStream 2", Icon = "@drawable/bicon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation), IntentFilter(new[] { Intent.ActionView }, DataScheme = "CloudStreamForms", Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]

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
            LoadApplication(new App());
            activity = this;

            mainDroid = new MainDroid();
            mainDroid.Awake();

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
            //   Main.print(">>>>>>>>>>>>>>> NAME : " + name);


            Android.Net.Uri uri = Android.Net.Uri.Parse(path);

            Intent intent = new Intent(Intent.ActionView);
            // intent.SetPackage("org.videolan.vlc");

            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
            intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);
            intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);

            if (subtitleLoc != "") {
                intent.PutExtra("subtitles_location", subtitleLoc);
            }
            intent.SetDataAndTypeAndNormalize(uri, "video/*");
            intent.PutExtra("title", name);
            //intent = intent.PutExtra("title", "title"); intent.PutExtra("title", "Kung Fury");

           // var activity = (Activity)MainActivity.activity;

            // Android.App.Application.Context.ApplicationContext.start
            Android.App.Application.Context.StartService(intent);
            /*
            int vlcRequestCode = 42;

            Intent vlcIntent = new Intent(Intent.ActionView);
            vlcIntent.SetPackage("org.videolan.vlc");
            vlcIntent.SetDataAndTypeAndNormalize(uri, "video/*");
            vlcIntent.PutExtra("title", "Kung Fury");
            vlcIntent.PutExtra("from_start", false);
            vlcIntent.PutExtra("position", 5);
            vlcIntent.SetComponent(new ComponentName("org.videolan.vlc", "org.videolan.vlc.gui.video.VideoPlayerActivity"));

            // vlcIntent.PutExtra("subtitles_location", "/sdcard/Movies/Fifty-Fifty.srt");
            var activity = (Activity)Application.Context;
            activity.StartActivityForResult(vlcIntent, vlcRequestCode);
            */
            //   activity.StartActivityForResult(intent,42);
        }



        public static void OpenPathsAsVideo(List<string> path, List<string> name, string subtitleLoc)
        {


            string absolutePath = Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads;
            Main.print("AVS: " + absolutePath);

            string basePath = absolutePath;
            string subPath = CloudStreamForms.App.baseM3u8Name;
            string rootPath = basePath + "/" + subPath;
            Main.print("ROOT: " + rootPath);

            Console.WriteLine("_RROT" + rootPath);

            string writeData = CloudStreamForms.App.ConvertPathAndNameToM3U8(path, name);

            try {
                File.Delete(rootPath);
            }
            catch (System.Exception) {

            }
            Java.IO.File file = new Java.IO.File(basePath, subPath);
            file.CreateNewFile();
            file.Mkdir();
            Java.IO.FileWriter writer = new Java.IO.FileWriter(file);
            // Writes the content to the file
            writer.Write(writeData);
            writer.Flush();
            writer.Close();

            Device.BeginInvokeOnMainThread(() => {
               // OpenPathAsVideo(path.First(), name.First(), "");
                _OpenPathAsVieo(rootPath);

            });

            //OpenPathAsVideo(path.First(), name.First(), subtitleLoc);
            //  OpenPathAsVieo(rootPath);


            /*
            Console.WriteLine("ROOT: " + rootPath);
            Main.print("ROOT: " + rootPath);


            try {
                File.Delete(rootPath);
            }
            catch (System.Exception) {

            }
            Java.IO.File file = new Java.IO.File(basePath, subPath);
            file.CreateNewFile();
            file.Mkdir();
            Java.IO.FileWriter writer = new Java.IO.FileWriter(file);
            // Writes the content to the file
            writer.Write(CloudStreamForms.App.ConvertPathAndNameToM3U8(path, name));
            writer.Flush();
            writer.Close();*/





        }

        public static void _OpenPathAsVieo(string path)
        {
            Main.print("PATH:: " + path);
            Console.WriteLine("PATH2: " + path);


            Android.Net.Uri uri = Android.Net.Uri.Parse(path);

            Intent intent = new Intent(Intent.ActionView).SetDataAndType(uri, "video/*");

            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
            intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);
            intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
            intent.AddFlags(ActivityFlags.NewTask);
            

            // Android.App.Application.Context.ApplicationContext.start
            //Android.App.Application.Context.StartService(intent);
            Android.App.Application.Context.StartActivity(intent);
        }

        public static void OpenPathAsVieo(string path)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(path);

            Intent intent = new Intent(Intent.ActionView);
            // intent.SetPackage("org.videolan.vlc");

            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
            intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);
            intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);

        
            intent.SetDataAndTypeAndNormalize(uri, "video/*");
          //  intent.PutExtra("title", name);
            //intent = intent.PutExtra("title", "title"); intent.PutExtra("title", "Kung Fury");

            var activity = (Activity)Android.App.Application.Context;

            // Android.App.Application.Context.ApplicationContext.start
            Android.App.Application.Context.StartService(intent);
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
            //try {
            MainDroid.OpenPathsAsVideo(url, name, subtitleLoc);
            /*
        }
        catch (Exception) {
            try {
                MainDroid.OpenPathAsVideo(url.First(), name.First(), subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url.First());
            }
        }*/
        }
        public void Awake()
        {
            App.platformDep = this;
        }
    }
}