// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using MediatR;

namespace Discounts.Application.Features.Auth.Command.RefreshToken
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponseDto>;
}
