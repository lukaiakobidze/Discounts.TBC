// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Coupon;
using MediatR;

namespace Discounts.Application.Features.Coupons.Command.PurchaseCoupon
{
    public record PurchaseCouponCommand(Guid OfferId) : IRequest<CouponDto>;
}
