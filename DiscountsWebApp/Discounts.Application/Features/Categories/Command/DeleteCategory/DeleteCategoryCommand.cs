// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.DeleteCategory
{
    [ApplicationAuthorize(Role = Roles.Admin)]
    public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;
}
