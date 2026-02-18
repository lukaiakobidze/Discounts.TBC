// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Models;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Offers.Query.SearchOffers
{
    public class SearchOffersQueryHandler : IRequestHandler<SearchOffersQuery, PaginatedList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchOffersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<OfferDto>> Handle(SearchOffersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Offers.Query()
                .Where(o => o.Status == OfferStatus.Active)
                .Include(o => o.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(o => o.Name.Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase) || (o.Description != null && o.Description.Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase)));
            }

            if (request.CategoryId.HasValue)
                query = query.Where(o => o.CategoryId == request.CategoryId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(o => o.DiscountedPrice >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(o => o.DiscountedPrice <= request.MaxPrice.Value);

            var ordered = query.OrderByDescending(o => o.CreatedAt);

            var totalCount = await ordered.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var dtos = items.Select(o =>
            {
                var dto = o.Adapt<OfferDto>();
                dto.CategoryName = o.Category?.Name ?? string.Empty;
                return dto;
            }).ToList();

            return new PaginatedList<OfferDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
