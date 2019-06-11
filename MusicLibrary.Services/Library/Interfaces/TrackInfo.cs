using System;
using System.Collections.Generic;

namespace MusicLibrary.Services.Library
{
    public class TrackInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public int DiscNumber { get; set; }
        public int TrackNumber { get; set; }
        public TimeSpan Duration { get; set; }

        public IEnumerable<string> Artists { get; set; }
        public IEnumerable<string> AlbumArtists { get; set; }
        public IEnumerable<string> Genres { get; set; }
    }
}
