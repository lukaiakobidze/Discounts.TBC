// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Application.DTOs.Category;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.UpdateCateogry
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<CategoryDto>;
}
