// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Categories;
using MediatR;

namespace Discounts.Application.Features.Categories.Query.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
}
