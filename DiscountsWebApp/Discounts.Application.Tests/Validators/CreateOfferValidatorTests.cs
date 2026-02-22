// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Offers.Command.CreateOffer;
using FluentAssertions;

namespace Discounts.Application.Tests.Validators
{
    public class CreateOfferCommandValidatorTests
    {
        private readonly CreateOfferCommandValidator _validator = new();

        [Fact]
        public async Task Validate_ValidCommand_ShouldBeValid()
        {
            var command = new CreateOfferCommand(
                "Valid Title", "Valid Description", null,
                100m, 50m, 10,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30),
                Guid.NewGuid());

            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_EmptyName_ShouldBeInvalid()
        {
            var command = new CreateOfferCommand(
                "", "Description", null,
                100m, 50m, 10,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30),
                Guid.NewGuid());

            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Validate_DiscountedPriceHigherThanOriginal_ShouldBeInvalid()
        {
            var command = new CreateOfferCommand(
                "Title", "Description", null,
                50m, 100m, 10,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30),
                Guid.NewGuid());

            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "DiscountedPrice");
        }

        [Fact]
        public async Task Validate_ZeroCouponQuantity_ShouldBeInvalid()
        {
            var command = new CreateOfferCommand(
                "Title", "Description", null,
                100m, 50m, 0,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30),
                Guid.NewGuid());

            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validate_ValidToBeforeValidFrom_ShouldBeInvalid()
        {
            var command = new CreateOfferCommand(
                "Title", "Description", null,
                100m, 50m, 10,
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow,
                Guid.NewGuid());

            var result = await _validator.ValidateAsync(command).ConfigureAwait(true);
            result.IsValid.Should().BeFalse();
        }
    }
}
