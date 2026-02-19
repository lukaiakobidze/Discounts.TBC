// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Command.BlockUser;
using Discounts.Application.Features.Admin.Command.UnblockUser;
using Discounts.Application.Features.Admin.Query.GetUsers;
using Discounts.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class UserManagementController : Controller
    {
        private readonly ISender _sender;

        public UserManagementController(ISender sender)
        {
            _sender = sender;
        }

        public async Task<IActionResult> Index(string? role = null)
        {
            var users = await _sender.Send(new GetUsersQuery(role)).ConfigureAwait(false);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id)
        {
            await _sender.Send(new BlockUserCommand(id)).ConfigureAwait(false);
            TempData["Success"] = "User blocked.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string id)
        {
            await _sender.Send(new UnblockUserCommand(id)).ConfigureAwait(false);
            TempData["Success"] = "User unblocked.";
            return RedirectToAction(nameof(Index));
        }
    }
}
