// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using MediatR;

namespace Discounts.Application.Features.Categories.Command.DeleteCategory
{
    public class DeleteCateogryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCateogryCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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
            return Unit.Value;
        }
    }
}
