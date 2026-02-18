// Copyright (C) TBC Bank. All Rights Reserved.

using FluentValidation;

namespace Discounts.Application.Features.Categories.Command.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category Name is required")
                                .MaximumLength(60).WithMessage("Category name has maximum length of 60");

            RuleFor(x => x.Description).MinimumLength(400).WithMessage("Category description has maximum length of 400");
        }
    }
}
