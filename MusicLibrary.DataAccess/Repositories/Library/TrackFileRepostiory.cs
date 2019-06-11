using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;

namespace MusicLibrary.DataAccess.Repositories
{
    public class TrackFileRepostiory : RepositoryBase<TrackFileDo>
    {
        public TrackFileRepostiory(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
    }
}
