// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Offers.Query.GetByMerchantId
{
    public class GetByMerchantIdQueryHandler : IRequestHandler<GetByMerchantIdQuery, IReadOnlyList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public GetByMerchantIdQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IReadOnlyList<OfferDto>> Handle(GetByMerchantIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.MerchantOffers}{request.MerchantId}";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<OfferDto>? cached) && cached != null)
                return cached;

            var offers = await _unitOfWork.Offers.GetByMerchantIdAsync(request.MerchantId, cancellationToken).ConfigureAwait(false);

            var dtos = offers.Select(x =>
            {
                var dto = x.Adapt<OfferDto>();
                dto.CategoryName = x.Category.Name;
                return dto;
            }).OrderByDescending(x => x.CreatedAt).ToList();

            _cache.Set(cacheKey, (IReadOnlyList<OfferDto>)dtos, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return dtos;
        }
    }
}
