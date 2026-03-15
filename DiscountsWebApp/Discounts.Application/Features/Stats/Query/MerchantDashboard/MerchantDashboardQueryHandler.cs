// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Stats;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Stats.Query.MerchantDashboard
{
    public class MerchantDashboardQueryHandler : IRequestHandler<MerchantDashboardQuery, MerchantDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cache;

        public MerchantDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<MerchantDashboardDto> Handle(MerchantDashboardQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.MerchantDashboard}{_currentUserService.UserId}";

            if (_cache.TryGetValue(cacheKey, out MerchantDashboardDto? cached) && cached != null)
                return cached;

            var offers = await _unitOfWork.Offers.Query().Include(x => x.Coupons)
                .Where(x => x.MerchantId == _currentUserService.UserId).ToListAsync(cancellationToken).ConfigureAwait(false);

            var dto = new MerchantDashboardDto()
            {
                TotalOffers = offers.Count,
                PendingOffers = offers.Count(x => x.Status == OfferStatus.Pending),
                ActiveOffers = offers.Count(x => x.Status == OfferStatus.Active),
                TotalCouponsSold = offers.Sum(x => x.Coupons.Count),
                TotalRevenue = offers.Sum(x => x.Coupons.Count * x.DiscountedPrice)
            };

            _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return dto;
        }
    }
}
