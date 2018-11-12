using System;
using Data.Repositories;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly bool disposed;

        private TokenRepository _tokenRepository;

        public UnitOfWork(DatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
            disposed = false;
        }

        public DatabaseContext DatabaseContext { get; }

        public TokenRepository TokenRepository => _tokenRepository ?? (_tokenRepository = new TokenRepository(DatabaseContext));

        public void Save()
        {
            DatabaseContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                DatabaseContext.Dispose();
            }
        }
    }
}

