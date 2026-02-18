// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupon;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Coupons.Query.GetSalesHistory
{
    public class GetSalesHistoryQueryHandler : IRequestHandler<GetSalesHistoryQuery, IReadOnlyList<CouponDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetSalesHistoryQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyList<CouponDto>> Handle(GetSalesHistoryQuery request, CancellationToken cancellationToken)
        {
            var merchantOffers = await _unitOfWork.Offers.GetByMerchantId(_currentUserService.UserId!, cancellationToken).ConfigureAwait(false);
            var offerIds = merchantOffers.Select(o => o.Id).ToHashSet();

            var baseQuery = _unitOfWork.Coupons.Query()
                .Where(c => offerIds.Contains(c.OfferId))
                .AsQueryable();

            if (request.OfferId.HasValue)
                baseQuery = baseQuery.Where(c => c.OfferId == request.OfferId.Value);

            var coupons = await baseQuery.OrderByDescending(c => c.PurchaseDate).ToListAsync(cancellationToken).ConfigureAwait(false);

            return coupons.Select(c =>
            {
                var dto = c.Adapt<CouponDto>();
                dto.OfferName = c.Offer.Name;
                return dto;
            }).ToList();
        }
    }
}
