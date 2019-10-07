using System;
using System.Collections.Generic;
using System.Text;

namespace CloudStreamForms.Models
{
    public class EpisodeResult
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Rating { set; get; }
        public string PosterUrl { set; get; }
        public string Description { set; get; }
        public double Progress { set; get; } 
        public List<string> mirros { set; get; }
        public List<string> mirrosUrls { set; get; }
        public List<string> subtitles { set; get; }
        public List<string> subtitlesUrls { set; get; }
        public bool epVis { set; get; }
        public LoadResult loadResult { set; get; }
        public bool loadedLinks { set; get; }
    }

    public enum LoadSelection { Play,Download,CopyLink,CopySubtitleLink }

    public struct LoadResult
    {
        public string url;
        public string subtitleUrl;
        public LoadSelection loadSelection;
    }
}
