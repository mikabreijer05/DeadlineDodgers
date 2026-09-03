using iteratie3matrix.Models;
using Microsoft.Data.SqlClient;

namespace iteratie3matrix.DAL;

public class StatusRepository
{
    // PURPOSE: central access to Status table
    // WHY: avoids hardcoded status values in UI

    private readonly SQLDAL _sqlDal;

    public StatusRepository()
    {
        _sqlDal = new SQLDAL();
    }

    // =========================
    // GET ALL STATUSES
    // =========================
    public async Task<List<Status>> ListAsync()
    {
        const string sql = """
                           SELECT StatusId, Status
                           FROM Status
                           ORDER BY StatusId;
                           """;

        var result = new List<Status>();

        await using var connection = _sqlDal.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Status
            {
                StatusId = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return result;
    }

    // =========================
    // GET SINGLE STATUS
    // =========================
    public async Task<Status?> GetAsync(int id)
    {
        const string sql = """
                           SELECT StatusId, Status
                           FROM Status
                           WHERE StatusId = @id;
                           """;

        await using var connection = _sqlDal.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Status
        {
            StatusId = reader.GetInt32(0),
            Name = reader.GetString(1)
        };
    }

    // =========================
    // SAFE UI HELPER
    // =========================
    public async Task<string> GetNameAsync(int id)
    {
        var status = await GetAsync(id);
        return status?.Name ?? "Unknown";
    }

    // =========================
    // FIX: UI DICTIONARY SUPPORT (USED BY PAGE MODEL)
    // =========================
    public async Task<Dictionary<int, string>> GetAllAsDictionaryAsync()
    {
        // WHAT: converts Status table into lookup dictionary
        // WHY: allows fast UI mapping (StatusId → StatusName)

        var dict = new Dictionary<int, string>();

        const string sql = """
                           SELECT StatusId, Status
                           FROM Status;
                           """;

        await using var connection = _sqlDal.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            dict[reader.GetInt32(0)] = reader.GetString(1);
        }

        return dict;
    }
}