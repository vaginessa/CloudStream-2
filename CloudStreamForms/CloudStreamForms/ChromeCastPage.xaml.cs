using CloudStreamForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CloudStreamForms.Main;
using static CloudStreamForms.App;
using Rg.Plugins.Popup.Services;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChromeCastPage : ContentPage
    {
        public EpisodeResult episodeResult;

        public string TitleName { set; get; } = "Iron Man";
        public string DescriptName { set; get; } = "Episode 1. Hello World!";
        public string PosterUrl { set; get; } = "https://m.media-amazon.com/images/M/MV5BMTczNTI2ODUwOF5BMl5BanBnXkFtZTcwMTU0NTIzMw@@._V1_UX1820_CR0,0,1820,2680_AL_.jpg";
        public int IconSize { set; get; } = 30;
        public int BigIconSize { set; get; } = 60;

        public ChromeCastPage()
        {
            InitializeComponent(); BindingContext = this;
            //https://material.io/resources/icons/?style=baseline
            StopAll.Source = GetImageSource("round_stop_white_48dp.png");
            LowVol.Source = GetImageSource("round_volume_down_white_48dp.png");
            MaxVol.Source = GetImageSource("round_volume_up_white_48dp.png");
            PlayList.Source = GetImageSource("round_playlist_play_white_48dp.png");

         //   UserDialogs.Instance.TimePrompt(new TimePromptConfig() { CancelText = "Cancel", Title = "da", Use24HourClock = false, OkText = "OK", IsCancellable = true });

        }

        private void StopAll_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CloudStreamForms.MyPopupPage());

        }
    }
}