// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Category;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.CreateCategory
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
}
