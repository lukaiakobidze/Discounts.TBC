// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Auth;
using MediatR;

namespace Discounts.Application.Features.Admin.Command.BlockUser
{
    public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public BlockUserCommandHandler(IIdentityService identityService) => _identityService = identityService;

        public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            await _identityService.BlockUserAsync(request.UserId).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
