namespace KE03_INTDEV_SE_2_Base.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string VehicleType { get; set; }
    public List<string> ProductDimensions { get; set; }
    public string LicensePlate { get; set; }
    public int TotalKM { get; set; }
    public string ParkingLocation { get; set; }
    
    public int MaxXsPackages { get; set; }
    public int MaxSPackages { get; set; }
    public int MaxMPackages { get; set; }
    public int MaxLPackages { get; set; }
    public int MaxXlPackages { get; set; }
}