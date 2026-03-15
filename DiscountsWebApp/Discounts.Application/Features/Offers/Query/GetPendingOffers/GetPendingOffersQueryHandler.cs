// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Models;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Offers.Query.GetPendingOffers
{

    public class GetPendingOffersQueryHandler : IRequestHandler<GetPendingOffersQuery, PaginatedList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public GetPendingOffersQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<PaginatedList<OfferDto>> Handle(GetPendingOffersQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.PendingOffers}:p{request.PageNumber}s{request.PageSize}";

            if (_cache.TryGetValue(cacheKey, out PaginatedList<OfferDto>? cached) && cached != null)
                return cached;

            var query = _unitOfWork.Offers.Query()
                .Where(o => o.Status == OfferStatus.Pending)
                .Include(o => o.Category)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var dtos = items.Select(o =>
            {
                var dto = o.Adapt<OfferDto>();
                dto.CategoryName = o.Category?.Name ?? string.Empty;
                return dto;
            }).ToList();

            var result = new PaginatedList<OfferDto>(dtos, totalCount, request.PageNumber, request.PageSize);

            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

            return result;
        }
    }
}
