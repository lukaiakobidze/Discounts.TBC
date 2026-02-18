// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Models;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Offers.Query.GetOffers
{
    public class GetOffersQueryHandler : IRequestHandler<GetOffersQuery, PaginatedList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOffersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PaginatedList<OfferDto>> Handle(GetOffersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Offers.Query().Include(x => x.Category).OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var items = await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var dtos = items.Select(x =>
            {
                var dto = x.Adapt<OfferDto>();
                dto.CategoryName = x.Category?.Name ?? string.Empty;
                return dto;
            }).ToList();

            return new PaginatedList<OfferDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
