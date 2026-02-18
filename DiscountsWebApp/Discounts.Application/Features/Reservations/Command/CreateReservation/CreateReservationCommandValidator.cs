// Copyright (C) TBC Bank. All Rights Reserved.

using FluentValidation;

namespace Discounts.Application.Features.Reservations.Command.CreateReservation
{
    public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationCommandValidator()
        {
            RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required");
        }
    }
}
