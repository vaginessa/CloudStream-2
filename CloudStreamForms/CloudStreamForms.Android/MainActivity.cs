using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using LibVLCSharp.Shared;

namespace CloudStreamForms.Droid
{
    [Activity(Label = "CloudStream 2", Icon = "@drawable/bicon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation), IntentFilter(new[] { Intent.ActionView }, DataScheme = "CloudStreamForms", Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        MainDroid mainDroid;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Core.Initialize();

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

            mainDroid = new MainDroid();
            mainDroid.Awake();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

            var activity = (Activity)Application.Context;

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

        public void PlayVlc(string url, string name, string subtitleLoc)
        {
            try {
                MainDroid.OpenPathAsVideo(url, name, subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url);
            }
        }
        public void Awake()
        {
            App.platformDep = this;
        }
    }
}