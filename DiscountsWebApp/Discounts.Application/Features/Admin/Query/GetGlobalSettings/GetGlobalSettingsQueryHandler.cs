// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace Discounts.Application.Features.Admin.Query.GetGlobalSettings
{
    public class GetGlobalSettingsQueryHandler : IRequestHandler<GetGlobalSettingsQuery, IReadOnlyList<GlobalSettingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGlobalSettingsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<GlobalSettingDto>> Handle(GetGlobalSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _unitOfWork.GlobalSettings.GetAllAsync(cancellationToken).ConfigureAwait(false);

            var dtos = settings.Adapt<IReadOnlyList<GlobalSettingDto>>();

            return dtos;
        }
    }
}
