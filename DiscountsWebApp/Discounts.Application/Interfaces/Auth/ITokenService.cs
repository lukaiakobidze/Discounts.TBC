// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string email, string role);
        string GenerateRefreshToken();
    }
}
