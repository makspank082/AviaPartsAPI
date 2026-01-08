using AviaPartsAPI.Data;
using AviaPartsAPI.Models;
using AviaPartsAPI.Models.DTOs;
using AviaPartsAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AviaPartsAPI.Services.Commands
{
    public class PartCommandService : IPartCommandService
    {
        private readonly AppDbContext _context;

        public PartCommandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PartResponseDto> CreatePartAsync(
            CreatePartDto partDto,
            CancellationToken cancellationToken = default)
        {
            var exists = await _context.Parts
                .AnyAsync(p => p.SerialNumber == partDto.SerialNumber, cancellationToken);

            if (exists)
                throw new ArgumentException($"Part with serial number {partDto.SerialNumber} already exists");

            var part = new Part
            {
                SerialNumber = partDto.SerialNumber,
                Name = partDto.Name,
                Description = partDto.Description,
                StockQuantity = partDto.InitialQuantity, 
                MinimumStockLevel = partDto.MinimumStockLevel,
                ReorderQuantity = 50, 
                StorageLocation = partDto.StorageLocation,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Status = PartStatus.InStock
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(part);
        }

        public async Task UpdatePartAsync(
            int id,
            UpdatePartDto partDto,
            CancellationToken cancellationToken = default)
        {
            var part = await _context.Parts.FindAsync(new object[] { id }, cancellationToken);

            if (part == null)
                throw new ArgumentException($"Part with ID {id} not found");

            if (!string.IsNullOrEmpty(partDto.Name))
                part.Name = partDto.Name;

            if (!string.IsNullOrEmpty(partDto.Description))
                part.Description = partDto.Description;

            if (!string.IsNullOrEmpty(partDto.StorageLocation))
                part.StorageLocation = partDto.StorageLocation;

            if (partDto.MinimumStockLevel.HasValue)
                part.MinimumStockLevel = partDto.MinimumStockLevel.Value;

            if (partDto.ReorderQuantity.HasValue)
                part.ReorderQuantity = partDto.ReorderQuantity.Value;

            part.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePartAsync(int id, CancellationToken cancellationToken = default)
        {
            var part = await _context.Parts.FindAsync(new object[] { id }, cancellationToken);

            if (part == null)
                throw new ArgumentException($"Part with ID {id} not found");

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PartResponseDto> UpdateStockAsync(
            int id,
            StockOperationDto stockDto,
            CancellationToken cancellationToken = default)
        {
            var part = await _context.Parts.FindAsync(new object[] { id }, cancellationToken);

            if (part == null)
                throw new ArgumentException($"Part with ID {id} not found");

            switch (stockDto.OperationType)
            {
                case StockOperationType.Withdrawal:
                    part.StockQuantity -= stockDto.Quantity;
                    break;
                case StockOperationType.Replenishment:
                    part.StockQuantity += stockDto.Quantity;
                    break;
                case StockOperationType.Adjustment:
                    part.StockQuantity = stockDto.Quantity;
                    part.LastStockTakeDate = DateTime.UtcNow;
                    break;
                case StockOperationType.Transfer:
                    part.StockQuantity -= stockDto.Quantity;
                    break;
            }

            if (part.StockQuantity <= 0)
                part.Status = PartStatus.OutOfStock;
            else if (part.StockQuantity < part.MinimumStockLevel)
                part.Status = PartStatus.LowStock;
            else
                part.Status = PartStatus.InStock;

            part.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(part);
        }

        private static PartResponseDto MapToDto(Part part)
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
    }
}