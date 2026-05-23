namespace EsunBank.FinancialProducts.Web.Models;

public sealed class Product
{
    public int No { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal FeeRate { get; set; }
}
