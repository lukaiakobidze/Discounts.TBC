// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Offers.Query.GetByMerchantId
{
    public class GetByMerchantIdQueryHandler : IRequestHandler<GetByMerchantIdQuery, IReadOnlyList<OfferDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByMerchantIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<OfferDto>> Handle(GetByMerchantIdQuery request, CancellationToken cancellationToken)
        {
            var offers = await _unitOfWork.Offers.GetByMerchantIdAsync(request.MerchantId, cancellationToken).ConfigureAwait(false);

            var dtos = offers.Select(x =>
            {
                var dto = x.Adapt<OfferDto>();
                dto.CategoryName = x.Category.Name;
                return dto;
            }).OrderByDescending(x => x.CreatedAt).ToList();

            return dtos;
        }
    }
}
