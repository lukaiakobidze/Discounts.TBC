// Copyright (C) TBC Bank. All Rights Reserved.

using MediatR;

namespace Discounts.Application.Features.Coupons.Command.UseCoupon
{
    public record UseCouponCommand(string Code) : IRequest<Unit>;
}
