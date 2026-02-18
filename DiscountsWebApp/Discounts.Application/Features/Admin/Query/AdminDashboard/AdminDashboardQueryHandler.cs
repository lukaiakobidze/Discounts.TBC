// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Stats;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.Domain.Enums;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.AdminDashboard
{
    public class AdminDashboardQueryHandler : IRequestHandler<AdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AdminDashboardQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<AdminDashboardDto> Handle(AdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var customers = await _identityService.GetUsersAsync(Roles.Customer).ConfigureAwait(false);
            var merchants = await _identityService.GetUsersAsync(Roles.Merchant).ConfigureAwait(false);

            var activeOffers = await _unitOfWork.Offers.CountAsync(x => x.Status == OfferStatus.Active, cancellationToken).ConfigureAwait(false);
            var pendingOffers = await _unitOfWork.Offers.CountAsync(x => x.Status == OfferStatus.Pending, cancellationToken).ConfigureAwait(false);

            var statsDto = new AdminDashboardDto()
            {
                TotalCustomers = customers.Count,
                TotalMerchants = merchants.Count,
                ActiveOffers = activeOffers,
                PendingOffers = pendingOffers
            };

            return statsDto;
        }
    }
}
