using iteratie3matrix.Models;
using Microsoft.Data.SqlClient;

namespace iteratie3matrix.DAL;

public class VehicleRepository
{
    private readonly SQLDAL _sqlDal;

    public VehicleRepository(SQLDAL sqlDal)
    {
        _sqlDal = sqlDal;
    }

    // =====================
    // GET ALL VEHICLES
    // =====================
    public async Task<List<Van>> GetAllAsync()
    {
        var vans = new List<Van>();

        const string sql = """
            SELECT VehicleId, LicensePlate, ParkingLocationId
            FROM Vehicle
            """;

        await using var conn = _sqlDal.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vans.Add(new Van
            {
                VanId = reader.GetInt32(0),
                LicensePlate = reader.GetString(1),
                ParkingLocation = $"Location {reader.GetInt32(2)}",
                Name = $"Van {reader.GetInt32(0)}"
            });
        }

        return vans;
    }
}