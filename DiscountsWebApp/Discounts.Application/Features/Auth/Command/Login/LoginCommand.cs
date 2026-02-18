// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using MediatR;

namespace Discounts.Application.Features.Auth.Command.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
}
