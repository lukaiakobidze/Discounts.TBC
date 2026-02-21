// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using Discounts.Application.Features.Admin.Command.UpdateGlobalSettings;
using Discounts.Application.Features.Admin.Query.GetGlobalSettings;
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

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var dtos = await _sender.Send(new GetGlobalSettingsQuery(), cancellationToken).ConfigureAwait(false);
            return View(dtos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Dictionary<string, string> settings, CancellationToken cancellationToken)
        {
            foreach (var (key, value) in settings)
            {
                await _sender.Send(new UpdateGlobalSettingsCommand(key, value), cancellationToken).ConfigureAwait(false);
            }

            TempData["Success"] = "Settings updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
