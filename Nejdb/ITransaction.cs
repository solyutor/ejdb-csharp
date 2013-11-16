using System;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates transaction for collection
    /// </summary>
    public class Transaction : IDisposable
    {
        private readonly Collection _collection;

        internal Transaction(Collection collection)
        {
            _collection = collection;
            _collection.BeginTransactionInternal();
        }

        /// <summary>
        /// Commits the transaction
        /// </summary>
        public void Commit()
        {
            if (!_collection.TransactionActive)
            {
                throw new InvalidOperationException("Could not commit transaction. No active transaction.");
            }
            _collection.CommitTransactionInternal();
        }

        /// <summary>
        /// Returns true if transaction is active, false otherwise.
        /// </summary>
        public bool Active
        {
            get { return _collection.TransactionActive; }
        }

        /// <summary>
        /// Rollbacks all changes
        /// </summary>
        public void Rollback()
        {
            if (!_collection.TransactionActive)
            {
                throw new InvalidOperationException("Could not rollback transaction.  No active transaction.");
            }
            _collection.RollbackTransactionInternal();
        }

        /// <summary>
        /// Disposes the transaction (rollback if it not committed)
        /// </summary>
        public void Dispose()
        {
            if (_collection.TransactionActive)
            {
                Rollback();
            }
        }
    }
}