// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Coupon;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Coupons.Command.PurchaseCoupon
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record PurchaseCouponCommand(Guid OfferId) : IRequest<CouponDto>;
}
