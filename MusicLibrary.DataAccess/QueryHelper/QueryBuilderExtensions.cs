using System.Linq;

namespace MusicLibrary.DataAccess.QueryHelper
{
    internal static class QueryBuilderExtensions
    {
        public static string GetFromStatement(this ObjectTableMap map, string alias = null)
        {
            if (alias == null)
            {
                alias = string.Empty;
            }
            else
            {
                alias = " AS " + alias;
            }

            var from = $"[{map.Schema}].[{map.Table}]" + alias;
            return from;
        }

        public static string GetSelectStatement(this ObjectTableMap map, string alias = null)
        {
            if (alias == null)
            {
                alias = string.Empty;
            }
            else
            {
                alias = alias + ".";
            }

            var select = string.Join(", ", map.Columns.Select(c => alias + $"[{c.Column}] as [{c.Property.Name}]"));
            return select;
        }
    }
}
