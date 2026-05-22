namespace EsunBank.FinancialProducts.Services;

public interface ILikeProductService
{
    Task<IReadOnlyList<UserOptionResult>> GetUsersAsync();

    Task<LikeProductIndexResult> GetIndexAsync(string? userId);

    Task<LikeProductFormResult> BuildCreateFormAsync();

    Task<LikeProductFormResult?> BuildEditFormAsync(int sn);

    Task<LikeProductResult?> GetDetailAsync(int sn);

    Task<int> CreateAsync(LikeProductInfo info);

    Task UpdateAsync(int sn, LikeProductInfo info);

    Task<bool> DeleteAsync(int sn);
}
