// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IGlobalSettingRepository : IRepository<GlobalSetting>
    {
        Task<GlobalSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
        Task<int> GetIntValueAsync(string key, int defaultValue, CancellationToken cancellationToken = default);
    }
}
