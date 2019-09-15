using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSteamForms.Models
{
    public class EpisodeResult
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Rating { set; get; }
        public string PosterUrl { set; get; }
        public string Description { set; get; }
        public double Progress { set; get; }

        public List<string> Mirros { set; get; }

        public List<string> MirrosUrls { set; get; }

        public List<string> Subtitles { set; get; }
        public List<string> SubtitlesUrls { set; get; }

        public bool EpVis { set; get; }
        public LoadResult LoadResult { set; get; }
    }

    public enum LoadSelection { Play,Download,CopyLink,CopySubtitleLink }

    public struct LoadResult
    {
        public string url;
        public string subtitleUrl;
        public LoadSelection loadSelection;
    }
}
