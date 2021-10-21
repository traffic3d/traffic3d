using System;

public class VehicleEventArgs : EventArgs
{
    public Vehicle vehicle;

    public VehicleEventArgs(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }
}
