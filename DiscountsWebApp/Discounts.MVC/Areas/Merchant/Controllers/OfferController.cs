// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offers;
using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Offers.Command.CreateOffer;
using Discounts.Application.Features.Offers.Command.UpdateOffer;
using Discounts.Application.Features.Offers.Query.GetOfferById;
using Discounts.Application.Interfaces.Auth;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Constants;
using Discounts.MVC.ViewModels;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Areas.Merchant.Controllers
{
    [Area("Merchant")]
    [Authorize(Roles = Roles.Merchant)]
    public class OfferController : Controller
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public OfferController(ISender sender, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _sender = sender;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            // need to add query feature in app layer
            var offers = await _unitOfWork.Offers.GetByMerchantId(_currentUser.UserId!).ConfigureAwait(false);
            var dtos = offers.Adapt<IReadOnlyList<OfferDto>>();
            return View(dtos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategories().ConfigureAwait(false);
            return View(new OfferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories().ConfigureAwait(false);
                return View(model);
            }

            try
            {
                await _sender.Send(new CreateOfferCommand(
                    model.Name, model.Description, model.ImagePath,
                    model.OriginalPrice, model.DiscountedPrice, model.RemainingCoupons,
                    model.ValidFrom, model.ValidTo, model.CategoryId)).ConfigureAwait(false);

                TempData["Success"] = "Offer created and submitted for approval.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PopulateCategories().ConfigureAwait(false);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var offer = await _sender.Send(new GetOfferByIdQuery(id)).ConfigureAwait(false);

            var vm = offer.Adapt<OfferViewModel>();
            vm.RemainingCoupons = offer.RemainingCount;

            await PopulateCategories().ConfigureAwait(false);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories().ConfigureAwait(false);
                return View(model);
            }

            try
            {
                await _sender.Send(new UpdateOfferCommand(
                    model.Id, model.Name, model.Description, model.ImagePath,
                    model.OriginalPrice, model.DiscountedPrice, model.RemainingCoupons,
                    model.ValidFrom, model.ValidTo, model.CategoryId)).ConfigureAwait(false);

                TempData["Success"] = "Offer updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PopulateCategories().ConfigureAwait(false);
                return View(model);
            }
        }

        private async Task PopulateCategories()
        {
            var categories = await _sender.Send(new GetAllCategoriesQuery()).ConfigureAwait(false);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
