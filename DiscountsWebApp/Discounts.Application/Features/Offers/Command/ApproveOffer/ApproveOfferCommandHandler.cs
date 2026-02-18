// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Enums;
using MediatR;

namespace Discounts.Application.Features.Offers.Command.ApproveOffer;

public class ApproveOfferCommandHandler : IRequestHandler<ApproveOfferCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveOfferCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ApproveOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(request.OfferId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(nameof(Offers), request.OfferId);

        if (offer.Status != OfferStatus.Pending)
            throw new ConflictException("Only pending offers can be approved.");

        offer.Status = OfferStatus.Active;

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Unit.Value;
    }
}
