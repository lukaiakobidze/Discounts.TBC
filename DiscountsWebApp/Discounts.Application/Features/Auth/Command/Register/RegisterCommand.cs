// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using MediatR;

namespace Discounts.Application.Features.Auth.Command.Register
{
    public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Role) : IRequest<AuthResponseDto>;
}
