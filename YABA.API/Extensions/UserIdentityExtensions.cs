using System.Security.Claims;
using System.Security.Principal;
using YABA.Common.Extensions;
using YABA.Common.Lookups;

namespace YABA.API.Extensions
{
    public static class UserIdentityExtensions
    {
        public static string GetAuthProviderId(this IIdentity identity) => GetCustomClaim(identity, ClaimsLookup.AuthProviderId);

        public static string GetCustomClaim(this IIdentity identity, ClaimsLookup claim)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(claim.GetClaimName())?.Value.ToString();
        }
    }
}
