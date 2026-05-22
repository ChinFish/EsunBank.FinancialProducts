namespace EsunBank.FinancialProducts.Services;

public sealed class LikeProductInfo
{
    public string UserId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal FeeRate { get; set; }

    public string Account { get; set; } = string.Empty;

    public int PurchaseQuantity { get; set; }
}
