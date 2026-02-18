// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;

namespace Discounts.Application.Interfaces.Auth
{
    public interface IIdentityService
    {
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<AuthResponseDto> RegisterAsync(string email, string password, string firstName, string lastName, string role);
        Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<IReadOnlyList<UserDto>> GetUsersAsync(string? role = null);
        Task BlockUserAsync(string userId);
        Task UnblockUserAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}
