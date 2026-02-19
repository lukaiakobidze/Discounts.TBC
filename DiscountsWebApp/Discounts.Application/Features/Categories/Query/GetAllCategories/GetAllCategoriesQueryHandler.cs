// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Categories;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Application.Features.Categories.Query.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.Query().OrderBy(x => x.Name).ToListAsync(cancellationToken).ConfigureAwait(false);

            var dtos = categories.Select(x =>
            {
                var dto = x.Adapt<CategoryDto>();
                dto.OfferCount = x.Offers.Count;
                return dto;
            }).ToList();

            return dtos;
        }
    }
}
