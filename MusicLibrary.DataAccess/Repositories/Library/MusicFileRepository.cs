using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;
using MusicLibrary.DataAccess.QueryHelper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public class MusicFileRepository : RepositoryBase<MusicFileDo>
    {
        public MusicFileRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public async Task<IEnumerable<string>> GetMusicFileIds(string machineId, string driveType)
        {
            var builder = new QueryBuilder();
            builder.AddSelectStatement("FileId");
            builder.AddFromStatement(Map.GetFromStatement());
            builder.AddFilter(machineId, "MachineId", p => "MachineId = " + p);
            builder.AddFilter(driveType, "DriveType", p => "DriveType = " + p);
            return await Query<string>(builder);
        }

        public async Task<IEnumerable<MusicFileDo>> GetFileSystemOnlyFiles(string machineId)
        {
            var sql = @"
SELECT mf.*
FROM [Library].[MusicFile] mf
LEFT OUTER JOIN [Library].[MusicFile] azuref ON mf.TrackId = azuref.TrackId AND azuref.DriveType = 'azure' AND mf.MachineId = azuref.MachineId
WHERE mf.DriveType = 'filesystem' AND azuref.Id IS NULL AND mf.MachineId = @MachineId
";

            return await Query<MusicFileDo>(sql, new { MachineId = machineId });
        }

        public async Task<IEnumerable<MusicFileDo>> GetByTrackId(int trackId)
        {
            var sql = @"SELECT * FROM [Library].[MusicFile] WHERE TrackId = @TrackId";

            return await Query<MusicFileDo>(sql, new { TrackId = trackId });
        }
    }
}
