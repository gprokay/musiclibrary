using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public class ArtistRepository : RepositoryBase<ArtistDo>
    {
        public ArtistRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public async Task<ILookup<int, ArtistDo>> GetArtistsByTrackIdList(List<int> trackIdList)
        {
            var builder = BaseQuery;
            builder.AddFromStatement("JOIN [Library].[TrackArtist] ta ON ta.ArtistId = base.Id AND ta.TrackId IN @TrackIds");
            builder.AddSelectStatement("ta.TrackId AS TrackId");
            builder.AddParameter("@TrackIds", trackIdList);

            var rows = await Query<(int Id, string Name, int TrackId)>(builder);
            return rows.ToLookup(r => r.TrackId, r => new ArtistDo { Id = r.Id, Name = r.Name });
        }
    }
}
