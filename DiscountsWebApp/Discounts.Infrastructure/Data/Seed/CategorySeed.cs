// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastructure.Data.Seed
{
    public static class CategorySeed
    {
        public static async Task SeedCategoriesAsync(IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            var existingNames = unitOfWork.Categories.Query().Select(c => c.Name).ToHashSet();

            var categories = new List<Category>()
            {
                new(){ Name = "Sports" },
                new(){ Name = "Cinema & Theatre" },
                new(){ Name = "Music" },
                new(){ Name = "Art" },
                new(){ Name = "Travel" },
                new(){ Name = "Education" },
                new(){ Name = "Technology" },
                new(){ Name = "Groceries" },
                new(){ Name = "Clothing" },
                new(){ Name = "Other" }
            };

            var newCategories = categories.Where(c => !existingNames.Contains(c.Name));

            if (newCategories.Any())
            {
                foreach (var category in newCategories)
                {
                    await unitOfWork.Categories.AddAsync(category).ConfigureAwait(false);
                }
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
