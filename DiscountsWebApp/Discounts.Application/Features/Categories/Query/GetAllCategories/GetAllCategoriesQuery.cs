// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;
using MediatR;

namespace Discounts.Application.Features.Categories.Query.GetAllCategories
{
    public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
}
