// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Coupons.Command.UseCoupon
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record UseCouponCommand(string Code) : IRequest<Unit>;
}
