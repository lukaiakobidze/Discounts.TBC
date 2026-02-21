// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Reservations.Command.CancelReservation;
using Discounts.Application.Features.Reservations.Command.CreateReservation;
using Discounts.Application.Features.Reservations.Query.GetMyReservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "CustomerOnly")]
    public class ReservationsController : ControllerBase
    {
        private readonly ISender _sender;

        public ReservationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new CancelReservationCommand(id), cancellationToken).ConfigureAwait(false);
            return NoContent();
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetMyReservationsQuery(), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
