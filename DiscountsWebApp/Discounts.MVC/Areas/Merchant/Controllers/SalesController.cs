// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Coupons.Query.GetSalesHistory;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Merchant.Controllers
{
    [Area("Merchant")]
    [Authorize(Roles = Roles.Merchant)]
    public class SalesController : Controller
    {
        private readonly ISender _sender;

        public SalesController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(Guid? offerId = null)
        {
            var sales = await _sender.Send(new GetSalesHistoryQuery(offerId)).ConfigureAwait(false);
            return View(sales);
        }
    }
}
