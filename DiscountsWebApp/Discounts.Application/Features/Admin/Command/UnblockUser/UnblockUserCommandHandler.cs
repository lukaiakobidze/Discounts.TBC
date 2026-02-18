// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Auth;
using MediatR;

namespace Discounts.Application.Features.Admin.Command.UnblockUser
{
    public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public UnblockUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Unit> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            await _identityService.UnblockUserAsync(request.UserId).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
