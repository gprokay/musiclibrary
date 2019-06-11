using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Transactions;

namespace MusicLibrary.DataAccess.Connection
{
    public class ConnectionManager
    {
        public bool SeparateConnectionsByThreadId { get; set; } = true;

        private readonly Dictionary<string, DatabaseConnection> _connections = new Dictionary<string, DatabaseConnection>();
        private readonly ConnectionStringSettings _connectionStringSettings;

        public ConnectionManager(ConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        internal DatabaseConnection GetConnection(int maxRetryCount = 1, int retryDelayMilliseconds = 10000)
        {
            var key = GetConnectionKey();

            Exception lastException;

            Debug.WriteLine(string.Format("database connection requested [" + key + "]"));
            lock (key)
            {
                try
                {
                    DatabaseConnection connection = null;

                    lock (_connections)
                    {
                        if (!_connections.TryGetValue(key, out connection))
                        {
                            connection = CreateConnection(key);
                        }

                        connection.ReferenceCount++;
                    }

                    if (connection.Opened)
                    {
                        return connection;
                    }

                    if (TryOpenConnection(maxRetryCount, retryDelayMilliseconds, connection, out lastException))
                    {
                        lock (_connections)
                        {
                            _connections[key] = connection;
                        }

                        return connection;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            var exception = new Exception("can't connect to database", lastException);
            exception.Data.Add("ConnectionStringKey", _connectionStringSettings.Name);
            exception.Data.Add("ProviderName", _connectionStringSettings.ProviderName);
            throw exception;
        }

        private bool TryOpenConnection(int maxRetryCount, int retryDelayMilliseconds, DatabaseConnection connection, out Exception lastException)
        {
            lastException = null;

            lock (connection)
            {
                var sw = Stopwatch.StartNew();

                for (var retry = 0; retry <= maxRetryCount; retry++)
                {
                    try
                    {
                        connection.Connection.Open();
                        connection.Opened = true;
                        Debug.WriteLine(string.Format("database connection opened to {0} using {1} provider in {2}", _connectionStringSettings.Name, _connectionStringSettings.ProviderName, sw.Elapsed));
                        return true;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        Debug.WriteLine(string.Format("can't connect to database, connection string key: {0} using {1} provider, retrying in {2} msec (#{3}): {4}", _connectionStringSettings.Name, _connectionStringSettings.ProviderName, retryDelayMilliseconds * (retry + 1), retry, lastException.Message));
                        Thread.Sleep(retryDelayMilliseconds * (retry + 1));
                    }
                }
            }

            return false;
        }

        public void ConnectionReleased(DatabaseConnection connection)
        {
            if (connection == null)
                return;

            Debug.WriteLine(string.Format("database connection released"));

            lock (connection)
            {
                connection.ReferenceCount--;

                if (connection.ReferenceCount == 0)
                {
                    if (connection.Connection != null)
                    {
                        connection.Connection.Close();
                        connection.Connection.Dispose();
                        connection.Connection = null;

                        Debug.WriteLine(string.Format("database connection DISPOSED"));
                    }
                }
            }

            if (connection.ReferenceCount > 0)
            { return; }

            lock (_connections)
            {
                if (connection.ReferenceCount > 0)
                { return; }

                _connections.Remove(connection.Key);
            }
        }

        private string GetConnectionKey()
        {
            return _connectionStringSettings.Name + "/" +
                _connectionStringSettings.ProviderName + "/" +
                (Transaction.Current != null
                    ? Transaction.Current.TransactionInformation.CreationTime.ToString()
                    : "-") +
                (SeparateConnectionsByThreadId
                    ? ("/" + Thread.CurrentThread.ManagedThreadId.ToString("D", CultureInfo.InvariantCulture))
                    : "-");
        }

        private DatabaseConnection CreateConnection(string key)
        {
            IDbConnection conn = new SqlConnection(_connectionStringSettings.ConnectionString);

            //var providerName = _connectionStringSettings.ProviderName;
            //if (providerName != null)
            //{
            //    var connectionType = Type.GetType(providerName);
            //    if (connectionType != null)
            //    {
            //        conn = Activator.CreateInstance(connectionType) as IDbConnection;
            //    }
            //}

            //if (conn == null)
            //{
            //    conn = DbProviderFactories.GetFactory(providerName).CreateConnection();
            //}

            //conn.ConnectionString = _connectionStringSettings.ConnectionString;

            var connection = new DatabaseConnection()
            {
                Manager = this,
                Key = key,
                Settings = _connectionStringSettings,
                Connection = conn,
                ReferenceCount = 0,
                TransactionWhenCreated = Transaction.Current,
            };

            return connection;
        }
    }
}