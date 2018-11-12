using Data.Repositories;

namespace Data
{
    public interface IUnitOfWork
    {
        DatabaseContext DatabaseContext { get; }

        TokenRepository TokenRepository { get; }

        void Save();

        void Dispose();
    }
}
