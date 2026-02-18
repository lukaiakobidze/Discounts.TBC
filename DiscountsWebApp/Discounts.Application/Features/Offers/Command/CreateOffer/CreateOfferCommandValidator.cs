// Copyright (C) TBC Bank. All Rights Reserved.

using FluentValidation;

namespace Discounts.Application.Features.Offers.Command.CreateOffer
{
    public class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
    {
        public CreateOfferCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.ImagePath)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(0).WithMessage("Original price must be greater than zero.");

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0).WithMessage("Discounted price must be greater than zero.")
                .LessThan(x => x.OriginalPrice).WithMessage("Discounted price must be less than original price.");

            RuleFor(x => x.TotalCount)
                .GreaterThan(0).WithMessage("Coupons quantity must be at least 1.");

            RuleFor(x => x.ValidFrom)
                .NotEmpty().WithMessage("Valid from date is required.");

            RuleFor(x => x.ValidTo)
                .NotEmpty().WithMessage("Valid to date is required.")
                .GreaterThan(x => x.ValidFrom).WithMessage("Valid to date must be after valid from date.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Categories is required.");
        }
    }
}
