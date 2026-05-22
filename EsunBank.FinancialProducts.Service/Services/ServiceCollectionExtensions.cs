using EsunBank.FinancialProducts.Data;
using EsunBank.FinancialProducts.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EsunBank.FinancialProducts.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinancialProductServices(this IServiceCollection services)
    {
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ILikeProductRepository, LikeProductRepository>();
        services.AddScoped<ILikeProductService, LikeProductService>();

        return services;
    }
}
