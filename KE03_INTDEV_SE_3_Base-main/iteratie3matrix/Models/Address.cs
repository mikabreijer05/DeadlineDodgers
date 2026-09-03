namespace iteratie3matrix.Models;

/// <summary>
/// WHAT:
/// Represents a physical delivery location.
///
/// WHY:
/// Used to convert AddressId into a readable courier address.
/// </summary>
public class Address
{
    public int AddressId { get; set; }
    public string Street { get; set; } = "";
    public string HouseNumber { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";

    /// <summary>
    /// WHAT:
    /// Full formatted address string.
    ///
    /// WHY:
    /// Keeps UI clean and consistent.
    /// </summary>
    public string FullAddress =>
        $"{Street} {HouseNumber}, {PostalCode} {City}, {Country}";
}