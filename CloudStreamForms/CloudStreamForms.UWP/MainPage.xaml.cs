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
namespace CloudStreamForms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
           Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            LoadApplication(new CloudStreamForms.App());
            CloudStreamForms.App.PlayVlc += App_PlayVlc;
        }

        private void App_PlayVlc(object sender, string e)
        {
            try {
                MainUWP.OpenPathAsVideo(e);
            }
            catch (Exception) {
                CloudStreamForms.App.OpenBrowser(e);
            }
        }
    }

    public static class MainUWP
    {
        public static void OpenPathAsVideo(string path)
        {
            System.Diagnostics.Process VLC = new System.Diagnostics.Process();
            VLC.StartInfo.FileName = "\"C:\\Program Files\\VideoLAN\\VLC\\vlc.exe\"";
            VLC.StartInfo.Arguments = "-vvv " + path;
            VLC.Start();
        }

    }
}
