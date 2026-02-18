// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupon;
using MediatR;

namespace Discounts.Application.Features.Coupons.Query.GetMyCoupons
{
    public record GetMyCouponsQuery() : IRequest<IReadOnlyList<CouponDto>>;
}
