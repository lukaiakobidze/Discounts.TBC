// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Attributes;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Features.Favourites.Commands.RemoveFavourite
{
    [ApplicationAuthorize(Role = Roles.Customer)]
    public record RemoveFavouriteCommand(Guid OfferId) : IRequest;
}
