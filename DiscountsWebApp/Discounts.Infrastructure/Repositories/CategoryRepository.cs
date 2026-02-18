// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DiscountsDbContext dbContext) : base(dbContext) { }

        public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        }
    }
}
