using System.Data;

namespace EsunBank.FinancialProducts.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
