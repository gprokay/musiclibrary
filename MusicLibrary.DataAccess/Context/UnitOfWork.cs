using System;
using System.Transactions;

namespace MusicLibrary.DataAccess.Context
{
    public class UnitOfWork : IDisposable
    {
        private readonly TransactionScope scope;

        public UnitOfWork()
        {
            scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Complete()
        {
            scope.Complete();
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
