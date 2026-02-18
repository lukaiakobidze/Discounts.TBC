// Copyright (C) TBC Bank. All Rights Reserved.

using FluentValidation;

namespace Discounts.Application.Features.Auth.Command.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
