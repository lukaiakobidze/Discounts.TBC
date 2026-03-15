// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Offers.Query.GetOfferById
{
    public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, OfferDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public GetOfferByIdQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<OfferDto> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.OfferById}{request.Id}";

            if (_cache.TryGetValue(cacheKey, out OfferDto? cached) && cached != null)
                return cached;

            var offer = await _unitOfWork.Offers.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (offer == null)
            {
                throw new NotFoundException(nameof(offer), request.Id);
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(offer.CategoryId, cancellationToken).ConfigureAwait(false);

            var dto = offer.Adapt<OfferDto>();
            dto.CategoryName = category?.Name ?? string.Empty;

            _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            return dto;
        }
    }
}
