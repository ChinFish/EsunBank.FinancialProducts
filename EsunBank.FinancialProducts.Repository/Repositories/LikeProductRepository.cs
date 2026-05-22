using System.Data;
using Dapper;
using EsunBank.FinancialProducts.Data;

namespace EsunBank.FinancialProducts.Repositories;

public sealed class LikeProductRepository(ISqlConnectionFactory connectionFactory) : ILikeProductRepository
{
    public async Task<IReadOnlyList<UserOptionDto>> GetUsersAsync()
    {
        using var connection = connectionFactory.CreateConnection();
        var users = await connection.QueryAsync<UserOptionDto>(
            "dbo.usp_User_GetOptions",
            commandType: CommandType.StoredProcedure);

        return users.AsList();
    }

    public async Task<IReadOnlyList<LikeProductDetailDto>> GetListAsync(string? userId)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<LikeProductDetailDto>(
            "dbo.usp_LikeProduct_GetList",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);

        return items.AsList();
    }

    public async Task<LikeProductDetailDto?> GetDetailAsync(int sn)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<LikeProductDetailDto>(
            "dbo.usp_LikeProduct_GetDetail",
            new { Sn = sn },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(LikeProductCommand command)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(
            "dbo.usp_LikeProduct_Create",
            command,
            commandType: CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(int sn, LikeProductCommand command)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "dbo.usp_LikeProduct_Update",
            new
            {
                Sn = sn,
                command.UserId,
                command.ProductName,
                command.Price,
                command.FeeRate,
                command.Account,
                command.PurchaseQuantity
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int sn)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "dbo.usp_LikeProduct_Delete",
            new { Sn = sn },
            commandType: CommandType.StoredProcedure);
    }
}
