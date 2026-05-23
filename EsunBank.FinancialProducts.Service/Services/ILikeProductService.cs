namespace EsunBank.FinancialProducts.Service.Services;

public interface ILikeProductService
{
    Task<IReadOnlyList<UserOptionResult>> GetUsersAsync();

    Task<LikeProductIndexResult> GetIndexAsync(string? userId);

    Task<LikeProductFormResult> BuildCreateFormAsync();

    Task<LikeProductFormResult?> BuildEditFormAsync(int sn);

    Task<LikeProductResult?> GetDetailAsync(int sn);

    Task<LikeProductSaveResult> CreateAsync(LikeProductInfo info);

    Task<LikeProductSaveResult> UpdateAsync(int sn, LikeProductInfo info);

    Task<bool> DeleteAsync(int sn);
}
