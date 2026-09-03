using Microsoft.Data.SqlClient;
using iteratie3matrix.Models;

namespace iteratie3matrix.DAL;

/// <summary>
/// WHAT:
/// Handles access to account/customer data.
///
/// WHY:
/// Keeps account logic out of PageModels and ensures
/// consistent retrieval of customer information across the app.
/// </summary>
public class AccountRepository
{
    private readonly SQLDAL _sqlDal;

    public AccountRepository()
    {
        _sqlDal = new SQLDAL();
    }

    /// <summary>
    /// WHAT:
    /// Retrieves a single account by ID.
    ///
    /// WHY:
    /// Used in order details to display customer information
    /// instead of raw AccountId.
    /// </summary>
    public async Task<Account?> GetAsync(int accountId)
    {
        const string sql = """
            SELECT AccId,
                   AccName,
                   CustName,
                   AccountPunten,
                   AccActive
            FROM Account
            WHERE AccId = @Id;
        """;

        await using var connection = _sqlDal.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", accountId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Account
        {
            AccountId = reader.GetInt32(0),
            AccountName = reader.GetString(1),
            CustomerName = reader.GetString(2),
            Points = reader.GetInt32(3),
            IsActive = reader.GetBoolean(4)
        };
    }
}