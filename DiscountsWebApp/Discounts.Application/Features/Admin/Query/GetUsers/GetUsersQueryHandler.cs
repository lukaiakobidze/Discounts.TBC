// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Interfaces.Auth;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
    {
        private readonly IIdentityService _identityService;

        public GetUsersQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.GetUsersAsync(request.Role).ConfigureAwait(false);
        }
    }
}
