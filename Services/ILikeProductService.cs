using EsunBank.FinancialProducts.ViewModels;

namespace EsunBank.FinancialProducts.Services;

public interface ILikeProductService
{
    Task<IReadOnlyList<UserOptionViewModel>> GetUsersAsync();

    Task<LikeProductIndexViewModel> GetIndexAsync(string? userId);

    Task<LikeProductFormViewModel> BuildCreateFormAsync();

    Task<LikeProductFormViewModel?> BuildEditFormAsync(int sn);

    Task<LikeProductListItemViewModel?> GetDetailAsync(int sn);

    Task<int> CreateAsync(LikeProductFormViewModel form);

    Task UpdateAsync(int sn, LikeProductFormViewModel form);

    Task DeleteAsync(int sn);
}
