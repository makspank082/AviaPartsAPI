namespace AviaPartsAPI.Models.DTOs;

public class PartResponseDto
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStockLevel { get; set; }
    public string StorageLocation { get; set; } = string.Empty;
    public PartStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int QuantityToReorder => Math.Max(0, MinimumStockLevel - StockQuantity);
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
}

