// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Repositories
{
    public class GlobalSettingRepository : GenericRepository<GlobalSetting>, IGlobalSettingRepository
    {
        public GlobalSettingRepository(DiscountsDbContext dbContext) : base(dbContext) { }

        public Task<GlobalSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
        }
        public async Task<int> GetIntValueAsync(string key, int defaultValue, CancellationToken cancellationToken = default)
        {
            var setting = await GetByKeyAsync(key, cancellationToken).ConfigureAwait(false);

            if (setting == null || !int.TryParse(setting.Value, out var result))
                return defaultValue;
            else
                return result;
        }
    }
}
