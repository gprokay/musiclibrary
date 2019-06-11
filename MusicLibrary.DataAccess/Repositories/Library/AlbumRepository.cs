using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;

namespace MusicLibrary.DataAccess.Repositories
{
    public class AlbumRepository : RepositoryBase<AlbumDo>
    {
        public AlbumRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }
    }
}
