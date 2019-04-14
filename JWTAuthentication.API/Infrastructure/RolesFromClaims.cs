using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace JWTAuthentication.API.Infrastructure
{
    /// <summary>
    /// Add roles on the fly based on som claims on the user identity
    /// </summary>
    public class RolesFromClaims
    {
        public static IEnumerable<Claim> CreateRolesBasedOnClaims(ClaimsIdentity identity)
        {
            List<Claim> claims = new List<Claim>();
            if(identity.HasClaim(c=>string.Equals(c.Value, "FTE"))
                && identity.HasClaim(ClaimTypes.Role, "Admin")
                )
            {
                claims.Add(new Claim(ClaimTypes.Role, "IncidentResolvers"));
            }
            return claims;
        }
    }
}