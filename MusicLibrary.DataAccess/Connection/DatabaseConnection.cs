using System;
using System.Configuration;
using System.Data;
using System.Transactions;

namespace MusicLibrary.DataAccess.Connection
{
    public class DatabaseConnection : IDisposable
    {
        public ConnectionManager Manager { get; set; }
        public string Key { get; set; }
        public ConnectionStringSettings Settings { get; set; }
        public IDbConnection Connection { get; set; }
        public int ReferenceCount { get; set; } = 0;
        public object Lock { get; set; } = new object();
        public Transaction TransactionWhenCreated { get; set; }

        public bool Opened { get; set; }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Manager.ConnectionReleased(this);
            }

            if (ReferenceCount == 0)
            {
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}