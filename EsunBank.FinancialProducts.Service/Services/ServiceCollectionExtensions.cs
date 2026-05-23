using EsunBank.FinancialProducts.Repository.Data;
using EsunBank.FinancialProducts.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EsunBank.FinancialProducts.Service.Services;

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
