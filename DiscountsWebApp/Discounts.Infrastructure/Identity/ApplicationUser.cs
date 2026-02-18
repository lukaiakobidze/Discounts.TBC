// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.AspNetCore.Identity;

namespace Discounts.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
    }
}
