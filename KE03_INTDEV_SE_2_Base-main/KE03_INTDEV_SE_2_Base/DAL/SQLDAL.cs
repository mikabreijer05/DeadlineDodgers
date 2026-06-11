using Microsoft.Data.SqlClient;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLDAL
{
    private readonly string _serverAddress = "localhost";
    private readonly string _databaseName = "TryoutDatabase";

    // Kept both constructors, but fixed assignment issue
    public SQLDAL(string serverAddress, string databaseName)
    {
        _serverAddress = serverAddress;
        _databaseName = databaseName;
    }

    public SqlConnection CreateConnection(string userId, string password)
    {
        var connectionString =
             $"Data Source={_serverAddress};" +
            $"Initial Catalog={_databaseName};" +
            $"Integrated Security=True;" +
            $"Encrypt=True;" +
            $"TrustServerCertificate=True;";

        return new SqlConnection(connectionString);
    }

    protected readonly SqlConnection connection;

    public SQLDAL()
    {
        if (connection == null)
            connection = CreateConnection("sa", "FJL7MzZPckC37uheZHp");
    }

    public void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}