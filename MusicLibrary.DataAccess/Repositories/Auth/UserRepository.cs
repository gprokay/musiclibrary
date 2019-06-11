using Dapper;
using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.Entities;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public class UserRepository : RepositoryBase<UserDo>
    {
        public UserRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public async Task<UserDo> GetUserByEmail(string email)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                return await conn.Connection.QueryFirstOrDefaultAsync<UserDo>(
                    "SELECT * FROM [Auth].[User] WHERE Email = @Email",
                    new { Email = email });
            }
        }
    }
}
