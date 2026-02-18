// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Coupon;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Coupons.Query.GetMyCoupons
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record GetMyCouponsQuery() : IRequest<IReadOnlyList<CouponDto>>;
}
