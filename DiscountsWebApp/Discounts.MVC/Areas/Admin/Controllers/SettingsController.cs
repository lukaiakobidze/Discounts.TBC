// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.Features.Admin.Command.UpdateGlobalSettings;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class SettingsController : Controller
    {
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public SettingsController(ISender sender, IUnitOfWork unitOfWork)
        {
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _unitOfWork.GlobalSettings.GetAllAsync().ConfigureAwait(false);
            var dtos = settings.Select(s => new GlobalSettingDto { Id = s.Id, Key = s.Key, Value = s.Value }).ToList();

            if (!dtos.Any(d => d.Key == GlobalSettingConstants.ReservationDurationMinutes))
                dtos.Add(new GlobalSettingDto { Key = GlobalSettingConstants.ReservationDurationMinutes, Value = "30" });
            if (!dtos.Any(d => d.Key == GlobalSettingConstants.MerchantEditWindowHours))
                dtos.Add(new GlobalSettingDto { Key = GlobalSettingConstants.MerchantEditWindowHours, Value = "24" });

            return View((IReadOnlyList<GlobalSettingDto>)dtos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Dictionary<string, string> settings)
        {
            foreach (var (key, value) in settings)
            {
                await _sender.Send(new UpdateGlobalSettingsCommand(key, value)).ConfigureAwait(false);
            }

            TempData["Success"] = "Settings updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
