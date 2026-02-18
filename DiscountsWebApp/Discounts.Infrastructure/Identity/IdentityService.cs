// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false)
                ?? throw new NotFoundException("User", email);

            if (user.IsBlocked)
                throw new ForbiddenAccessException("Your account has been blocked.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new ForbiddenAccessException("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var role = roles.FirstOrDefault() ?? "Customer";

            var token = _tokenService.GenerateToken(user.Id, user.Email!, role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
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
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(result.Errors.Select(e => new FluentValidation.Results.ValidationFailure("", e.Description)));
            }

            await _userManager.AddToRoleAsync(user, role).ConfigureAwait(false);

            var token = _tokenService.GenerateToken(user.Id, user.Email!, role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<IReadOnlyList<UserDto>> GetUsersAsync(string? role = null)
        {
            var users = string.IsNullOrEmpty(role)
                ? await _userManager.Users.ToListAsync().ConfigureAwait(false)
                : (await _userManager.GetUsersInRoleAsync(role).ConfigureAwait(false)).ToList();

            var userDtos = new List<UserDto>();
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
    }
}
