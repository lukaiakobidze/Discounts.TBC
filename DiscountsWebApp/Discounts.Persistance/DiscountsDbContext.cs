// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance
{
    public class DiscountsDbContext : IdentityDbContext<DiscountsDbContext>
    {
        public DiscountsDbContext(DbContextOptions options) : base(options)
        {
        }


    }
}
