using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
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

using Xamarin.Forms.Xaml;

namespace CloudStreamForms.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {


            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);
                ((Style)this.Resources["TabbedPageStyle"]).Setters[0] = ((Style)this.Resources["TabbedPageStyle2"]).Setters[0];
               // ((Style)this.Resources["TabbedPageStyle"]).Setters[2] = ((Style)this.Resources["TabbedPageStyle2"]).Setters[1];

                FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null) {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args) // INTENTDATA
        {
            base.OnActivated(args);



            if (args.Kind == ActivationKind.Protocol) {
                //Main.print("DATA RECIVED");
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                Window.Current.Activate();


                /*
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null) {
                    rootFrame = new Frame();
                }
                Window.Current.Content = rootFrame;
                rootFrame.Navigate(typeof( App));*/
                /// Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { 


                //  });
                // new Frame().Navigate(typeof(MainPage), args);

                var url = eventArgs.Uri.AbsoluteUri;
                if (url != null) {

                    if (url != "") {

                        Main.PushPageFromUrlAndName(url);
                    }
                }
                try {
                    CloudStreamForms.MainPage.intentData = eventArgs.Data.ToString();
                }
                catch (Exception) {

                }
                /*
                try {
                    CloudStreamForms.MainPage.intentData = eventArgs.Data.ToString();
                }
                catch (Exception) {

                }*/
            }

        }

        internal static void OnDownloadProgressChanged(object basePath, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
