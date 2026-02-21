// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Admin;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.GetGlobalSettings
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record GetGlobalSettingsQuery : IRequest<IReadOnlyList<GlobalSettingDto>>;
}
