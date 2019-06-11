namespace MusicLibrary.Model
{
    public class SearchTrackRequest
    {
        public TrackFilter Filter { get; set; }

        public Page<TrackOrderColumn> Page { get; set; }
    }
}
