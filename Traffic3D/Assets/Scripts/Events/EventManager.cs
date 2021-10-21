using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Event Manager tracks and listens to events happening in Traffic3D such as when a vehicle spawns or when it's destroyed.
/// 
/// To call an event use the Call methods such as:
/// <code>CallVehicleSpawnEvent(this, new VehicleEventArgs(vehicleDriver))</code>
/// 
/// To listen to an event use:
/// <code>EventManager.GetInstance().VehicleSpawnEvent += OnVehicleSpawnEvent;</code>
/// 
/// Then add a method in the listeneing object such as:
/// <code>
/// public void OnVehicleSpawnEvent(object sender, VehicleEventArgs args)
/// {
///     AddVehicleIntersectionPoint(args.vehicleDriver);
/// }
/// </code>
/// 
/// Be sure to unregister the listener once done:
/// <code>EventManager.GetInstance().VehicleSpawnEvent -= OnVehicleSpawnEvent;</code>
/// </summary>
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
        if (VehicleSpawnEvent != null && args.vehicle != null)
        {
            VehicleSpawnEvent.Invoke(sender, args);
        }
    }

    public void CallVehicleDestroyEvent(object sender, VehicleEventArgs args)
    {
        if (VehicleDestroyEvent != null && args.vehicle != null)
        {
            VehicleDestroyEvent.Invoke(sender, args);
        }
    }
}
