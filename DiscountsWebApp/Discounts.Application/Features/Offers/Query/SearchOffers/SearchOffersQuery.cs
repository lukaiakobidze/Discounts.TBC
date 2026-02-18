// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Models;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.SearchOffers
{
    public record SearchOffersQuery(
        string? SearchTerm,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PaginatedList<OfferDto>>;
}
