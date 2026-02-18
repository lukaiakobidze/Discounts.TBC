// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Category;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCategoryCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

            var dto = category.Adapt<CategoryDto>();
            dto.OfferCount = 0;
            return dto;
        }
    }
}
