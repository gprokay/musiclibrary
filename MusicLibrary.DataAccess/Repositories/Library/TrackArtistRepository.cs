using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;

namespace MusicLibrary.DataAccess.Repositories
{
    public class TrackArtistRepository : RepositoryBase<TrackArtistDo>
    {
        public TrackArtistRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
    }
}
