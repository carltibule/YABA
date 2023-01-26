using YABA.Common.Attributes;

namespace YABA.Common.Lookups
{
    public enum ClaimsLookup
    {
        [ClaimNameAttribute("https://dev.iwanaga.moe/api/auth_provider_id")]
        AuthProviderId = 1,

        [ClaimNameAttribute("https://dev.iwanaga.moe/api/email_address")]
        UserEmail = 2,

        [ClaimNameAttribute("https://dev.iwanaga.moe/api/email_verified")]
        IsEmailConfirmed = 3,

        [ClaimNameAttribute("https://dev.iwanaga.moe/api/username")]
        Username = 4,

        [ClaimNameAttribute("https://dev.iwanaga.moe/api/id")]
        UserId = 5
    }
}
