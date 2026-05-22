namespace EsunBank.FinancialProducts.Services;

public sealed class LikeProductIndexResult
{
    public string? SelectedUserId { get; set; }

    public IReadOnlyList<UserOptionResult> Users { get; set; } = [];

    public IReadOnlyList<LikeProductResult> Items { get; set; } = [];
}
