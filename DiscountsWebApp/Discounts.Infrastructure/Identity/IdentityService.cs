// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Discounts.Application.DTOs.Auth;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly DiscountsDbContext _context;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService, DiscountsDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false)
                ?? throw new NotFoundException("user", email);

            if (user.IsBlocked)
                throw new ForbiddenAccessException("Your account has been blocked.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new ForbiddenAccessException("Invalid email or password.");

            var role = await GetPrimaryRoleAsync(user).ConfigureAwait(false);
            return await CreateAuthResponseAsync(user, role).ConfigureAwait(false);
        }

        public async Task<AuthResponseDto> RegisterAsync(string email, string password, string firstName, string lastName, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (existingUser != null)
                throw new ConflictException($"User with email '{email}' already exists.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);

            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => new FluentValidation.Results.ValidationFailure("", e.Description)));

            await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);

            return await CreateAuthResponseAsync(user, role).ConfigureAwait(false);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken)
                ?? throw new ForbiddenAccessException("Invalid access token.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new ForbiddenAccessException("Invalid access token claims.");

            var storedToken = await _context.RefreshTokens.Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId).ConfigureAwait(false);

            if (storedToken == null || !storedToken.IsActive)
                throw new ForbiddenAccessException("Invalid or expired refresh token.");

            storedToken.IsRevoked = true;

            var role = await GetPrimaryRoleAsync(storedToken.User).ConfigureAwait(false);
            var newAccessToken = _tokenService.GenerateAccessToken(userId, storedToken.User.Email!, role);
            var newRefreshToken = await StoreNewRefreshTokenAsync(userId).ConfigureAwait(false);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = storedToken.User.Email!,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<IReadOnlyList<UserDto>> GetUsersAsync(string? role = null)
        {
            var users = string.IsNullOrEmpty(role)
                ? await _userManager.Users.ToListAsync().ConfigureAwait(false)
                : (await _userManager.GetUsersInRoleAsync(role).ConfigureAwait(false)).ToList();

            var userDtos = new List<UserDto>(users.Count);
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = roles.FirstOrDefault() ?? "Customer",
                    IsBlocked = user.IsBlocked
                });
            }
            return userDtos;
        }

        public async Task BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
                ?? throw new NotFoundException("User", userId);

            user.IsBlocked = true;
            await _userManager.UpdateAsync(user).ConfigureAwait(false);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue).ConfigureAwait(false);
        }

        public async Task UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
                ?? throw new NotFoundException("User", userId);

            user.IsBlocked = false;
            await _userManager.UpdateAsync(user).ConfigureAwait(false);
            await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            return user != null && await _userManager.IsInRoleAsync(user, role).ConfigureAwait(false);
        }

        private async Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user, string role)
        {
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, role);
            var refreshToken = await StoreNewRefreshTokenAsync(user.Id).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        private async Task<string> StoreNewRefreshTokenAsync(string userId)
        {
            var tokenValue = _tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = tokenValue,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _context.RefreshTokens.AddAsync(refreshToken).ConfigureAwait(false);
            return tokenValue;
        }

        private async Task<string> GetPrimaryRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            return roles.FirstOrDefault() ?? "Customer";
        }
    }
}
