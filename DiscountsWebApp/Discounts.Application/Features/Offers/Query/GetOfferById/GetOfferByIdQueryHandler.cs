// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetOfferById
{
    public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, OfferDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOfferByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferDto> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (offer == null)
            {
                throw new NotFoundException(nameof(offer), request.Id);
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(offer.CategoryId, cancellationToken).ConfigureAwait(false);

            var dto = offer.Adapt<OfferDto>();

            dto.CategoryName = category?.Name ?? string.Empty;
            Console.WriteLine($"ValidFrom Kind: {offer.ValidFrom.Kind}");
            return dto;
        }
    }
}
