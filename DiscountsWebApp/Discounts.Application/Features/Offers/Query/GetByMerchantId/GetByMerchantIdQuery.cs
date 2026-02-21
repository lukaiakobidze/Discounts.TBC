// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetByMerchantId
{
    public record GetByMerchantIdQuery(string MerchantId) : IRequest<IReadOnlyList<OfferDto>>;
}
