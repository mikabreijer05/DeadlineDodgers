using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iteratie3matrix.Services;

public enum DeliveryState
{
    None = 0,
    OrderSelected = 1,
    VehicleInspection = 2,
    Loading = 3,
    Scanning = 4,
    ReadyForDelivery = 5
}