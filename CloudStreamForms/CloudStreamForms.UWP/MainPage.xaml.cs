using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CloudStreamForms;

namespace CloudStreamForms.UWP
{
    public sealed partial class MainPage
    {
        MainUWP mainUWP;
        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
           Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            LoadApplication(new CloudStreamForms.App());
            mainUWP = new MainUWP();
            mainUWP.Awake();
        }

    }

    public class MainUWP : CloudStreamForms.App.IPlatformDep
    {
        public static void OpenPathAsVideo(string path)
        {
            System.Diagnostics.Process VLC = new System.Diagnostics.Process();
            VLC.StartInfo.FileName = "\"C:\\Program Files\\VideoLAN\\VLC\\vlc.exe\"";
            VLC.StartInfo.Arguments = "-vvv " + path;
            VLC.Start();
        }
        public void Awake()
        {
            CloudStreamForms.App.platformDep = this;
        }

        public void PlayVlc(string url, string name, string subtitleLoc)
        {
            try {
                MainUWP.OpenPathAsVideo(url);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(url);
            }
        }
    }
}
