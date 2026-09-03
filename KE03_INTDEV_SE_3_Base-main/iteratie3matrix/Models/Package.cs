namespace iteratie3matrix.Models;

/// <summary>
/// WHAT:
/// Represents a package that must be loaded.
///
/// WHY:
/// Drivers scan packages, not products.
/// </summary>
public class Package
{
    public int OPId { get; set; }

    public bool IsScanned { get; set; }

    public string Barcode =>
        $"PKG-{OPId:0000}";
}