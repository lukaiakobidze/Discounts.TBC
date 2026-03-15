// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Constants;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Discounts.Application.Features.Categories.Command.DeleteCategory
{
    public class DeleteCateogryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cache;

        public DeleteCateogryCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            if (category.Offers.Any(x => (x.Status == OfferStatus.Active || x.Status == OfferStatus.Pending) && !x.IsDeleted))
                throw new ConflictException("Can not delete category with active or pending offers");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _cache.Remove(CacheKeys.AllCategories);

            return Unit.Value;
        }
    }
}
