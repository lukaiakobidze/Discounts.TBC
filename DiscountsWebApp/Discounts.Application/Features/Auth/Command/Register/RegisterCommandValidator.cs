// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Constants;
using FluentValidation;

namespace Discounts.Application.Features.Auth.Command.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(r => r.Equals(Roles.Merchant, StringComparison.CurrentCultureIgnoreCase) || r.Equals(Roles.Customer, StringComparison.CurrentCultureIgnoreCase))
                .WithMessage("Role must be either 'Merchant' or 'Customer'.");
        }
    }
}
