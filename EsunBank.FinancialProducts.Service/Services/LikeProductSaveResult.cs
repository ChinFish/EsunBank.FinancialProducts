namespace EsunBank.FinancialProducts.Service.Services;

public sealed class LikeProductSaveResult
{
    private LikeProductSaveResult(bool isSuccess, int? sn, IReadOnlyList<LikeProductValidationError> errors)
    {
        IsSuccess = isSuccess;
        Sn = sn;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public int? Sn { get; }

    public IReadOnlyList<LikeProductValidationError> Errors { get; }

    public static LikeProductSaveResult Success(int sn)
    {
        return new LikeProductSaveResult(true, sn, []);
    }

    public static LikeProductSaveResult Failure(IReadOnlyList<LikeProductValidationError> errors)
    {
        return new LikeProductSaveResult(false, null, errors);
    }
}
