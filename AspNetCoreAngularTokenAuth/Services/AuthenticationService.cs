using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GenerateAuthToken(string email, string secret)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.AddSeconds(15), // Short timeframe to test with
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            "the server key used to sign the JWT token is here, use more than 16 chars")),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public AspNetUserToken GetRefreshTokenByToken(string loginProvider, string name, string token, string device)
        {
            return _unitOfWork.TokenRepository.GetAspNetUserTokenByToken(loginProvider, name, token, device);
        }

        public void AddRefreshToken(string userId, string loginProvider, string name, string token, string device)
        {
            var aspNetUserToken = new AspNetUserToken
            {
                UserId = userId,
                LoginProvider = loginProvider,
                Name = name,
                Value = token,
                Device = device
            };

            _unitOfWork.TokenRepository.CreateAspNetUserToken(aspNetUserToken);
            _unitOfWork.Save();
        }

        public void RemoveRefreshTokenByUser(string userId, string loginProvider, string name, string device)
        {
            var refreshToken = _unitOfWork.TokenRepository.GetAspNetUserTokenByUser(userId, loginProvider, name, device);
            if (refreshToken != null)
            {
                _unitOfWork.TokenRepository.RemoveAspNetUserToken(refreshToken);
                _unitOfWork.Save();
            }
        }

        public void RemoveRefreshToken(string loginProvider, string name, string token, string device)
        {
            var refreshToken = GetRefreshTokenByToken(loginProvider, name, token, device);
            if (refreshToken != null)
            {
                _unitOfWork.TokenRepository.RemoveAspNetUserToken(refreshToken);
                _unitOfWork.Save();
            }
        }
    }
}
