using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CloudStreamForms.Models
{
    public class EpisodeResult
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Rating { set; get; }
        public string PosterUrl { set; get; }

        public string extraInfo { set; get; }

        public string ExtraText { set; get; }
        public double ExtraProgress { set; get; }
        public bool DownloadNotDone { set; get; }

        string _Description = "";
        public string Description { set { _Description = Settings.EpDecEnabled ? value : ""; } get { return _Description; } }
        public double Progress { set; get; } 
        public List<string> Mirros { set; get; }
        public List<string> mirrosUrls { set; get; }
        public List<string> subtitles { set; get; }
        public List<string> subtitlesUrls { set; get; }
        public bool epVis { set; get; }
       // public LoadResult loadResult { set; get; }
        public bool LoadedLinks { set; get; }
        public string MainTextColor { set; get; } = "#ffffff";
        public string MainDarkTextColor { get; set; } = "#808080"; 
        public string ogTitle { set; get; }
        public double TranslateYOffset
        {
            get {
                if (Device.RuntimePlatform == Device.UWP) {
                    return -20;
                }
                else {
                    return 0;
                }
            }
        }
        public double TranslateYOffsetVertical
        {
            get {
                if (Device.RuntimePlatform == Device.UWP) {
                    return 0;
                }
                else {
                    return 0;
                }
            }
        }
    }
    /*
    public enum LoadSelection { Play,Download,CopyLink,CopySubtitleLink }

    public struct LoadResult
    {
        public string url;
        public string subtitleUrl;
        public LoadSelection loadSelection;
    }*/
}
