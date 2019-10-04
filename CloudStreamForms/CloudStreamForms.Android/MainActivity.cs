using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace CloudStreamForms.Droid
{
    [Activity(Label = "CloudStream 2", Icon = "@drawable/bicon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation), IntentFilter(new[] { Intent.ActionView }, DataScheme = "CloudStreamForms", Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;

            ToolbarResource = Resource.Layout.Toolbar;

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

            CloudStreamForms.App.PlayVlc += App_PlayVlc;
        }

        private void App_PlayVlc(object sender, string e)
        {
            try {
                MainDroid.OpenPathAsVideo(e);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(e);
            }
        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    public static class MainDroid
    {
        public static void OpenPathAsVideo(string path)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(path);

            Intent intent = new Intent(Intent.ActionView).SetDataAndType(uri, "video/mp4");
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}