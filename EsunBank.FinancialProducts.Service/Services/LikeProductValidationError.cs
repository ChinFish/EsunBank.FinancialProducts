namespace EsunBank.FinancialProducts.Service.Services;

public sealed class LikeProductValidationError
{
    public required string Field { get; init; }

    public required string Message { get; init; }
}
