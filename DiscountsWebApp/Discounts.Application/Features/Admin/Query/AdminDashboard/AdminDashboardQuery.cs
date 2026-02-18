// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Stats;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.AdminDashboard
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record AdminDashboardQuery : IRequest<AdminDashboardDto>;
}
