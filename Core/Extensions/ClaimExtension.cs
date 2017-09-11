using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace yamvc.Core.Extensions
{
    public static class ClaimExtension
    {
        public static string GetUserLogin(this IEnumerable<Claim> claims)
        {
            var loginClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            return loginClaim?.Value;
        }
    }
}
