using Microsoft.Data.SqlClient;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLDAL
{
    private readonly string _connectionString;
    protected readonly SqlConnection connection;

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public SQLDAL(IConfiguration configuration)
    {
        var activeConnectionName = configuration["DatabaseSettings:ActiveConnection"];

        _connectionString = configuration.GetConnectionString(activeConnectionName)
                            ?? throw new InvalidOperationException($"Connection string '{activeConnectionName}' was not found.");
        if (connection == null)
            connection = CreateConnection();
    }

    public void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}