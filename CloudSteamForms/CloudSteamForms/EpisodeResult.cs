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
    }
}
