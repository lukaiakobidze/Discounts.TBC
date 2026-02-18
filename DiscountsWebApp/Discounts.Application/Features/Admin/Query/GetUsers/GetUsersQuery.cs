// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Auth;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.GetUsers
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record GetUsersQuery(string? Role) : IRequest<IReadOnlyList<UserDto>>;
}
