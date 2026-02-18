// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Discounts.Infrastructure.Data.Context
{
    public class DiscountsDbContextFactory : IDesignTimeDbContextFactory<DiscountsDbContext>
    {
        public DiscountsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscountsDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=TBC_Discounts;Trusted_Connection=True;TrustServerCertificate=True");

            return new DiscountsDbContext(optionsBuilder.Options);
        }
    }
}
