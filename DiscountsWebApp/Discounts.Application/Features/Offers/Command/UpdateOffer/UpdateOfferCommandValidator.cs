// Copyright (C) TBC Bank. All Rights Reserved.

using FluentValidation;

namespace Discounts.Application.Features.Offers.Command.UpdateOffer
{
    public class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
    {
        public UpdateOfferCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(0).WithMessage("Original price must be greater than zero.");

            RuleFor(x => x.DiscountedPrice)
                .GreaterThan(0).WithMessage("Discounted price must be greater than zero.")
                .LessThan(x => x.OriginalPrice).WithMessage("Discounted price must be less than original price.");

            RuleFor(x => x.CouponQuantity)
                .GreaterThan(0).WithMessage("Coupons quantity must be at least 1.");

            RuleFor(x => x.ValidTo)
                .GreaterThan(x => x.ValidFrom).WithMessage("Valid to date must be after valid from date.");

            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
