// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Admin.Command.BlockUser;
using Discounts.Application.Features.Admin.Command.UnblockUser;
using Discounts.Application.Features.Admin.Command.UpdateGlobalSettings;
using Discounts.Application.Features.Admin.Query.AdminDashboard;
using Discounts.Application.Features.Admin.Query.GetUsers;
using Discounts.Application.Features.Offers.Command.ApproveOffer;
using Discounts.Application.Features.Offers.Command.RejectOffer;
using Discounts.Application.Features.Offers.Query.GetPendingOffers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _sender.Send(new AdminDashboardQuery()).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("pending-offers")]
        public async Task<IActionResult> GetPendingOffers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _sender.Send(new GetPendingOffersQuery(pageNumber, pageSize)).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("offers/{id:guid}/approve")]
        public async Task<IActionResult> ApproveOffer(Guid id)
        {
            await _sender.Send(new ApproveOfferCommand(id)).ConfigureAwait(false);
            return NoContent();
        }

        [HttpPost("offers/{id:guid}/reject")]
        public async Task<IActionResult> RejectOffer(Guid id)
        {
            await _sender.Send(new RejectOfferCommand(id)).ConfigureAwait(false);
            return NoContent();
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? role)
        {
            var result = await _sender.Send(new GetUsersQuery(role)).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("users/{userId}/block")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            await _sender.Send(new BlockUserCommand(userId)).ConfigureAwait(false);
            return NoContent();
        }

        [HttpPost("users/{userId}/unblock")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            await _sender.Send(new UnblockUserCommand(userId)).ConfigureAwait(false);
            return NoContent();
        }

        [HttpPost("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateGlobalSettingsCommand command)
        {
            await _sender.Send(command).ConfigureAwait(false);
            return NoContent();
        }
    }
}
