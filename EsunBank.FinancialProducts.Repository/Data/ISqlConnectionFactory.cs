using System.Data;

namespace EsunBank.FinancialProducts.Repository.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
