namespace EsunBank.FinancialProducts.Models;

public sealed class LikeProduct
{
    public int Sn { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int ProductNo { get; set; }

    public int PurchaseQuantity { get; set; }

    public string Account { get; set; } = string.Empty;

    public decimal TotalFee { get; set; }

    public decimal TotalAmount { get; set; }
}
