// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupon;
using MediatR;

namespace Discounts.Application.Features.Coupons.Query.GetSalesHistory
{
    public record GetSalesHistoryQuery(Guid? OfferId = null) : IRequest<IReadOnlyList<CouponDto>>;
}
