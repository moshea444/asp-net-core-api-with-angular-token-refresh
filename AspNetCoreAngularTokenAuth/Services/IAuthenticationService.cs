using Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public interface IAuthenticationService
    {
        string GenerateAuthToken(string email, string secret);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        AspNetUserToken GetRefreshTokenByToken(string loginProvider, string name, string token, string device);
        void AddRefreshToken(string userId, string loginProvider, string name, string token, string device);
        void RemoveRefreshToken(string loginProvider, string name, string token, string device);
        void RemoveRefreshTokenByUser(string userId, string loginProvider, string name, string device);
    }
}
