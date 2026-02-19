// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Categories;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Categories.Query.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            var dto = category.Adapt<CategoryDto>();
            dto.OfferCount = category.Offers.Count;

            return dto;
        }
    }
}
