using AviaPartsAPI.Data;
using AviaPartsAPI.Models;
using AviaPartsAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AviaPartsAPI.Services
{
    public class PartService : IPartService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PartService> _logger;

        public PartService(AppDbContext context, ILogger<PartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<PartResponseDto>> GetAllPartsAsync()
        {
            var parts = await _context.Parts
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id)
                .ToListAsync();

            return parts.Select(MapToDto);
        }

        public async Task<PartResponseDto?> GetPartByIdAsync(int id)
        {
            var part = await _context.Parts
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            return part == null ? null : MapToDto(part);
        }

        public async Task<PartResponseDto> CreatePartAsync(CreatePartDto dto)
        {
            var part = new Part
            {
                Name = dto.Name,
                Description = dto.Description,
                SerialNumber = dto.SerialNumber,
                StockQuantity = dto.InitialQuantity,
                MinimumStockLevel = dto.MinimumStockLevel,
                ReorderQuantity = 50,
                StorageLocation = dto.StorageLocation,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Status = CalculateStatus(dto.InitialQuantity, dto.MinimumStockLevel)
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            return MapToDto(part);
        }

        public async Task<PartResponseDto?> UpdatePartAsync(int id, UpdatePartDto dto)
        {
            var part = await _context.Parts
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (part == null) return null;

            if (dto.Name != null) part.Name = dto.Name;
            if (dto.Description != null) part.Description = dto.Description;
            if (dto.StorageLocation != null) part.StorageLocation = dto.StorageLocation;
            if (dto.MinimumStockLevel.HasValue) part.MinimumStockLevel = dto.MinimumStockLevel.Value;
            if (dto.ReorderQuantity.HasValue) part.ReorderQuantity = dto.ReorderQuantity.Value;

            part.LastUpdatedAt = DateTime.UtcNow;
            part.Status = CalculateStatus(part.StockQuantity, part.MinimumStockLevel);

            await _context.SaveChangesAsync();
            return MapToDto(part);
        }

        public async Task<bool> DeletePartAsync(int id)
        {
            var part = await _context.Parts
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (part == null) return false;

            part.IsActive = false;
            part.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PartResponseDto>> GetLowStockPartsAsync()
        {
            var parts = await _context.Parts
                .Where(p => p.IsActive
                    && p.StockQuantity <= p.MinimumStockLevel
                    && p.Status != PartStatus.OutOfStock)
                .OrderBy(p => p.StockQuantity)
                .ThenByDescending(p => p.LastUpdatedAt ?? DateTime.MinValue)
                .ToListAsync();

            return parts.Select(MapToDto);
        }

        private PartResponseDto MapToDto(Part part)
        {
            return new PartResponseDto
            {
                Id = part.Id,
                SerialNumber = part.SerialNumber,
                Name = part.Name,
                Description = part.Description,
                StockQuantity = part.StockQuantity,
                MinimumStockLevel = part.MinimumStockLevel,
                StorageLocation = part.StorageLocation,
                Status = part.Status,
                CreatedAt = part.CreatedAt,
                SupplierId = part.SupplierId,
                CategoryId = part.CategoryId
            };
        }

        private PartStatus CalculateStatus(int stockQuantity, int minimumStockLevel)
        {
            if (stockQuantity == 0) return PartStatus.OutOfStock;
            if (stockQuantity <= minimumStockLevel) return PartStatus.LowStock;
            return PartStatus.InStock;
        }
    }
}
