namespace iteratie3matrix.Models;

public class ScannedPackage
{
    // =====================
    // SCAN IDENTIFIER
    // =====================

    /*
        WHAT:
        Barcode value (in this system = ProductId)

        WHY:
        Simplifies scanning logic during development.
    */
    public string Barcode { get; set; } = string.Empty;

    // =====================
    // TIMESTAMP
    // =====================

    public DateTime ScannedAt { get; set; }
}