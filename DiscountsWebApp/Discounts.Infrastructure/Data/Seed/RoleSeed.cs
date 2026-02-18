// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastructure.Data.Seed
{
    public class RoleSeed
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { Roles.Customer, Roles.Admin, Roles.Merchant };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                {
                    await roleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
                }
            }
        }
    }
}
