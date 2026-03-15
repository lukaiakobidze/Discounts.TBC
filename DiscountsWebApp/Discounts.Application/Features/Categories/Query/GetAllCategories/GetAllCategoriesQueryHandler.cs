// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Categories;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Categories.Query.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(CacheKeys.AllCategories, out IReadOnlyList<CategoryDto>? cached) && cached != null)
                return cached;

            var categories = await _unitOfWork.Categories.Query().OrderBy(x => x.Name).ToListAsync(cancellationToken).ConfigureAwait(false);

            var dtos = categories.Select(x =>
            {
                var dto = x.Adapt<CategoryDto>();
                dto.OfferCount = x.Offers.Count(x => x.Status == OfferStatus.Active);
                return dto;
            }).ToList();

            _cache.Set(CacheKeys.AllCategories, (IReadOnlyList<CategoryDto>)dtos, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            });

            return dtos;
        }
    }
}
