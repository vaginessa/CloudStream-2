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
        public int IconSize { set; get; } = 48;
        public int BigIconSize { set; get; } = 60;
        public int FastForwardTime { set; get; } = 30;
        public int BackForwardTime { set; get; } = 30;
        public float ScaleAll { set; get; } = 1.4f;
        public float ScaleAllBig { set; get; } = 2f;

        public ChromeCastPage()
        {
            InitializeComponent(); BindingContext = this;
            //https://material.io/resources/icons/?style=baseline
            /*
            LowVol.Source = GetImageSource("round_volume_down_white_48dp.png");
            MaxVol.Source = GetImageSource("round_volume_up_white_48dp.png");*/

         //   UserDialogs.Instance.TimePrompt(new TimePromptConfig() { CancelText = "Cancel", Title = "da", Use24HourClock = false, OkText = "OK", IsCancellable = true });

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PlayList.Source = GetImageSource("round_playlist_play_white_48dp.png");
            StopAll.Source = GetImageSource("round_stop_white_48dp.png");
            BackForward.Source = GetImageSource("round_replay_white_48dp.png");
            FastForward.Source = GetImageSource("round_replay_white_48dp_mirror.png");
            SkipBack.Source = GetImageSource("round_skip_previous_white_48dp.png");
            SkipForward.Source = GetImageSource("round_skip_next_white_48dp.png");
            Pause.Source = GetImageSource("round_pause_white_48dp.png");
            Audio.Source = GetImageSource("round_volume_up_white_48dp.png");
        }

        private void AudioClicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CloudStreamForms.MyPopupPage());

        }

        private void Pause_Clicked(object sender, EventArgs e)
        {

        }
    }
}