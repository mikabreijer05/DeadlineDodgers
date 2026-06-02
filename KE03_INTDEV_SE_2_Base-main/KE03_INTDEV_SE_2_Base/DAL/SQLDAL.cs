using Microsoft.Data.SqlClient;
namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLDAL
{
    private readonly string _serverAddress = "matrixincserver.database.windows.net";
    private readonly string _databaseName = "MatrixInc";

    public SQLDAL(string serverAddress, string databaseName)
    {
        _serverAddress = serverAddress;
        _databaseName = databaseName;
    }

    public SqlConnection CreateConnection(string userId, string password)
    {
        var connectionString =
            $"Server=tcp:{_serverAddress},1433;" +
            $"Initial Catalog={_databaseName};" +
            $"Persist Security Info=False;" +
            $"User ID={userId};" +
            $"Password={password};" +
            $"MultipleActiveResultSets=False;" +
            $"Encrypt=True;" +
            $"TrustServerCertificate=False;" +
            $"Connection Timeout=30;";

        return new SqlConnection(connectionString);
    }

    protected readonly SqlConnection connection;

    public SQLDAL()
    {
        if (connection == null)
            connection = CreateConnection("DD-admin", "FJL7MzZPckC37uheZHp");
    }

    public void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }
}