using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Function.Providers
{
    public class JwtProvider : IJwtProvider
    {
        private readonly ILogger _logger;

        public JwtProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JwtProvider>();
        }

        public string GenerateToken(string authorizationKey)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("userID", Guid.NewGuid().ToString()));

            var tokeOptions = new JwtSecurityToken(
                issuer: "https://auth.function/issuer",
                audience: "https://auth.function/audience",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        public (bool, Dictionary<string, string>) ValidateToken(string token, string authorizationKey)
        {
            token = token.Replace("Bearer ", string.Empty);

            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizationKey));

            Dictionary<string, string> claims = new Dictionary<string, string>();
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters();
                validationParameters.RequireExpirationTime = true;
                validationParameters.ValidateLifetime = true;
                validationParameters.RequireSignedTokens = true;
                validationParameters.ValidateAudience = true;
                validationParameters.ValidAudience = "https://auth.function/audience";
                validationParameters.ValidateIssuer = true;
                validationParameters.ValidIssuer = "https://auth.function/issuer";
                validationParameters.ValidateIssuerSigningKey = true;
                validationParameters.IssuerSigningKey = mySecurityKey;

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken).Claims;

                foreach (Claim claim in claimsPrincipal)
                    claims.Add(claim.Type, claim.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (false, null);
            }
            return (true, claims);
        }
    }
}