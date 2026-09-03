using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iteratie3matrix.Models;

public class Van
{
    /*
        WHAT
        Unique vehicle identifier.

        WHY
        Used to identify the courier van.

        EXAMPLE
        12
        18
        24
    */
    public int VanId { get; set; }

    /*
        WHAT
        Readable vehicle name.

        WHY
        Easier for users to recognize.

        EXAMPLE
        Van 12
    */
    public string Name { get; set; } = string.Empty;

    /*
        WHAT
        Vehicle registration plate.

        WHY
        Real couriers identify vehicles by plate.

        EXAMPLE
        XX-123-X
    */
    public string LicensePlate { get; set; } = string.Empty;

    /*
        WHAT
        Parking location of the vehicle.

        WHY
        Helps couriers find the correct vehicle.

        EXAMPLE
        MC-012
        MC-018
        Loading Dock A
    */
    public string ParkingLocation { get; set; } = string.Empty;
}