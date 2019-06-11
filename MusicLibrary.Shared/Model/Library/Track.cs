using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.Model
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DiscNumber { get; set; }
        public int TrackNumber { get; set; }
        public double Duration { get; set; }
        public Album Album { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public bool SavedToLibrary { get; set; }

        public string GetArtists()
        {
            return string.Join(", ", Artists.Select(a => a.Name));
        }
    }
}
