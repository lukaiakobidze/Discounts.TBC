// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Features.Admin.Command.BlockUser;
using Discounts.Application.Features.Admin.Command.UnblockUser;
using Discounts.Application.Features.Admin.Query.GetUsers;
using Discounts.Application.Models;
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

        public async Task<IActionResult> Index(CancellationToken cancellationToken, string? role = null, int pageNumber = 1)
        {
            const int pageSize = 15;
            var all = await _sender.Send(new GetUsersQuery(role), cancellationToken).ConfigureAwait(false);
            var paged = new PaginatedList<Discounts.Application.DTOs.Auth.UserDto>(
                all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                all.Count, pageNumber, pageSize);
            return View(paged);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id, CancellationToken cancellationToken)
        {
            await _sender.Send(new BlockUserCommand(id), cancellationToken).ConfigureAwait(false);
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
