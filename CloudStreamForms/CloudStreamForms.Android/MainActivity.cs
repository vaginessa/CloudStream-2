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
using static CloudStreamForms.CloudStreamCore;
using Android.Provider;
using Acr.UserDialogs;
using static CloudStreamForms.App;
using Plugin.LocalNotifications;

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
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            UserDialogs.Init(this);

            LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.bicon;

            LoadApplication(new App());
            activity = this;

            mainDroid = new MainDroid();
            mainDroid.Awake();

            if (Intent.DataString != null) {
                if (Intent.DataString != "") {
                    MainPage.PushPageFromUrlAndName(Intent.DataString);
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

        public StorageInfo GetStorageInformation(string path = "")
        {
            StorageInfo storageInfo = new StorageInfo();

            long totalSpaceBytes = 0;
            long freeSpaceBytes = 0;
            long availableSpaceBytes = 0;

            /*
              We have to do the check for the Android version, because the OS calls being made have been deprecated for older versions. 
              The ‘old style’, pre Android level 18 didn’t use the Long suffixes, so if you try and call use those on 
              anything below Android 4.3, it’ll crash on you, telling you that that those methods are unavailable. 
              http://blog.wislon.io/posts/2014/09/28/xamarin-and-android-how-to-use-your-external-removable-sd-card/
             */
            if (path == "") {
                totalSpaceBytes = Android.OS.Environment.ExternalStorageDirectory.TotalSpace;
                freeSpaceBytes = Android.OS.Environment.ExternalStorageDirectory.FreeSpace;
                availableSpaceBytes = Android.OS.Environment.ExternalStorageDirectory.UsableSpace;
            }
            else {
                StatFs stat = new StatFs(path); //"/storage/sdcard1"

                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2) {


                    long blockSize = stat.BlockSizeLong;
                    totalSpaceBytes = stat.BlockCountLong * stat.BlockSizeLong;
                    availableSpaceBytes = stat.AvailableBlocksLong * stat.BlockSizeLong;
                    freeSpaceBytes = stat.FreeBlocksLong * stat.BlockSizeLong;

                }
                else {
                    totalSpaceBytes = (long)stat.BlockCount * (long)stat.BlockSize;
                    availableSpaceBytes = (long)stat.AvailableBlocks * (long)stat.BlockSize;
                    freeSpaceBytes = (long)stat.FreeBlocks * (long)stat.BlockSize;
                }
            }

            storageInfo.TotalSpace = totalSpaceBytes;
            storageInfo.AvailableSpace = availableSpaceBytes;
            storageInfo.FreeSpace = freeSpaceBytes;
            return storageInfo;

        }

        public static void OpenPathAsVideo(string path, string name, string subtitleLoc)
        {
            OpenPathsAsVideo(new List<string>() { path }, new List<string>() { name }, subtitleLoc);
        }
        public bool DeleteFile(string path)
        {
            //Context context = Android.App.Application.Context;
            try {
                Java.IO.File file = new Java.IO.File(path);
                if (file.Exists()) {

                    file.Delete();
                }
                return true;
            }


            catch (Exception) {
                return false;
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
        public static void DownloadFromLink(string url, string title, string toast = "", string ending = "", bool openFile = false, string descripts = "")
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
            manager = (DownloadManager)MainActivity.activity.GetSystemService(Context.DownloadService);

            long downloadId = manager.Enqueue(request);



            // AUTO OPENS FILE WHEN DONE DOWNLOADING
            if (openFile || toast != "") {
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
                        Java.Lang.Thread.Sleep(1000);
                        if (toast != "") {
                            App.ShowToast(toast);
                        }
                        if (openFile) {

                            print("OPEN FILE");
                            //            
                            string truePath = ("file://" + Android.OS.Environment.ExternalStorageDirectory + "/" + fullPath);

                            OpenFile(truePath);
                        }
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
            //  Android.Net.Uri uri = Android.Net.Uri.Parse(link);//link);
            Java.IO.File file = new Java.IO.File(Java.Net.URI.Create(link));
            print("Path:" + file.Path);

            Android.Net.Uri photoURI = FileProvider.GetUriForFile(MainActivity.activity.ApplicationContext, (MainActivity.activity.ApplicationContext.PackageName + ".provider.FileProvider"), file);
            Intent promptInstall = new Intent(Intent.ActionView).SetDataAndType(photoURI, "application/vnd.android.package-archive"); //vnd.android.package-archive
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
            CloudStreamCore.print("PATH: " + basePath + "/" + name);
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
            CloudStreamCore.print("AVS: " + absolutePath);

            bool subtitlesEnabled = subtitleLoc != "";
            string writeData = CloudStreamForms.App.ConvertPathAndNameToM3U8(path, name, subtitlesEnabled, "content://" + absolutePath + "/");
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
            // Main.print("Da_ " + Android.Net.Uri.Parse(subfile));

            if (subfile != "") {
                var sfile = Android.Net.Uri.FromFile(new Java.IO.File(subfile));  //"content://" + Android.Net.Uri.Parse(subfile);
                                                                                  //  print(sfile.Path);
                intent.PutExtra("subtitles_location", sfile);//Android.Net.Uri.FromFile(subFile));
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

        public string DownloadUrl(string url, string fileName, bool mainPath, string extraPath, string toast = "", bool isNotification = false, string body = "")
        {
            try {

                string basePath = GetPath(mainPath, extraPath);
                CloudStreamCore.print(basePath);
                Java.IO.File _file = new Java.IO.File(basePath);

                _file.Mkdirs();
                basePath += "/" + CensorFilename(fileName);
                CloudStreamCore.print(basePath);
                //webClient.DownloadFile(url, basePath);
                using (WebClient wc = new WebClient()) {
                    wc.DownloadProgressChanged += (o, e) => {

                        App.OnDownloadProgressChanged(basePath, e);

                        /*
                        if (e.ProgressPercentage == 100) {
                            App.ShowToast("Download Successful");
                            //OpenFile(basePath);
                        }*/
                        // print(e.ProgressPercentage + "|" + basePath);
                    };
                    wc.DownloadFileCompleted += (o, e) => {
                        if (toast != "") {
                            if (isNotification) {
                                App.ShowNotification(toast, body);
                            }
                            else {
                                App.ShowToast(toast);
                            }
                        }
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



        static string CensorFilename(string name, bool toLower = true)
        {
            name = Regex.Replace(name, @"[^A-Za-z0-9\.]+", String.Empty);
            name.Replace(" ", "");
            if (toLower) {
                name = name.ToLower();
            }
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
            App.ShowToast("Download started!");
            //  DownloadUrl(downloadLink, "com.CloudStreamForms.CloudStreamForms.apk", true, "", "Download complete!");
            DownloadFromLink(downloadLink, "com.CloudStreamForms.CloudStreamForms.apk", "Download complete!", "", false, "");

        }

        public string GetDownloadPath(string path, string extraFolder)
        {
            return GetPath(true, extraFolder + "/" + CensorFilename(path, false));
        }

        public string GetExternalStoragePath()
        {
            return Android.OS.Environment.ExternalStorageDirectory.Path;
        }
    }
}
