// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Offers.Command.CreateOffer;
using Discounts.Application.Features.Offers.Command.UpdateOffer;
using Discounts.Application.Features.Offers.Query.GetByMerchantId;
using Discounts.Application.Features.Offers.Query.GetOfferById;
using Discounts.Application.Interfaces.Auth;
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
        private readonly IWebHostEnvironment _env;

        public OfferController(ISender sender, ICurrentUserService currentUser, IWebHostEnvironment env)
        {
            _sender = sender;
            _currentUser = currentUser;
            _env = env;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var dtos = await _sender.Send(new GetByMerchantIdQuery(_currentUser.UserId!), cancellationToken).ConfigureAwait(false);
            return View(dtos);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PopulateCategories(cancellationToken).ConfigureAwait(false);
            return View(new OfferViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfferViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(cancellationToken).ConfigureAwait(false);
                return View(model);
            }
            try
            {
                model.ValidFrom = DateTime.SpecifyKind(model.ValidFrom.AddMinutes(model.TimezoneOffset), DateTimeKind.Utc);
                model.ValidTo = DateTime.SpecifyKind(model.ValidTo.AddMinutes(model.TimezoneOffset), DateTimeKind.Utc);

                if (model.ImageFile != null)
                    model.ImagePath = await SaveImageAsync(model.ImageFile).ConfigureAwait(false);

                await _sender.Send(new CreateOfferCommand(
                    model.Name, model.Description, model.ImagePath,
                    model.OriginalPrice, model.DiscountedPrice, model.RemainingCoupons,
                    model.ValidFrom, model.ValidTo, model.CategoryId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer created and submitted for approval.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PopulateCategories(cancellationToken).ConfigureAwait(false);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var offer = await _sender.Send(new GetOfferByIdQuery(id), cancellationToken).ConfigureAwait(false);

            var vm = offer.Adapt<OfferViewModel>();
            vm.RemainingCoupons = offer.RemainingCount;

            await PopulateCategories(cancellationToken).ConfigureAwait(false);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OfferViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategories(cancellationToken).ConfigureAwait(false);
                return View(model);
            }
            try
            {
                model.ValidFrom = DateTime.SpecifyKind(model.ValidFrom.AddMinutes(model.TimezoneOffset), DateTimeKind.Utc);
                model.ValidTo = DateTime.SpecifyKind(model.ValidTo.AddMinutes(model.TimezoneOffset), DateTimeKind.Utc);

                if (model.ImageFile != null)
                {
                    DeleteImage(model.ImagePath);
                    model.ImagePath = await SaveImageAsync(model.ImageFile).ConfigureAwait(false);
                }

                await _sender.Send(new UpdateOfferCommand(
                    model.Id, model.Name, model.Description, model.ImagePath,
                    model.OriginalPrice, model.DiscountedPrice, model.RemainingCoupons,
                    model.ValidFrom, model.ValidTo, model.CategoryId), cancellationToken).ConfigureAwait(false);
                TempData["Success"] = "Offer updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await PopulateCategories(cancellationToken).ConfigureAwait(false);
                return View(model);
            }
        }

        private async Task PopulateCategories(CancellationToken cancellationToken)
        {
            var categories = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken).ConfigureAwait(false);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension)) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "offers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream).ConfigureAwait(false);
            }

            return $"/uploads/offers/{uniqueFileName}";
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
