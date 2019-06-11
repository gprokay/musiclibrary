using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;

namespace MusicLibrary.DataAccess.Repositories
{
    public class AlbumArtistRepository : RepositoryBase<AlbumArtistDo>
    {
        public AlbumArtistRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
    }
}
