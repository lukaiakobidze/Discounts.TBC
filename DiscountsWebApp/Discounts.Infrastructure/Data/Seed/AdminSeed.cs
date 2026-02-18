// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Constants;
using Discounts.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastructure.Data.Seed
{
    public class AdminSeed
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = "admin@tbc.ge";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);

            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123").ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin).ConfigureAwait(false);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingAdmin, Roles.Admin).ConfigureAwait(false))
                    await userManager.AddToRoleAsync(existingAdmin, Roles.Admin).ConfigureAwait(false);
            }
        }
    }
}
