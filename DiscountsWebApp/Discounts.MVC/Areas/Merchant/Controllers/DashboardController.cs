// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Coupons.Query.GetSalesHistory;
using Discounts.Application.Features.Stats.Query.MerchantDashboard;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Merchant.Controllers
{
    [Area("Merchant")]
    [Authorize(Roles = Roles.Merchant)]
    public class DashboardController : Controller
    {
        private readonly ISender _sender;

        public DashboardController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var dashboard = await _sender.Send(new MerchantDashboardQuery(), cancellationToken).ConfigureAwait(false);
            var salesHistory = await _sender.Send(new GetSalesHistoryQuery(), cancellationToken).ConfigureAwait(false);
            ViewBag.SalesHistory = salesHistory;
            return View(dashboard);
        }
    }
}
