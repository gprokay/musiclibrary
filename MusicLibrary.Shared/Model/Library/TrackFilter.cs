namespace MusicLibrary.Model
{
    public class TrackFilter
    {
        public int? ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public int? PlaylistId { get; set; }
        public bool PlayedByMe { get; set; }
        public bool IsInMyLibrary { get; set; }
        public string Title { get; set; }
    }
}
