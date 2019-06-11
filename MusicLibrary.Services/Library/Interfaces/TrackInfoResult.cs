using GP.Tools.DriveService;

namespace MusicLibrary.Services.Library
{
    public class TrackInfoResult
    {
        public TrackInfo TrackInfo { get; }

        public IFile File { get; }

        public TrackInfoResult(TrackInfo trackInfo, IFile file)
        {
            TrackInfo = trackInfo;
            File = file;
        }
    }
}
