// Copyright (C) TBC Bank. All Rights Reserved.

using System.Reflection;
using Discounts.Application.Attributes;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using MediatR;

namespace Discounts.Application.Behaviours
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType()
                .GetCustomAttributes<ApplicationAuthorizeAttribute>();

            foreach (var attr in authorizeAttributes)
            {
                if (!_currentUserService.IsAuthenticated)
                    throw new ForbiddenAccessException("User is not authenticated");

                if (_currentUserService.UserId is null)
                    throw new ForbiddenAccessException("NameIdentifier claim is missing from token");

                if (attr.Role != null && !_currentUserService.IsInRole(attr.Role))
                    throw new ForbiddenAccessException($"User doesn't have Role: {attr.Role}");
            }

            return await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
