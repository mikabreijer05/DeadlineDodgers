using Microsoft.Data.SqlClient;
using iteratie3matrix.Models;

namespace iteratie3matrix.DAL;

/// <summary>
/// WHAT:
/// Handles retrieval of address data.
///
/// WHY:
/// Converts AddressId into readable delivery locations
/// for courier UI.
/// </summary>
public class AddressRepository
{
    private readonly SQLDAL _sqlDal;

    public AddressRepository()
    {
        _sqlDal = new SQLDAL();
    }

    /// <summary>
    /// WHAT:
    /// Gets address details by ID.
    ///
    /// WHY:
    /// Used to build full delivery address strings
    /// for courier clarity.
    /// </summary>
    public async Task<Address?> GetAsync(int addressId)
    {
        const string sql = """
            SELECT AddressId,
                   Street,
                   HouseNumber,
                   PostalCode,
                   City,
                   Country
            FROM Address
            WHERE AddressId = @Id;
        """;

        await using var connection = _sqlDal.CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", addressId);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Address
        {
            AddressId = reader.GetInt32(0),
            Street = reader.GetString(1),
            HouseNumber = reader.GetString(2),
            PostalCode = reader.GetString(3),
            City = reader.GetString(4),
            Country = reader.GetString(5)
        };
    }
}