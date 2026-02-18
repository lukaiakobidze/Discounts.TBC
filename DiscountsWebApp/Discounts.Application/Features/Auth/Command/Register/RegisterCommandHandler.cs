// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Interfaces.Auth;
using MediatR;

namespace Discounts.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IIdentityService _identityService;
        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.Role).ConfigureAwait(false);
        }
    }
}
