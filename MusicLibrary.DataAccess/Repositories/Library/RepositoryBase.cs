using Dapper;
using MusicLibrary.DataAccess.Connection;
using MusicLibrary.DataAccess.QueryHelper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLibrary.DataAccess.Repositories
{
    public abstract class RepositoryBase<T>
    {
        protected ConnectionManager ConnectionManager { get; }

        protected ObjectTableMap Map => SqlStatements.GetMap(typeof(T));

        protected QueryBuilder BaseQuery
        {
            get
            {
                var builder = new QueryBuilder();
                builder.AddSelectStatement(Map.GetSelectStatement("base"));
                builder.AddFromStatement(Map.GetFromStatement("base"));
                return builder;
            }
        }

        protected RepositoryBase(ConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public Task<IEnumerable<T>> GetByIdList<TKey>(List<TKey> ids, string customKeyColumn = null)
        {
            return GetByIdList(BaseQuery, ids, customKeyColumn);
        }

        protected Task<IEnumerable<T>> GetByIdList<TKey>(QueryBuilder builder, List<TKey> ids, string customKeyColumn = null)
        {
            var keyColumn = customKeyColumn ?? Map.Columns.Single(c => c.IsKey).Column;
            builder.AddFilter(ids, "Ids", p => $"{keyColumn} IN {p}");

            return Query<T>(builder);
        }

        protected async Task<IEnumerable<TResult>> Query<TResult>(QueryBuilder query)
        {
            return await Query<TResult>(query.BuildStatement(), query.Parameters);
        }

        protected async Task<IEnumerable<TResult>> Query<TResult>(string query, object parameters)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                return await conn.Connection.QueryAsync<TResult>(query, parameters);
            }
        }

        protected async Task<TResult> ExecuteScalar<TResult>(string query, object parameters)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                return await conn.Connection.ExecuteScalarAsync<TResult>(query, parameters);
            }
        }

        protected async Task Execute(string query, object parameters)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                await conn.Connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<T> GetById(params object[] keys)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                var query = SqlStatements.GetSelectByKeysStatement<T>(keys);
                return await conn.Connection.QueryFirstOrDefaultAsync<T>(query.BuildStatement(), query.Parameters);
            }
        }

        public async Task Save(List<T> entities)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                using (var trans = conn.Connection.BeginTransaction())
                {
                    foreach (var entity in entities)
                    {
                        var sql = SqlStatements.GetInsertStatement<T>();
                        var saved = await conn.Connection.QueryFirstAsync<T>(sql, entity, trans);
                        SqlStatements.SetGeneratedValues(entity, saved);
                    }

                    trans.Commit();
                }
            }
        }
    }
}
