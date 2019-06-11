using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public class GenreRepository : RepositoryBase<GenreDo>
    {
        public GenreRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public async Task<ILookup<int, GenreDo>> GetGenresByTrackIdList(List<int> trackIdList)
        {
            var builder = BaseQuery;
            builder.AddFromStatement("JOIN [Library].[TrackGenre] ta ON ta.GenreId = base.Id AND ta.TrackId IN @TrackIds");
            builder.AddSelectStatement("ta.TrackId AS TrackId");
            builder.AddParameter("@TrackIds", trackIdList);

            var rows = await Query<(int Id, string Name, int TrackId)>(builder);
            return rows.ToLookup(r => r.TrackId, r => new GenreDo { Id = r.Id, Name = r.Name });
        }
    }
}
