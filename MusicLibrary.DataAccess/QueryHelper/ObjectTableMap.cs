using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace MusicLibrary.DataAccess.QueryHelper
{
    public class UniqueAttribute : Attribute { }

    public class PropertyColumnMap
    {
        public PropertyInfo Property { get; }

        public string Column { get; }

        public bool DatabaseGenerated { get; }

        public bool IsKey { get; }

        public bool IsUnique { get; }

        public PropertyColumnMap(PropertyInfo property)
        {
            Property = property;
            Column = property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;
            DatabaseGenerated = property.GetCustomAttribute<DatabaseGeneratedAttribute>() != null;
            IsKey = property.GetCustomAttribute<KeyAttribute>() != null;
            IsUnique = property.GetCustomAttribute<UniqueAttribute>() != null;
        }
    }

    public class ObjectTableMap
    {
        public string Schema { get; }

        public string Table { get; }

        public Type Type { get; }

        public List<PropertyColumnMap> Columns { get; }

        public ObjectTableMap(Type type)
        {
            Type = type;
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            Schema = tableAttribute?.Schema ?? "dbo";
            Table = tableAttribute?.Name ?? type.Name;
            Columns = type.GetProperties().Select(p => new PropertyColumnMap(p)).ToList();
        }

        internal object[] Select()
        {
            throw new NotImplementedException();
        }
    }
}
