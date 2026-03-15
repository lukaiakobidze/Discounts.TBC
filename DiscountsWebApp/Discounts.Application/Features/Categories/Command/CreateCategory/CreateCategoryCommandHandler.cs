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

namespace Discounts.Application.Features.Categories.Command.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cache;

        public CreateCategoryCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Categories.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false) != null)
                throw new ConflictException($"category with name {request.Name} already exists");

            var category = new Category()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await _unitOfWork.Categories.AddAsync(category, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _cache.Remove(CacheKeys.AllCategories);

            var dto = category.Adapt<CategoryDto>();
            dto.OfferCount = 0;
            return dto;
        }
    }
}
