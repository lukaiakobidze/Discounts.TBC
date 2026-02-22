// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Auth.Command.Register;
using FluentAssertions;

namespace Discounts.Application.Tests.Validators
{
    public class RegisterCommandValidatorTests
    {
        private readonly RegisterCommandValidator _validator = new();

        [Fact]
        public async Task Validate_ValidCommand_ShouldBeValid()
        {
            var command = new RegisterCommand("test@test.com", "Pass123!", "John", "Doe", "Customer");
            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_InvalidEmail_ShouldBeInvalid()
        {
            var command = new RegisterCommand("notanemail", "Pass123!", "John", "Doe", "Customer");
            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validate_ShortPassword_ShouldBeInvalid()
        {
            var command = new RegisterCommand("test@test.com", "12345", "John", "Doe", "Customer");
            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validate_InvalidRole_ShouldBeInvalid()
        {
            var command = new RegisterCommand("test@test.com", "Pass123!", "John", "Doe", "SuperAdmin");
            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
        }
    }
}
