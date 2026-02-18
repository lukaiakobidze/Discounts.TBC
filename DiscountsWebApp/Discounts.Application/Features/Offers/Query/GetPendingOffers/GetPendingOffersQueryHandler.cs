// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Models;
using Discounts.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Offers.Query.GetPendingOffers
{

    public class GetPendingOffersQueryHandler : IRequestHandler<GetPendingOffersQuery, PaginatedList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPendingOffersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<OfferDto>> Handle(GetPendingOffersQuery request, CancellationToken cancellationToken)
        {
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

            return new PaginatedList<OfferDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
