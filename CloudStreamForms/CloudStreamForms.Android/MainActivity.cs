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
using Android.Provider;

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
        public static int REQUEST_INSTALL = 113;
        public static int REQUEST_INSTALL2 = 113;
        private static void RequestPermission(Activity context)
        {
            bool hasPermission = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == Permission.Granted);
            if (!hasPermission) {
                ActivityCompat.RequestPermissions(context,
                   new string[] { Manifest.Permission.WriteExternalStorage },
                 REQUEST_WRITE_STORAGE);
            }
            bool hasPermission2 = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.RequestInstallPackages) == Permission.Granted);
            if (!hasPermission2) {
                ActivityCompat.RequestPermissions(context,
                   new string[] { Manifest.Permission.RequestInstallPackages },
                 REQUEST_INSTALL);
            }
            bool hasPermission3 = (ContextCompat.CheckSelfPermission(context, Manifest.Permission.InstallPackages) == Permission.Granted);
            if (!hasPermission3) {
                ActivityCompat.RequestPermissions(context,
                   new string[] { Manifest.Permission.InstallPackages },
                 REQUEST_INSTALL2);
            }
        }


    }
    public class MainDroid : App.IPlatformDep
    {
        public static void OpenPathAsVideo(string path, string name, string subtitleLoc)
        {
            OpenPathsAsVideo(new List<string>() { path }, new List<string>() { name }, subtitleLoc);
        }
        public void DeleteFile(string path)
        {
            //Context context = Android.App.Application.Context;
            Java.IO.File file = new Java.IO.File(path);
            if (file.Exists()) {

                file.Delete();
            }
            /*

            string where = MediaStore.MediaColumns.Data + "=?";
            string[] selectionArgs = new string[] { file.AbsolutePath };
            ContentResolver contentResolver = context.ContentResolver;
            Android.Net.Uri filesUri = MediaStore.Files.GetContentUri("external");

            if (file.Exists()) {
                contentResolver.Delete(filesUri, where, selectionArgs);
            }*/
        }


        static Java.Lang.Thread downloadThread;
        public static void DownloadFromLink(string url, string title, string snackBar = "", string ending = "", bool openFile = false, string descripts = "")
        {
            print("DOWNLOADING: " + url);

            DownloadManager.Request request = new DownloadManager.Request(Android.Net.Uri.Parse(url));
            request.SetTitle(title);
            request.SetDescription(descripts);
            string mainPath = Android.OS.Environment.DirectoryDownloads;
            string subPath = title + ending;
            string fullPath = mainPath + "/" + subPath;

            print("PATH: " + fullPath);

            request.SetDestinationInExternalPublicDir(mainPath, subPath);
            request.SetVisibleInDownloadsUi(true);
            request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);

            DownloadManager manager;
            manager = (DownloadManager) MainActivity.activity.GetSystemService(Context.DownloadService);
     
            long downloadId = manager.Enqueue(request);

            // AUTO OPENS FILE WHEN DONE DOWNLOADING
            if (openFile) {
                downloadThread = new Java.Lang.Thread(() => {
                    try {
                        bool exists = false;
                        while (!exists) {
                            try {
                                string p = manager.GetUriForDownloadedFile(downloadId).Path;
                                exists = true;
                            }
                            catch (System.Exception) {
                                Java.Lang.Thread.Sleep(100);
                            }

                        }
                        Java.Lang.Thread.Sleep(5000);
                        print("OPEN FILE");
                        //            
                        string truePath = ("content://" + Android.OS.Environment.ExternalStorageDirectory + "/" + fullPath);

                       OpenFile(truePath);
                    }
                    finally {
                        downloadThread.Join();
                    }
                });
                downloadThread.Start();
            }
        }
        public static void OpenFile(string link)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(link);//link);
            print("FILE: " + uri);

            Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(uri, "application/vnd.android.package-archive"); //vnd.android.package-archive
            promptInstall.AddFlags(ActivityFlags.NewTask);
            promptInstall.AddFlags(ActivityFlags.GrantReadUriPermission);
            promptInstall.AddFlags(ActivityFlags.NoHistory);
            promptInstall.AddFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(promptInstall);
            /*
            Intent promptInstall = new Intent(Intent.ActionView).SetData(uri);//.SetDataAndType(uri, "application/vnd.android.package-archive");
            //   promptInstall.AddFlags(ActivityFlags.NewTask);
            promptInstall.AddFlags(ActivityFlags.GrantReadUriPermission);
            promptInstall.AddFlags(ActivityFlags.GrantWriteUriPermission);
            promptInstall.AddFlags(ActivityFlags.GrantPrefixUriPermission);
            promptInstall.AddFlags(ActivityFlags.GrantPersistableUriPermission);
            
            promptInstall.AddFlags(ActivityFlags.NewTask);*/


            // Android.App.Application.Context.ApplicationContext.start
            //Android.App.Application.Context.StartService(intent);
            Android.App.Application.Context.StartActivity(promptInstall);
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
            //intent.SetPackage("org.videolan.vlc");
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

        public static string GetPath(bool mainPath, string extraPath)
        {
            return (mainPath ? (Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads) : (Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads + "/Extra")) + extraPath;
        }
        public string DownloadFile(string file, string fileName, bool mainPath, string extraPath)
        {
            return WriteFile(CensorFilename(fileName), GetPath(mainPath, extraPath), file).Path;
        }

        public string DownloadUrl(string url, string fileName, bool mainPath, string extraPath)
        {
            try {

                string basePath = GetPath(mainPath, extraPath);
                Main.print(basePath);
                Java.IO.File _file = new Java.IO.File(basePath);

                _file.Mkdirs();
                basePath += "/" + CensorFilename(fileName);
                Main.print(basePath);
                //webClient.DownloadFile(url, basePath);
                using (WebClient wc = new WebClient()) {
                    wc.DownloadProgressChanged += (o, e) => {
                        if(e.ProgressPercentage == 100) {
                            App.ShowToast("Download Successful");
                            //OpenFile(basePath);
                        }
                        // print(e.ProgressPercentage + "|" + basePath);
                    };
                    wc.DownloadFileAsync(
                        // Param1 = Link of file
                        new System.Uri(url),
                        // Param2 = Path to save
                        basePath
                    );
                }

            }
            catch (Exception) {
                App.ShowToast("Download Failed");
                return "";
            }
            return GetPath(mainPath, extraPath) + "/" + CensorFilename(fileName);
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            print("Progress:" + e.ProgressPercentage);
            //  throw new NotImplementedException();
        }

        static string CensorFilename(string name)
        {
            name = Regex.Replace(name, @"[^A-Za-z0-9\.]+", String.Empty);
            name.Replace(" ", "");
            name = name.ToLower();
            return name;
        }

        public string GetBuildNumber()
        {
            var context = Android.App.Application.Context;
            var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
            return BuildNumber + " " + VersionNumber;
        }

        public void DownloadUpdate(string update)
        {
            string downloadLink = "https://github.com/LagradOst/CloudStream-2/releases/download/" + update + "/com.CloudStreamForms.CloudStreamForms.apk";
            DownloadUrl(downloadLink, "com.CloudStreamForms.CloudStreamForms.apk", true, "");
            /*
            print(Android.OS.Environment.DataDirectory);
            string absolutePath = Android.OS.Environment.ExternalStorageDirectory + "/" + Android.OS.Environment.DirectoryDownloads;

            string fullPath = "content:///storage/emulated/0/Download/CloudStream-2.apk";//com.CloudStreamForms.CloudStreamForms.apk";
            Java.IO.File file = new Java.IO.File(fullPath);

            //  Android.Net.Uri uri = Android.Support.V4.Content.FileProvider.GetUriForFile(MainActivity.activity, "com.CloudStreamForms.CloudStreamForms.Fileprovider", file);
            //  Android.Net.Uri uri = Android.Net.Uri.Parse(fullPath);//link);
            file.SetReadable(true);

            Android.Net.Uri uri = Android.Support.V4.Content.FileProvider.GetUriForFile(Android.App.Application.Context, "com.cloudstreamforms.cloudstreamforms.provider", file);
            print(uri.Path);

          //  Android.Net.Uri contentUri = MainActivity.activity.Ge(getContext(), "com.mydomain.fileprovider", newFile);

          //  MainActivity.activity.GrantUriPermission("com.CloudStreamForms.CloudStreamForms", uri, ActivityFlags.GrantReadUriPermission);
            
            Stream stream = MainActivity.activity.OpenFileOutput(fullPath, FileCreationMode.Private);
            Java.IO.File file = MainActivity.activity.GetFileStreamPath(fullPath);
            var uri = FileProvider.GetUriForFile(MainActivity.activity, "", file);
            // ActivityFlags.GrantReadUriPermissios

            //OpenFile(absolutePath +"/" + "CloudStream-2.apk");
            Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(uri, "application/vnd.android.package-archive"); //vnd.android.package-archive
            promptInstall.AddFlags(ActivityFlags.NewTask);
            promptInstall.AddFlags(ActivityFlags.GrantReadUriPermission);
            promptInstall.AddFlags(ActivityFlags.NoHistory);
            promptInstall.AddFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(promptInstall);
            //  App.ShowToast("Download Started");
            // DownloadFromLink(downloadLink, "com.CloudStreamForms.CloudStreamForms.apk", "Downloading APK", ".apk", true);
            */
        }
    }
}
