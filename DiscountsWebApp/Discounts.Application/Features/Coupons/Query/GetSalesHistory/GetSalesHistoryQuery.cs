// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Coupon;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Coupons.Query.GetSalesHistory
{
    [ApplicationAuthorize(Role = Roles.Merchant)]
    public record GetSalesHistoryQuery(Guid? OfferId = null) : IRequest<IReadOnlyList<CouponDto>>;
}
