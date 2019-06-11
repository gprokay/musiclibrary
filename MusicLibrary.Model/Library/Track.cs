using System;
using System.Collections.Generic;

namespace MusicLibrary.Model
{

    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DiscNumber { get; set; }
        public int TrackNumber { get; set; }
        public TimeSpan Duration { get; set; }
        public Album Album { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public bool SavedToLibrary { get; set; }
    }
}
