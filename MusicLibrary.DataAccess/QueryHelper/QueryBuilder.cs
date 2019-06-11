using Dapper;
using System;
using System.Text;

namespace MusicLibrary.DataAccess.QueryHelper
{
    internal class QueryWithParameters
    {
        public string Statement { get; }

        public DynamicParameters Parameters { get; }

        public QueryWithParameters(QueryBuilder builder)
        {
            Statement = builder.BuildStatement();
            Parameters = builder.Parameters;
        }

        public QueryWithParameters(string statement, DynamicParameters parameters)
        {
            Statement = statement;
            Parameters = parameters;
        }
    }

    public class QueryBuilder
    {
        private readonly StringBuilder _preStatement = new StringBuilder();
        private readonly StringBuilder _selectStatement = new StringBuilder();
        private readonly StringBuilder _fromStatement = new StringBuilder();
        private readonly StringBuilder _whereStatement = new StringBuilder();
        private readonly StringBuilder _orderByStatement = new StringBuilder();
        private readonly StringBuilder _postStatement = new StringBuilder();

        public DynamicParameters Parameters { get; } = new DynamicParameters();

        public QueryBuilder()
        {
        }

        public QueryBuilder(string selectStatement, string fromStatement)
        {
            AddSelectStatement(selectStatement);
            AddFromStatement(fromStatement);
        }

        public void AddPreStatement(string statement)
        {
            AddStatement(_preStatement, statement);
        }

        public void AddSelectStatement(string statement)
        {
            if (string.IsNullOrEmpty(statement))
                return;
            if (_selectStatement.Length > 1)
                _selectStatement.Append(",\n");
            _selectStatement.Append("\t");
            _selectStatement.Append(statement);
        }

        public void AddFromStatement(string statement)
        {
            AddStatement(_fromStatement, statement);
        }

        public void AddPostStatement(string statement)
        {
            AddStatement(_postStatement, statement);
        }

        public void AddFromSubSelect(QueryBuilder innerBuilder, string alias)
        {
            AddFromStatement("(" + innerBuilder.BuildStatement().Replace("\n", "\n\t") + ") " + alias);
            foreach (var p in innerBuilder.Parameters.ParameterNames)
            {
                Parameters.Add(p, innerBuilder.Parameters.Get<object>(p));
            }
        }

        public void AddFromSubSelect(string join, string on, QueryBuilder innerBuilder, string alias)
        {
            AddFromStatement(join + " (" + innerBuilder.BuildStatement().Replace("\n", "\n\t") + ") " + alias + " ON " + on);
            foreach (var p in innerBuilder.Parameters.ParameterNames)
            {
                Parameters.Add(p, innerBuilder.Parameters.Get<object>(p));
            }
        }

        public void AddOrderByStatement(string column, OrderByDirection direction)
        {
            if (_orderByStatement.Length > 0)
                _orderByStatement.Append(", ");
            _orderByStatement.Append(column);
            _orderByStatement.Append(" ");
            _orderByStatement.Append(direction == OrderByDirection.Ascending ? "ASC" : "DESC");
        }

        public void SetOrderByStatement(string statement)
        {
            if (_orderByStatement.Length > 0)
                _orderByStatement.Clear();
            _orderByStatement.Append(statement);
        }

        public string GetOrderByStatement()
        {
            return _orderByStatement.ToString();
        }

        public void AddWhereStatement(string operation, string statement)
        {
            if (_whereStatement.Length > 0)
                _whereStatement.Append(" " + operation + " ");
            _whereStatement.Append(statement);
        }

        private void AddStatement(StringBuilder builder, string statement)
        {
            if (string.IsNullOrEmpty(statement))
                return;
            if (builder.Length > 1)
                builder.Append(" \n");
            builder.Append(statement);
        }

        public void AddParameter(string name, object value)
        {
            Parameters.Add(name, value);
        }

        public void AddFilter<T>(T value, string paramName, Func<string, string> whereGenerator, string extraJoin = null)
        {
            if (!paramName.StartsWith("@"))
            {
                paramName = "@" + paramName;
            }

            AddFilter(value, paramName, whereGenerator(paramName), p => p, extraJoin);
        }

        public void AddFilter<T>(T value, string paramName, string wherePart, Func<T, object> paramValueGenerator, string extraJoin = null)
        {
            if (value == null)
                return;

            if (!string.IsNullOrEmpty(extraJoin))
            {
                AddFromStatement(extraJoin);
            }

            AddWhereStatement("AND", wherePart);

            AddParameter(paramName, paramValueGenerator(value));
        }

        public void AddPaging(int? skip, int? take)
        {
            AddPostStatement("OFFSET @PagingSkip ROWS FETCH NEXT @PagingTake ROWS ONLY");
            AddParameter("PagingSkip", skip ?? 0);
            AddParameter("PagingTake", take ?? 10);
        }

        public string BuildStatement()
        {
            return "SELECT" + (_preStatement.Length > 0 ? " " + _preStatement.ToString() : "") + "\n" +
                _selectStatement.ToString() +
                "\nFROM " + _fromStatement.ToString() +
                (_whereStatement.Length > 0 ? "\nWHERE " + _whereStatement.ToString() : "") +
                (_orderByStatement.Length > 0 ? "\nORDER BY " + _orderByStatement.ToString() : "") +
                (_postStatement.Length > 0 ? "\n" + _postStatement.ToString() : "");
        }
    }
}