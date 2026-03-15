// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Reviews.Commands.AddReview
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record AddReviewCommand(Guid CouponId, int Stars, string? Comment) : IRequest<Unit>;
}
