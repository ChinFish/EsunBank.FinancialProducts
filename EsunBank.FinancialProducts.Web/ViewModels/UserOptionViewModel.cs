namespace EsunBank.FinancialProducts.ViewModels;

public sealed class UserOptionViewModel
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string DisplayName => $"{UserName} ({UserId})";
}
