// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Reviews;
using MediatR;

namespace Discounts.Application.Features.Reviews.Queries.GetOfferReviews
{
    public record GetOfferReviewsQuery(Guid OfferId) : IRequest<IReadOnlyList<ReviewDto>>;
}
