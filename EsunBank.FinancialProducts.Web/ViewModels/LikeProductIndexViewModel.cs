namespace EsunBank.FinancialProducts.ViewModels;

public sealed class LikeProductIndexViewModel
{
    public string? SelectedUserId { get; set; }

    public IReadOnlyList<UserOptionViewModel> Users { get; set; } = [];

    public IReadOnlyList<LikeProductListItemViewModel> Items { get; set; } = [];
}
