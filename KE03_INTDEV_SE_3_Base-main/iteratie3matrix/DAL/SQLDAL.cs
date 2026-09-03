using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace iteratie3matrix.DAL;

public class SQLDAL
{
    // WHAT: Azure SQL Server connection layer
    // WHY: Centralizes DB configuration and avoids duplication

    private readonly IConfiguration? _configuration;

    public SQLDAL()
    {
        // Default fallback for non-DI usage
    }

    public SQLDAL(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // WHAT: Creates SQL connection
    // WHY: Handles Azure SQL + long startup sleep recovery
    public SqlConnection CreateConnection()
    {
        var connectionString =
            _configuration?.GetConnectionString("Default")
            ?? BuildFallbackConnection();

        return new SqlConnection(connectionString);
    }

    private string BuildFallbackConnection()
    {
        return
            "Server=tcp:matrixincdd.database.windows.net,1433;" +
            "Initial Catalog=free-sql-db-3506747;" +
            "User ID=matrixincddadmin;" +
            "Password=dgp5U3DdF!SX;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=120;";
    }
}