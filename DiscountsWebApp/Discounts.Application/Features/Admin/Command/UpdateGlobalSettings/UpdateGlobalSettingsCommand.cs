// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Admin.Command.UpdateGlobalSettings
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record UpdateGlobalSettingsCommand(string Key, string Value) : IRequest<Unit>;
}
