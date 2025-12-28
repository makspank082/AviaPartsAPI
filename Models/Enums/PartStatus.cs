namespace AviaPartsAPI.Models;

public enum PartStatus
{
    InStock,        // В наличии достаточно (StockQuantity >= MinimumStockLevel)
    LowStock,       // Мало осталось (0 < StockQuantity < MinimumStockLevel)
    OutOfStock,     // Нет в наличии (StockQuantity == 0)
    Discontinued,   // Снята с производства (IsActive == false)
    OnOrder         // Заказана у поставщика (ждём поставку)
}