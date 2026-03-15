// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Auth;
using Discounts.Application.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IMemoryCache _cache;

        public RegisterCommandHandler(IIdentityService identityService, IMemoryCache cache)
        {
            _identityService = identityService;
            _cache = cache;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.Role).ConfigureAwait(false);
            _cache.Remove(CacheKeys.AdminDashboard);
            return result;
        }
    }
}
