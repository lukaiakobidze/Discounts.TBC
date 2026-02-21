// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Infrastructure.Data.Seed
{
    public static class GlobalSettingsSeed
    {
        public static async Task SeedGlobalSettingsAsync(IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            var settings = await unitOfWork.GlobalSettings.GetAllAsync().ConfigureAwait(false);

            var names = settings.Select(x => x.Key);

            var changed = false;

            if (!names.Contains(GlobalSettingConstants.ReservationDurationMinutes))
            {
                await unitOfWork.GlobalSettings.AddAsync(new GlobalSetting() { Key = GlobalSettingConstants.ReservationDurationMinutes, Value = "5" }).ConfigureAwait(false);
                changed = true;
            }
            if (!names.Contains(GlobalSettingConstants.MerchantEditWindowHours))
            {
                await unitOfWork.GlobalSettings.AddAsync(new GlobalSetting() { Key = GlobalSettingConstants.MerchantEditWindowHours, Value = "12" }).ConfigureAwait(false);
                changed = true;
            }

            if (changed)
            {
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
