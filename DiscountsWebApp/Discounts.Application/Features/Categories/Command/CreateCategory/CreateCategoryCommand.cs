// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.CreateCategory
{
    public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
}
