// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupons;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Coupons.Query.GetMyCoupons
{
    public class GetMyCouponsQueryHandler : IRequestHandler<GetMyCouponsQuery, IReadOnlyList<CouponDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetMyCouponsQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<CouponDto>> Handle(GetMyCouponsQuery request, CancellationToken cancellationToken)
        {
            var coupons = await _unitOfWork.Coupons.GetByCustomerIdAsync(_currentUserService.UserId!, cancellationToken).ConfigureAwait(false);

            var dtos = coupons.Select(x =>
            {
                var dto = x.Adapt<CouponDto>();
                dto.OfferName = x.Offer.Name;
                return dto;
            }).ToList();

            return dtos;
        }
    }
}
