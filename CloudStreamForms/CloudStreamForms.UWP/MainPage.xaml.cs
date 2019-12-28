using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using static CloudStreamForms.Main;
using _App = CloudStreamForms.App;

namespace CloudStreamForms.UWP
{
    public sealed partial class MainPage
    {
        MainUWP mainUWP;
        public static MainPage mainPage;
        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
           Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            LoadApplication(new CloudStreamForms.App());
            mainPage = this;
            mainUWP = new MainUWP();
            mainUWP.Awake();

        }



    }

    public class MainUWP : CloudStreamForms.App.IPlatformDep
    {

        public static string GetPath(string filename)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            return storageFolder.Path + "\\" + CensorFilename(filename);
        }

        static string CensorFilename(string name)
        {
            name = Regex.Replace(name, @"[^A-Za-z0-9\.]+", String.Empty);
            name.Replace(" ", "");
            name = name.ToLower();
            return name;
        }
        public static async Task CreateFile(string filename, byte[] write, bool autoOpen = true)
        {
            filename = CensorFilename(filename);
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var file = await storageFolder.GetFileAsync(filename);
            var s = await file.OpenStreamForWriteAsync();

            s.Write(write, 0, write.Length);
            s.Close();
            if (autoOpen) {
                var success = await Windows.System.Launcher.LaunchFileAsync(file);
            }
        }

        public static async Task OpenPathsAsVideo(List<string> path, List<string> name, string subtitleLoc = "")
        {

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            bool subEnabled = subtitleLoc != "";

            await CreateFile(CloudStreamForms.App.baseM3u8Name, CloudStreamForms.App.ConvertPathAndNameToM3U8Bytes(path, name, subEnabled, storageFolder.Path + "\\"));
            if (subEnabled) {
                await CreateFile(CloudStreamForms.App.baseSubtitleName, Encoding.UTF8.GetBytes(subtitleLoc));
            }

            //Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync(CloudStreamForms.App.baseM3u8Name, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            //#EXTVLCOPT:sub-file=sample2.srt
        }

        public static async void OpenPathAsVideo(string path, string name = "", string subtitleLoc = "")
        {
            await OpenPathsAsVideo(new List<string>() { path }, new List<string>() { name }, subtitleLoc);
            // string vlcPath = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";
            //string args = "-vvv " + path;
            /*
            System.Diagnostics.Process VLC = new System.Diagnostics.Process();
            VLC.StartInfo.FileName = 
            VLC.StartInfo.Arguments = 
            VLC.Start();
            */

            /*
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null) {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            }
            StorageFolder newFolder;

            newFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
            await newFolder.CreateFileAsync("testYeet.txt");*/

            /*
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                //  try {
                var result = await ProcessLauncher.RunToCompletionAsync(vlcPath, args, options);
            //    Console.WriteLine(result.ExitCode + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                //   }
                //  catch (System.Exception e) {

                //}
            });*/

            // await StorageApplicationPermissions.FutureAccessList.GetFileAsync("C:\\Program Files\\VideoLAN\\VLC");
            //   await Windows.System.ProcessLauncher.RunToCompletionAsync(vlcPath, args, new Windows.System.ProcessLauncherOptions() { WorkingDirectory = "C:\\Program Files\\VideoLAN\\VLC" });
            /*
            var uriPath = new Uri(path);

            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(uriPath);*/
            //CloudStreamForms.App.OpenBrowser(@"file:///C:/Program Files/VideoLAN/VLC/vlc.exe");
        }
        public void Awake()
        {
            CloudStreamForms.App.platformDep = this;
        }

        public void PlayVlc(string url, string name, string subtitleLoc)
        {
            //MainUWP.OpenPathAsVideo(url,name);
            try {
                MainUWP.OpenPathAsVideo(url, name, subtitleLoc);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url);
            }
        }
        public void PlayVlc(List<string> url, List<string> name, string subtitleLoc)
        {
            //MainUWP.OpenPathAsVideo(url,name);
            try {
                MainUWP.OpenPathsAsVideo(url, name, subtitleLoc);

            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url.FirstOrDefault());
            }
        }

        public void ShowToast(string message, double duration)
        {
            Message.ShowMessage(message, duration);
        }

        public string DownloadFile(string file, string fileName, bool mainPath, string extraPath)
        {
            DownloadFile(Encoding.UTF8.GetBytes(file), fileName, mainPath, extraPath);
            return GetPath(fileName);
        }
        public void DownloadFile(byte[] file, string fileName, bool mainPath, string extraPath)
        {
            CreateFile(fileName, file, false);
        }


        public string DownloadUrl(string url, string fileName, bool mainPath, string extraPath, string toast = "", bool isNotification = false, string body = "")
        {

            Main.print(fileName);

            string basePath = GetPath(fileName);

            try {

                //webClient.DownloadFile(url, basePath);
                using (WebClient wc = new WebClient()) {
                    wc.DownloadProgressChanged += (o, e) => {

                        _App.OnDownloadProgressChanged(basePath, e);

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
                                _App.ShowNotification(toast, body);
                            }
                            else {
                                ShowToast(toast, 2.5);
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
                print("PROGRESS FAILED");
                _App.ShowToast("Download Failed");
                return "";
            }
            return basePath;

            try {
                TempThred tempThred = new TempThred();
                tempThred.typeId = 4; // MAKE SURE THIS IS BEFORE YOU CREATE THE THRED
                tempThred.Thread = new System.Threading.Thread(() => {
                    try {
                        WebClient webClient = new WebClient();
                        byte[] data = webClient.DownloadData(url);
                        DownloadFile(data, fileName, mainPath, extraPath);
                        //if (!GetThredActive(tempThred)) { return; }; // COPY UPDATE PROGRESS

                    }
                    finally {
                        JoinThred(tempThred);
                    }
                });
                tempThred.Thread.Name = "Download Thread";
                tempThred.Thread.Start();


                return GetPath(fileName);

            }
            catch (Exception) {
                CloudStreamForms.App.ShowToast("Download Failed");
                return "";
            }
        }

        public string GetBuildNumber()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            return v.Major + "." + v.Minor + "." + v.Build;
        }

        public bool DeleteFile(string path)
        {
            try {
                System.IO.File.Delete(path);
                return true;
            }
            catch (Exception) {
                _App.ShowToast("Error deleting file");
                return false;
            }
        }

        public void DownloadUpdate(string update)
        {
            // throw new NotImplementedException();
        }

        public string GetDownloadPath(string path, string extraFolder)
        {
            return GetPath(CensorFilename(path));
        }

        public _App.StorageInfo GetStorageInformation(string path = "")
        {
            if (path == "") {
                path = GetExternalStoragePath();
            }
            ulong FreeBytesAvailable;
            ulong TotalNumberOfBytes;
            ulong TotalNumberOfFreeBytes;

            bool success = GetDiskFreeSpaceEx(ApplicationData.Current.LocalFolder.Path,
                                              out FreeBytesAvailable,
                                              out TotalNumberOfBytes,
                                              out TotalNumberOfFreeBytes);            
            //  var drive = DriveInfo.GetDrives().Where(t => t.Name == path).ToList()[0];

            return new _App.StorageInfo() { AvailableSpace=(long) FreeBytesAvailable, FreeSpace= (long)TotalNumberOfFreeBytes, TotalSpace= (long)TotalNumberOfBytes };//AvailableSpace = drive.AvailableFreeSpace, FreeSpace = drive.TotalFreeSpace, TotalSpace = drive.TotalSize };

        }
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out ulong lpFreeBytesAvailable,
           out ulong lpTotalNumberOfBytes,
           out ulong lpTotalNumberOfFreeBytes);


        public string GetExternalStoragePath()
        {
            return "C:\\";
        }



    }

    public static class Message
    {
        private const double LONG_DELAY = 3.5;
        private const double SHORT_DELAY = 2.0;

        public static void LongAlert(string message) =>
          ShowMessage(message, LONG_DELAY);

        public static void ShortAlert(string message) =>
          ShowMessage(message, SHORT_DELAY);

        public static void ShowMessage(string message, double duration)
        {
            var label = new TextBlock {
                Text = message,
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var style = new Style { TargetType = typeof(FlyoutPresenter) };
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Windows.UI.Colors.White)));
            style.Setters.Add(new Setter(FrameworkElement.MaxHeightProperty, 1));
            var flyout = new Flyout {
                Content = label,
                Placement = FlyoutPlacementMode.Bottom,

                FlyoutPresenterStyle = style,
            };

            flyout.ShowAt(Window.Current.Content as FrameworkElement);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            timer.Tick += (sender, e) => {
                timer.Stop();
                flyout.Hide();
            };
            timer.Start();
        }
    }
}
