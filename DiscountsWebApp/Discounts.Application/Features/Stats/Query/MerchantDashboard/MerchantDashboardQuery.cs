// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Stats;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Stats.Query.MerchantDashboard
{
    [ApplicationAuthorize(Role = Roles.Merchant)]

    public record MerchantDashboardQuery : IRequest<MerchantDashboardDto>;
}
