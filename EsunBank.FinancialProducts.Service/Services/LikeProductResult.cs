namespace EsunBank.FinancialProducts.Service.Services;

public sealed class LikeProductResult
{
    public int Sn { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DefaultAccount { get; set; } = string.Empty;

    public string DebitAccount { get; set; } = string.Empty;

    public int ProductNo { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal FeeRate { get; set; }

    public int PurchaseQuantity { get; set; }

    public decimal TotalFee { get; set; }

    public decimal TotalAmount { get; set; }
}
