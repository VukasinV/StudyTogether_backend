using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using StudyTogether_backend.Models;
using System.Configuration;

namespace StudyTogether_backend.Filters
{
    public class JwtManager
    {
        // Use the below code to generate symmetric Secret Key
        //     var hmac = new HMACSHA256();
        //     var key = Convert.ToBase64String(hmac.Key);

        private static string Secret = ConfigurationManager.AppSettings["HashKey"];

        public static string GenerateToken(int userId, int expireMonths = 3, string role = "user")
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.Now;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),

                Expires = now.AddMonths(expireMonths),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static string GenerateAuthToken(int sid, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.Now;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, sid.ToString()),
                }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static int generateConfirmationSid()
        {
            Random r = new Random();

            int sid = r.Next(1000000);

            return sid;
        }

        public static ClaimsPrincipal GetPrincipal(string token, bool requireExpiration = false)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validatorsParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = requireExpiration,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validatorsParameters, out SecurityToken securityToken);

                return principal;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int getUserId(string token)
        {
            int userId;

            try
            {
                var principal = GetPrincipal(token);

                var claims = principal.Identity as ClaimsIdentity;

                var id = claims.FindFirst(ClaimTypes.NameIdentifier).Value;

                userId = Convert.ToInt32(id);

            }
            catch
            {
                throw;
            }

            return userId;
        }

        public static int getUserConformationCode(string token)
        {
            int conformationCode;

            try
            {
                var principal = GetPrincipal(token, true);

                if (principal == null)
                {
                    return -1;
                }

                var claims = principal.Identity as ClaimsIdentity;

                var sid = claims.FindFirst(ClaimTypes.Sid).Value;

                conformationCode = Convert.ToInt32(sid);
            }
            catch
            {
                throw;
            }

            return conformationCode;
        }
    }
}