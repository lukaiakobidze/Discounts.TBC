// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.RejectOffer
{
    public class RejectOfferCommandHandler : IRequestHandler<RejectOfferCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectOfferCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RejectOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false)
                ?? throw new NotFoundException(nameof(Offer), request.OfferId);

            if (offer.Status != OfferStatus.Pending)
                throw new ConflictException("Only pending offers can be rejected.");

            offer.Status = OfferStatus.Denied;

            _unitOfWork.Offers.Update(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
