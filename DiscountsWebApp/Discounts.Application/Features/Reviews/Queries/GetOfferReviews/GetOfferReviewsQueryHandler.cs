// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Reviews;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Reviews.Queries.GetOfferReviews
{
    public class GetOfferReviewsQueryHandler : IRequestHandler<GetOfferReviewsQuery, IReadOnlyList<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public GetOfferReviewsQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IReadOnlyList<ReviewDto>> Handle(GetOfferReviewsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.OfferReviews}{request.OfferId}";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<ReviewDto>? cached) && cached != null)
                return cached;

            var reviews = await _unitOfWork.Reviews.GetByOfferIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false);
            var result = reviews.Select(r => r.Adapt<ReviewDto>()).ToList().AsReadOnly();

            _cache.Set(cacheKey, (IReadOnlyList<ReviewDto>)result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return result;
        }
    }
}
