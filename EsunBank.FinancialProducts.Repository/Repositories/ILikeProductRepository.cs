namespace EsunBank.FinancialProducts.Repository.Repositories;

public interface ILikeProductRepository
{
    Task<IReadOnlyList<UserOptionDto>> GetUsersAsync();

    Task<IReadOnlyList<LikeProductDetailDto>> GetListAsync(string? userId);

    Task<LikeProductDetailDto?> GetDetailAsync(int sn);

    Task<int> CreateAsync(LikeProductCommand command);

    Task UpdateAsync(int sn, LikeProductCommand command);

    Task DeleteAsync(int sn);
}
