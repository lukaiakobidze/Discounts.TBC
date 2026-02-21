// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Query.AdminDashboard;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class DashboardController : Controller
    {
        private readonly ISender _sender;

        public DashboardController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stats = await _sender.Send(new AdminDashboardQuery(), cancellationToken).ConfigureAwait(false);
            return View(stats);
        }
    }
}
