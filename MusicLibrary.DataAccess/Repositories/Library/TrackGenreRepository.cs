using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;

namespace MusicLibrary.DataAccess.Repositories
{
    public class TrackGenreRepository : RepositoryBase<TrackGenreDo>
    {
        public TrackGenreRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
    }
}
