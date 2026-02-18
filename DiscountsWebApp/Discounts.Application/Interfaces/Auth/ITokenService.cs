// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;

namespace Discounts.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
