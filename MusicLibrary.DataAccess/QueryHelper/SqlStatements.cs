using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace MusicLibrary.DataAccess.QueryHelper
{
    internal static class SqlStatements
    {
        private static ConcurrentDictionary<string, string> InsertStatements = new ConcurrentDictionary<string, string>();

        private static ConcurrentDictionary<string, ObjectTableMap> ObjectTableMaps = new ConcurrentDictionary<string, ObjectTableMap>();

        public static string GetInsertStatement<T>()
        {
            var type = typeof(T);
            var map = GetMap(type);
            return InsertStatements.GetOrAdd(type.FullName, k => GenerateUpsertStatement(map));
        }

        public static void SetGeneratedValues<T>(T original, T saved)
        {
            var type = typeof(T);
            var map = GetMap(type);
            foreach (var column in map.Columns)
            {
                column.Property.SetValue(original, column.Property.GetValue(saved));
            }
        }

        public static QueryBuilder GetSelectByKeysStatement<T>(params object[] keys)
        {
            var type = typeof(T);
            var map = GetMap(type);
            return GenerateSelectByIdStatement(map, keys);
        }

        public static ObjectTableMap GetMap(Type type)
        {
            return ObjectTableMaps.GetOrAdd(type.FullName, k => new ObjectTableMap(type));
        }

        private static string GenerateInsertStatement(ObjectTableMap map)
        {
            var properties = map.Columns.Where(c => !c.DatabaseGenerated).ToList();
            var columnList = string.Join(", ", properties.Select(p => p.Column));
            var valueList = string.Join(", ", properties.Select(p => "@" + p.Property.Name));
            var insert = $"INSERT INTO [{map.Schema}].[{map.Table}] ({columnList}) OUTPUT inserted.* VALUES ({valueList})";
            return insert;
        }

        private static QueryBuilder GenerateSelectByIdStatement(ObjectTableMap map, params object[] keys)
        {
            var keyColumns = map.Columns.Where(c => c.IsKey).ToList();
            if (keys.Length != keyColumns.Count)
            {
                throw new ArgumentException(nameof(keys));
            }

            var builder = new QueryBuilder();

            builder.AddSelectStatement(map.GetSelectStatement());
            builder.AddFromStatement(map.GetFromStatement());
            var keyIndex = 0;
            foreach (var keyColumn in keyColumns)
            {
                builder.AddWhereStatement("AND", $"{keyColumn.Column} = @{keyColumn.Property.Name}");
                builder.AddParameter($"@{keyColumn.Property.Name}", keys[keyIndex++]);
            }

            return builder;
        }

        public static string GenerateUpsertStatement(ObjectTableMap map)
        {
            var notGeneratedColumns = map.Columns.Where(m => !m.DatabaseGenerated).ToList();
            var columns = map.Columns;
            var uniqueColumns = map.Columns.Where(m => m.IsUnique).ToList();
            var idColumns = map.Columns.Where(m => m.IsKey).ToList();

            var builder = new StringBuilder();
            builder.AppendLine($"MERGE INTO [{map.Schema}].[{map.Table}] AS tgt");
            builder.AppendLine($"USING (SELECT {string.Join(", ", columns.Select(p => $"@{p.Property.Name} AS {p.Column}"))}) AS src");
            builder.AppendLine($"ON {string.Join(" AND ", idColumns.Select(ic => $"src.{ic.Column} = tgt.{ic.Column}"))}");
            if (uniqueColumns.Count > 0)
            {
                builder.AppendLine(
                    $"OR {string.Join(" AND ", uniqueColumns.Select(ic => $"src.{ic.Column} = tgt.{ic.Column}"))}");
            }
            builder.AppendLine("WHEN MATCHED ");
            builder.AppendLine($"THEN UPDATE SET {string.Join(", ", notGeneratedColumns.Select(p => $"{p.Column} = src.{p.Column}"))}");
            builder.AppendLine($"WHEN NOT MATCHED THEN INSERT ({string.Join(", ", notGeneratedColumns.Select(p => $"{p.Column}"))}) VALUES ({string.Join(", ", notGeneratedColumns.Select(p => $"src.{p.Column}"))})");
            builder.AppendLine($"OUTPUT {string.Join(", ", columns.Select(p => $"inserted.{p.Column} AS {p.Property.Name}"))};");
            return builder.ToString();
        }
    }
}
