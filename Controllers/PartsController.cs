using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AviaPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;
        private readonly ILogger<PartsController> _logger;

        public PartsController(IPartService partService, ILogger<PartsController> logger)
        {
            _partService = partService;
            _logger = logger;
        }

        // GET: api/parts
        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            try
            {
                var parts = await _partService.GetAllPartsAsync();
                return Ok(parts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка деталей");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // GET: api/parts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartById(int id)
        {
            try
            {
                var part = await _partService.GetPartByIdAsync(id);

                if (part == null)
                {
                    return NotFound($"Деталь с ID {id} не найдена");
                }

                return Ok(part);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении детали с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // POST: api/parts
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] CreatePartDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPart = await _partService.CreatePartAsync(dto);

                return CreatedAtAction(
                    nameof(GetPartById),
                    new { id = createdPart.Id },
                    createdPart
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании детали");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // PUT: api/parts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id, [FromBody] UpdatePartDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedPart = await _partService.UpdatePartAsync(id, dto);

                if (updatedPart == null)
                {
                    return NotFound($"Деталь с ID {id} не найдена");
                }

                return Ok(updatedPart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении детали с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // DELETE: api/parts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            try
            {
                var result = await _partService.DeletePartAsync(id);

                if (!result)
                {
                    return NotFound($"Деталь с ID {id} не найдена");
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении детали с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // GET: api/parts/low-stock
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockParts()
        {
            try
            {
                var lowStockParts = await _partService.GetLowStockPartsAsync();
                return Ok(lowStockParts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении деталей с низким запасом");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        // PATCH: api/parts/{id}/stock - если нужно операции со складом
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockOperationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Получаем деталь
                var part = await _partService.GetPartByIdAsync(id);
                if (part == null)
                {
                    return NotFound($"Деталь с ID {id} не найдена");
                }

                // TODO: Реализовать логику операций со складом
                // Это можно добавить в PartService как отдельный метод
                // Например: UpdateStockAsync(int id, StockOperationDto dto)

                return Ok(new
                {
                    message = "Операция со складом будет реализована",
                    operation = dto.OperationType,
                    quantity = dto.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении запаса детали с ID {id}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}