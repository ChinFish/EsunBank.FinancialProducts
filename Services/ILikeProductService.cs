using EsunBank.FinancialProducts.ViewModels;

namespace EsunBank.FinancialProducts.Services;

public interface ILikeProductService
{
    Task<IReadOnlyList<UserOptionViewModel>> GetUsersAsync();

    Task<LikeProductIndexViewModel> GetIndexAsync(string? userId);

    Task<LikeProductFormViewModel> BuildCreateFormAsync();

    Task<LikeProductFormViewModel?> BuildEditFormAsync(int sn);

    Task<LikeProductListItemViewModel?> GetDetailAsync(int sn);

    Task<int> CreateAsync(LikeProductInfo info);

    Task UpdateAsync(int sn, LikeProductInfo info);

    Task<bool> DeleteAsync(int sn);
}
