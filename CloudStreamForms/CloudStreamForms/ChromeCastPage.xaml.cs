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
using static CloudStreamForms.MainChrome;

namespace CloudStreamForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChromeCastPage : ContentPage
    {
        public EpisodeResult episodeResult;

        public string TitleName { set; get; } = "Iron Man";
        public string DescriptName { set; get; } = "Episode 1. Hello World!";
        public string PosterUrl { set; get; } =   "https://m.media-amazon.com/images/M/MV5BMTczNTI2ODUwOF5BMl5BanBnXkFtZTcwMTU0NTIzMw@@._V1_UX1820_CR0,0,1820,2680_AL_.jpg";
        public int IconSize { set; get; } = 48;
        public int BigIconSize { set; get; } = 60;
        public int FastForwardTime { set; get; } = 30;
        public int BackForwardTime { set; get; } = 30;
        public float ScaleAll { set; get; } = 1.4f;
        public float ScaleAllBig { set; get; } = 2f;

        int currentSelected = 0;


        void SelectMirror()
        {
            CastVideo(episodeResult.mirrosUrls[currentSelected], episodeResult.Mirros[currentSelected], CurrentTime);
        }

        public ChromeCastPage()
        {

            InitializeComponent(); BindingContext = this;
            
            TitleName = episodeResult.Title;
            PosterUrl = episodeResult.PosterUrl;
            DescriptName = episodeResult.Mirros[currentSelected];

            //https://material.io/resources/icons/?style=baseline
            VideoSlider.DragStarted += (o, e) => {
                draging = true;
            };
            VideoSlider.DragCompleted += (o, e) => {
                MainChrome.SetChromeTime(VideoSlider.Value * CurrentCastingDuration);
                draging = false;
                UpdateTxt();
            };
            Pause.Clicked += (o, e) => {
                SetPause(!IsPaused);
                PauseAndPlay(!IsPaused);
            };
            FastForward.Clicked += (o, e) => {
                SeekMedia(FastForwardTime);
            };
            BackForward.Clicked += (o, e) => {
                SeekMedia(-BackForwardTime);
            };

            SkipForward.Clicked += (o, e) => {
                currentSelected++;
                if (currentSelected > episodeResult.Mirros.Count) { currentSelected = 0; }
            };
            SkipBack.Clicked += (o, e) => {
                currentSelected--;
                if (currentSelected < 0) { currentSelected = episodeResult.Mirros.Count - 1; }
            };

            PlayList.Clicked += async (o, e) => {
                string a = await DisplayActionSheet("Select Mirror", "Cancel", null, episodeResult.Mirros.ToArray());

                for (int i = 0; i < episodeResult.Mirros.Count; i++) {
                    if (a == episodeResult.Mirros[i]) {
                        currentSelected = i;
                        SelectMirror();
                        return;
                    }
                }
            };
            ConstUpdate();
            /*
            LowVol.Source = GetImageSource("round_volume_down_white_48dp.png");
            MaxVol.Source = GetImageSource("round_volume_up_white_48dp.png");*/

            //   UserDialogs.Instance.TimePrompt(new TimePromptConfig() { CancelText = "Cancel", Title = "da", Use24HourClock = false, OkText = "OK", IsCancellable = true });

        }

        bool draging = false;
        public async void ConstUpdate()
        {
            while (true) {
                await Task.Delay(1000);
                UpdateTxt();
            }
        }

        public void UpdateTxt()
        {
            StartTxt.Text = ConvertTimeToString(CurrentTime);
            EndTxt.Text = ConvertTimeToString(CurrentCastingDuration - CurrentTime);
            if (!draging) {
                VideoSlider.Value = CurrentTime / CurrentCastingDuration;
            }
        }

        void SetPause(bool paused)
        {
            Pause.Source = paused ? GetImageSource("round_play_arrow_white_48dp.png") : GetImageSource("round_pause_white_48dp.png");
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
            Audio.Source = GetImageSource("round_volume_up_white_48dp.png");
            SetPause(IsPaused);
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