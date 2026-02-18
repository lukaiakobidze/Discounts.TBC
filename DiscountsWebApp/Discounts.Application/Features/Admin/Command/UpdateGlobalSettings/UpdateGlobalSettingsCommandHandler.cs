// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Features.Admin.Command.UpdateGlobalSettings
{
    public class UpdateGlobalSettingsCommandHandler : IRequestHandler<UpdateGlobalSettingsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateGlobalSettingsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateGlobalSettingsCommand request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.GlobalSettings.GetByKeyAsync(request.Key, cancellationToken).ConfigureAwait(false);

            if (setting == null)
            {
                setting = new GlobalSetting
                {
                    Id = Guid.NewGuid(),
                    Key = request.Key,
                    Value = request.Value
                };
                await _unitOfWork.GlobalSettings.AddAsync(setting, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                setting.Value = request.Value;
                _unitOfWork.GlobalSettings.Update(setting);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
