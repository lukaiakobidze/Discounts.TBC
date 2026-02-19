// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Offers.Command.ApproveOffer;
using Discounts.Application.Features.Offers.Command.RejectOffer;
using Discounts.Application.Features.Offers.Query.GetPendingOffers;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ModerationController : Controller
    {
        private readonly ISender _sender;

        public ModerationController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var result = await _sender.Send(new GetPendingOffersQuery(pageNumber, 20)).ConfigureAwait(false);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _sender.Send(new ApproveOfferCommand(id)).ConfigureAwait(false);
            TempData["Success"] = "Offer approved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _sender.Send(new RejectOfferCommand(id)).ConfigureAwait(false);
            TempData["Success"] = "Offer rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}
