// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Categories.Command.CreateCategory;
using Discounts.Application.Features.Categories.Command.DeleteCategory;
using Discounts.Application.Features.Categories.Command.UpdateCateogry;
using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Categories.Query.GetCategoryById;
using Discounts.Domain.Constants;
using Discounts.MVC.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken).ConfigureAwait(false);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            await _sender.Send(new CreateCategoryCommand(model.Name, model.Description), cancellationToken).ConfigureAwait(false);
            TempData["Success"] = "Category created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var category = await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken).ConfigureAwait(false);
            var vm = new CategoryViewModel { Id = category.Id, Name = category.Name, Description = category.Description };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            await _sender.Send(new UpdateCategoryCommand(model.Id!.Value, model.Name, model.Description), cancellationToken).ConfigureAwait(false);
            TempData["Success"] = "Category updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteCategoryCommand(id), cancellationToken).ConfigureAwait(false);
            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
