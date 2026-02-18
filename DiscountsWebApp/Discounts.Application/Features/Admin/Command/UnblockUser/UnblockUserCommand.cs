// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Admin.Command.UnblockUser
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record UnblockUserCommand(string UserId) : IRequest<Unit>;
}
