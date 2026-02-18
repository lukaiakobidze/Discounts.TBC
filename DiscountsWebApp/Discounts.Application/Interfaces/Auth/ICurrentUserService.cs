// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.Interfaces.Auth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
