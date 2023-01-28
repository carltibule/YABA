using System.Security.Claims;
using System.Security.Principal;
using YABA.Common.Lookups;

namespace YABA.Common.Extensions
{
    public static class UserIdentityExtensions
    {
        public static string GetUserId(this IIdentity identity) => GetCustomClaim(identity, ClaimsLookup.UserId);
        public static string GetAuthProviderId(this IIdentity identity) => GetCustomClaim(identity, ClaimsLookup.AuthProviderId);

        public static string GetCustomClaim(this IIdentity identity, ClaimsLookup claim)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(claim.GetClaimName())?.Value.ToString();
        }
    }
}
