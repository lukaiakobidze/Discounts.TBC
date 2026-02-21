// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Discounts.Application.Features.Categories.Command.CreateCategory;
using Discounts.Application.Features.Categories.Command.DeleteCategory;
using Discounts.Application.Features.Categories.Command.UpdateCateogry;
using Discounts.Application.Features.Categories.Query.GetAllCategories;
using Discounts.Application.Features.Categories.Query.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _sender;

        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch.");

            var result = await _sender.Send(command, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteCategoryCommand(id), cancellationToken).ConfigureAwait(false);
            return NoContent();
        }
    }
}
