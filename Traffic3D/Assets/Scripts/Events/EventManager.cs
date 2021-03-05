using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager instance;

    public static EventManager GetInstance()
    {
        if (instance == null)
        {
            instance = new EventManager();
        }
        return instance;
    }

    public static void Destroy()
    {
        instance = null;
    }

    public event EventHandler<VehicleEventArgs> VehicleSpawnEvent;
    public event EventHandler<VehicleEventArgs> VehicleDestroyEvent;

    private EventManager()
    {

    }

    public void CallVehicleSpawnEvent(object sender, VehicleEventArgs args)
    {
        if (VehicleSpawnEvent != null && args.vehicleEngine != null)
        {
            VehicleSpawnEvent.Invoke(sender, args);
        }
    }

    public void CallVehicleDestroyEvent(object sender, VehicleEventArgs args)
    {
        if (VehicleDestroyEvent != null && args.vehicleEngine != null)
        {
            VehicleDestroyEvent.Invoke(sender, args);
        }
    }
}
