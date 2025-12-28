using System.ComponentModel.DataAnnotations;

namespace AviaPartsAPI.Models.DTOs;

public class UpdatePartDto
{
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [RegularExpression(@"^[A-Z]-\d{2}$")]
    public string? StorageLocation { get; set; }

    [Range(1, 1000)]
    public int? MinimumStockLevel { get; set; }

    [Range(1, 10000)]
    public int? ReorderQuantity { get; set; }
}
