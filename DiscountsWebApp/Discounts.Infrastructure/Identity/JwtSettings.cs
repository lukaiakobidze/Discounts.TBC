// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenExpirationHours { get; set; }
        public int RefreshTokenbExpirationHours { get; set; }
        public int CookieExpirationHours { get; set; }
    }
}
