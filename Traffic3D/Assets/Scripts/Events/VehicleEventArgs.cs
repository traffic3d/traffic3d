using System;

public class VehicleEventArgs : EventArgs
{
    public VehicleEngine vehicleEngine;

    public VehicleEventArgs(VehicleEngine vehicleEngine)
    {
        this.vehicleEngine = vehicleEngine;
    }
}
