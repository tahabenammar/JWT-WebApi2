using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace JWTAuthentication.API.Infrastructure
{
    public class ExtendedClaimsProvider
    {
        /// <summary>
        /// Add FULLTIMEEMPLOYEE claim to user identity if he work forthe company for more than 90 days
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>();

            if(DateTime.Now.Date.Subtract(user.JoinDate.Date).TotalDays >= 90)
            {
                claims.Add(CreateClaim("FTE", "1"));
            }
            else
            {
                claims.Add(CreateClaim("FTE", "0"));
            }
            return claims;
        }

        internal static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }
    }
}