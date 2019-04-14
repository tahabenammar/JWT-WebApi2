using JWTAuthentication.API.Entities;
using JWTAuthentication.API.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace JWTAuthentication.API.Providers
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer = string.Empty;
        private const string ClientPropertyKey = "as:client_id";

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Client client = null;
            string clientId = data.Properties.Dictionary.ContainsKey(ClientPropertyKey) 
                              ? data.Properties.Dictionary[ClientPropertyKey] 
                              : null;

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include as:client_id");

            
            using (AuthRepository _repo = new AuthRepository())
            {
                client = _repo.FindClient(clientId);
            }

            string symmetricKeyAsBase64 = client.Secret;

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;
            //Optional: Map Identity Claims names to JWT names (using jwtClaims instead of 'data.Identity.Claims' in JwtSecurityToken constructor)
            var jwtClaims = new List<Claim>
            {
                new Claim("sub", data.Identity.Name)
            };
            jwtClaims.AddRange(data.Identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => new Claim("roles", c.Value)));

            var token = new JwtSecurityToken(_issuer, clientId, jwtClaims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;

        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}