// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.DTOs.Categories;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Categories.Command.UpdateCateogry
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cache;

        public UpdateCategoryCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            category.Name = request.Name;
            category.Description = request.Description;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _cache.Remove(CacheKeys.AllCategories);

            var dto = category.Adapt<CategoryDto>();
            dto.OfferCount = category.Offers.Count;
            return dto;
        }
    }
}
