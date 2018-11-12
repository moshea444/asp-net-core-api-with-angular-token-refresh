using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Data.Repositories
{
    public class TokenRepository
    {
        private readonly DatabaseContext _databaseContext;

        public TokenRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void CreateAspNetUserToken(AspNetUserToken aspNetUserToken)
        {
            _databaseContext.AspNetUserTokens.Add(aspNetUserToken);
        }

        public AspNetUserToken GetAspNetUserTokenByToken(string loginProvider, string name, string token, string device)
        {
            return _databaseContext.AspNetUserTokens.FirstOrDefault(p =>
                p.LoginProvider == loginProvider && p.Value == token && p.Name == name && p.Device == device);
        }

        public AspNetUserToken GetAspNetUserTokenByUser(string userId, string loginProvider, string name, string device)
        {
            return _databaseContext.AspNetUserTokens.FirstOrDefault(p =>
                p.UserId == userId && p.LoginProvider == loginProvider && p.Name == name && p.Device == device);
        }

        public void RemoveAspNetUserToken(AspNetUserToken aspNetUserToken)
        {
            _databaseContext.AspNetUserTokens.Remove(aspNetUserToken);
            _databaseContext.Set<AspNetUserToken>().Attach(aspNetUserToken);
            _databaseContext.Entry(aspNetUserToken).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }
    }
}
