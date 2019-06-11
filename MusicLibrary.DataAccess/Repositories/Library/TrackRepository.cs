using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;
using MusicLibrary.DataAccess.QueryHelper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public enum TrackOrderColumnDo
    {
        TrackNumber,
        LastPlayed,
        CreatedOn,
        Title
    }

    public class TrackFilterDo
    {
        public int? ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public int? PlaylistId { get; set; }
        public int? PlayedByUserId { get; set; }
        public int? IsInLibraryForUserId { get; set; }
        public string Title { get; set; }
    }

    public class TrackRepository : RepositoryBase<TrackDo>
    {
        public TrackRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public Task AddTrackToUserLibrary(int trackId, int userId)
        {
            var sql = "INSERT INTO [UserLibrary].[UserTrack] (TrackId, UserId) VALUES (@TrackId, @UserId)";
            return Execute(sql, new { TrackId = trackId, UserId = userId });
        }

        public Task RemoveTrackToUserLibrary(int trackId, int userId)
        {
            var sql = "DELETE FROM [UserLibrary].[UserTrack] WHERE TrackId = @TrackId AND UserId = @UserId";
            return Execute(sql, new { TrackId = trackId, UserId = userId });
        }

        public Task<IEnumerable<TrackDo>> SearchTracks(TrackFilterDo filter = null, PagedQuery<TrackOrderColumnDo> page = null)
        {
            QueryBuilder baseQuery = GetTrackQuery(filter, page);

            var sql = baseQuery.BuildStatement();
            return Query<TrackDo>(sql, baseQuery.Parameters);
        }

        public Task<int> CountTracks(TrackFilterDo filter = null)
        {
            QueryBuilder baseQuery = GetTrackQuery(filter);
            var countQuery = new QueryBuilder();
            countQuery.AddSelectStatement("COUNT(*)");
            countQuery.AddFromSubSelect(baseQuery, "base");

            var sql = countQuery.BuildStatement();

            return ExecuteScalar<int>(sql, baseQuery.Parameters);
        }


        public async Task<IEnumerable<int>> GetUserTracksByTrackIdList(List<int> trackIdList, int userId)
        {
            var sql = @"
SELECT t.Id
FROM [Library].[Track] t
JOIN [UserLibrary].[UserTrack] ut ON ut.TrackId = t.Id AND ut.TrackId IN @TrackIds AND ut.UserId = @UserId
";

            return await Query<int>(sql, new { TrackIds = trackIdList, UserId = userId });
        }

        private QueryBuilder GetTrackQuery(TrackFilterDo filter, PagedQuery<TrackOrderColumnDo> page = null)
        {
            var baseQuery = new QueryBuilder();

            baseQuery.AddSelectStatement(Map.GetSelectStatement("track"));
            baseQuery.AddFromStatement(Map.GetFromStatement("track"));

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Title))
                {
                    baseQuery.AddFilter(filter.Title, "title", p => $"track.[Title] LIKE '%' + {p} + '%'");
                }

                if (filter.ArtistId.HasValue)
                {
                    baseQuery.AddFromStatement("JOIN [Library].[TrackArtist] ta ON ta.TrackId = track.Id AND ta.ArtistId = @ArtistId");
                    baseQuery.AddParameter("ArtistId", filter.ArtistId.Value);
                }

                if (filter.PlaylistId.HasValue)
                {
                    baseQuery.AddFromStatement("JOIN [UserLibrary].[PlaylistTrack] pt ON pt.TrackId = track.Id AND pt.PlaylistId = @PlaylistId");
                    baseQuery.AddParameter("PlaylistId", filter.PlaylistId.Value);
                }

                if (filter.AlbumId.HasValue)
                {
                    baseQuery.AddFilter(filter.AlbumId.Value, "AlbumId", p => $"track.AlbumId = {p}");
                }

                if (filter.PlayedByUserId.HasValue)
                {
                    baseQuery.AddFromStatement("JOIN [UserLibrary].[PlayCount] pc ON pc.TrackId = track.Id AND pc.UserId = @UserId1");
                    baseQuery.AddParameter("UserId1", filter.PlayedByUserId.Value);
                }

                if (filter.IsInLibraryForUserId.HasValue)
                {
                    baseQuery.AddFromStatement("JOIN [UserLibrary].[UserTrack] ut ON ut.TrackId = track.Id AND ut.UserId = @UserId2");
                    baseQuery.AddParameter("UserId2", filter.IsInLibraryForUserId.Value);
                }
            }

            if (page != null)
            {
                if (page.OrderBy != null)
                {
                    switch (page.OrderBy.Column)
                    {
                        case TrackOrderColumnDo.TrackNumber:
                            baseQuery.AddOrderByStatement("DiscNumber", page.OrderBy.Direction);
                            baseQuery.AddOrderByStatement("TrackNumber", page.OrderBy.Direction);
                            break;
                        case TrackOrderColumnDo.LastPlayed:
                            baseQuery.AddSelectStatement("(SELECT MAX(Timestamp) FROM [UserLibrary].[PlayCount] WHERE TrackId = track.Id) AS Timestamp");
                            baseQuery.AddOrderByStatement("Timestamp", page.OrderBy.Direction);
                            break;
                        case TrackOrderColumnDo.CreatedOn:
                            baseQuery.AddOrderByStatement("CreatedOn", page.OrderBy.Direction);
                            break;
                        case TrackOrderColumnDo.Title:
                            baseQuery.AddOrderByStatement("track.Title", page.OrderBy.Direction);
                            break;
                    }
                }

                if (page.Skip.HasValue && page.Take.HasValue)
                {
                    baseQuery.AddPaging(page.Skip, page.Take);
                }
            }

            return baseQuery;
        }
    }
}