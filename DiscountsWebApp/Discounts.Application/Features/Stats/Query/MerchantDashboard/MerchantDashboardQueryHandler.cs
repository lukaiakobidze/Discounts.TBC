// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Stats;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Stats.Query.MerchantDashboard
{
    public class MerchantDashboardQueryHandler : IRequestHandler<MerchantDashboardQuery, MerchantDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MerchantDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<MerchantDashboardDto> Handle(MerchantDashboardQuery request, CancellationToken cancellationToken)
        {
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
            return dto;
        }
    }
}
