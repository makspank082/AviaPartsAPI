using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace AviaPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartQueryService _queryService;
        private readonly IPartCommandService _commandService;

        public PartsController(
            IPartQueryService queryService,
            IPartCommandService commandService)
        {
            _queryService = queryService;
            _commandService = commandService;
        }

        // GET: api/parts
        [HttpGet]
        public async Task<IActionResult> GetAllParts(CancellationToken cancellationToken)
        {
            var parts = await _queryService.GetAllPartsAsync(cancellationToken);
            return Ok(parts);
        }

        // GET: api/parts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartById(int id, CancellationToken cancellationToken)
        {
            var part = await _queryService.GetPartByIdAsync(id, cancellationToken);

            if (part == null)
            {
                throw new KeyNotFoundException($"Part with id {id} not found.");
            }

            return Ok(part);
        }

        // GET: api/parts/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<PartResponseDto>>> GetPartsPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _queryService.GetPartsPagedAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        // GET: api/parts/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockParts(CancellationToken cancellationToken)
        {
            var lowStockParts = await _queryService.GetLowStockPartsAsync(cancellationToken);
            return Ok(lowStockParts);
        }

        // POST: api/parts
        [HttpPost]
        public async Task<IActionResult> CreatePart(
            [FromBody] CreatePartDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid part data");
            }

            var createdPart = await _commandService.CreatePartAsync(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetPartById),
                new { id = createdPart.Id },
                createdPart
            );
        }

        // PUT: api/parts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(
            int id,
            [FromBody] UpdatePartDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid update data");
            }

            var existingPart = await _queryService.GetPartByIdAsync(id, cancellationToken);
            if (existingPart == null)
            {
                throw new KeyNotFoundException($"Part with id {id} not found.");
            }

            await _commandService.UpdatePartAsync(id, dto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/parts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id, CancellationToken cancellationToken)
        {
            var existingPart = await _queryService.GetPartByIdAsync(id, cancellationToken);
            if (existingPart == null)
            {
                throw new KeyNotFoundException($"Part with id {id} not found.");
            }

            await _commandService.DeletePartAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH: api/parts/{id}/stock
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(
            int id,
            [FromBody] StockOperationDto dto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid stock operation data");
            }

            var part = await _queryService.GetPartByIdAsync(id, cancellationToken);
            if (part == null)
            {
                throw new KeyNotFoundException($"Part with id {id} not found.");
            }

            try
            {
                var updatedPart = await _commandService.UpdateStockAsync(id, dto, cancellationToken);
                return Ok(updatedPart);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        [HttpGet("test-exception")]
        public IActionResult TestException()
        {
            throw new KeyNotFoundException("Test exception from middleware");
        }
    }
}