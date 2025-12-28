using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AviaPartsAPI.Models;
using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Services;

namespace AviaPartsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            _partService = partService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PartResponseDto>> GetAllParts()
        {
            var parts = _partService.GetAllParts();
            return Ok(parts);
        }

        [HttpGet("{id}")]
        public ActionResult<PartResponseDto> GetPart(int id)
        {
            var part = _partService.GetPartById(id);

            if (part == null)
            {
                return NotFound(); 
            }

            return Ok(part); 
        }

        [HttpGet("low-stock(Аллерт. Мало деталей на складе)")]
        public ActionResult<IEnumerable<PartResponseDto>> GetLowStockParts()
        {
            var parts = _partService.GetLowStockParts();
            return Ok(parts);
        }

        [HttpPost]
        public ActionResult<PartResponseDto> CreatePart(CreatePartDto dto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }

            var createdPart = _partService.CreatePart(dto);

            return CreatedAtAction(nameof(GetPart), new { id = createdPart.Id }, createdPart);
        }

        [HttpPut("{id}")]
        public ActionResult<PartResponseDto> UpdatePart(int id, [FromBody] UpdatePartDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updatedPart = _partService.UpdatePart(id, dto);
            if (updatedPart == null) return NotFound();
            return Ok(updatedPart);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePart(int id)
        {
            var isDeleted = _partService.DeletePart(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}