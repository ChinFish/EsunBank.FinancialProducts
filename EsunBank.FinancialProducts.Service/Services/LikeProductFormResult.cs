namespace EsunBank.FinancialProducts.Service.Services;

public sealed class LikeProductFormResult
{
    public int? Sn { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal FeeRate { get; set; }

    public string Account { get; set; } = string.Empty;

    public int PurchaseQuantity { get; set; } = 1;

    public IReadOnlyList<UserOptionResult> Users { get; set; } = [];
}
