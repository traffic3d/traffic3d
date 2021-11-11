using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadlockSensor : ISensor
{
    private Vehicle vehicle;
    public float currentDeadlockSeconds = 0;
    public bool deadlock;
    public float deadlockReleaseAtTime = 0;
    private const int deadlockSearchMaxAttempts = 100;
    private const float attemptAnotherDeadlockReleaseAfter = 5f;

    public DeadlockSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {
        this.vehicle.vehicleDriver.vehicleSensors.GetSensor<MergeSensor>().mergeSensorRunCompleteEvent.AddListener(() => this.Run(null));
    }

    public void Run(Dictionary<string, object> args)
    {
        deadlock = IsInDeadlock();
        UpdateDeadlockTime(deadlock);
        bool isReleasingDeadlock = IsReleasingDeadlock();
        if (deadlock && !isReleasingDeadlock && ShouldReleaseDeadlock())
        {
            currentDeadlockSeconds = 0;
            deadlockReleaseAtTime = Time.time;
            isReleasingDeadlock = true;
        }
        if (isReleasingDeadlock)
        {
            VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
            vehicleDriver.waitingForVehicleDriverAhead = null;
            vehicleDriver.waitingForVehicleDriverBehind = null;
            // Bypass merge speed and proceed with caution
            vehicle.vehicleEngine.SetTargetSpeed(vehicle.vehicleSettings.deadlockReleaseProceedSpeed);
            return;
        }
    }

    /// <summary>
    /// Checks if vehicle is currently in a merging deadlock
    /// Find out by checking through the vehicles that are being waited on to see if there is a loop back to the current vehicle.
    /// </summary>
    /// <returns>True if in a merging deadlock</returns>
    public bool IsInDeadlock()
    {
        VehicleDriver vehicleDriver = this.vehicle.vehicleDriver;
        if (vehicleDriver.waitingForVehicleDriverBehind == null && vehicleDriver.waitingForVehicleDriverAhead == null)
        {
            return false;
        }
        List<VehicleDriver> vehiclesToSearch = new List<VehicleDriver>();
        if (vehicleDriver.waitingForVehicleDriverAhead != null)
        {
            vehiclesToSearch.Add(vehicleDriver.waitingForVehicleDriverAhead);
        }
        if (vehicleDriver.waitingForVehicleDriverBehind != null)
        {
            vehiclesToSearch.Add(vehicleDriver.waitingForVehicleDriverBehind);
        }
        bool isInDeadlock = false;
        for (int i = 0; i < deadlockSearchMaxAttempts; i++)
        {
            if (vehiclesToSearch.Count == 0)
            {
                break;
            }
            VehicleDriver vehicleDriverToSearch = vehiclesToSearch.First();
            // Vehicle has been found in a loop of waiting vehicles.
            if (vehicleDriver.Equals(vehicleDriverToSearch.waitingForVehicleDriverAhead) || vehicleDriver.Equals(vehicleDriverToSearch.waitingForVehicleDriverBehind))
            {
                isInDeadlock = true;
                break;
            }
            if (vehicleDriverToSearch.waitingForVehicleDriverAhead != null)
            {
                if (vehicleDriverToSearch.waitingForVehicleDriverAhead.vehicleSensors.GetSensor<DeadlockSensor>().deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleDriverToSearch.waitingForVehicleDriverAhead);
            }
            if (vehicleDriverToSearch.waitingForVehicleDriverBehind != null)
            {
                if (vehicleDriverToSearch.waitingForVehicleDriverBehind.vehicleSensors.GetSensor<DeadlockSensor>().deadlock)
                {
                    isInDeadlock = true;
                    break;
                }
                vehiclesToSearch.Add(vehicleDriverToSearch.waitingForVehicleDriverBehind);
            }
            if (vehiclesToSearch.Count == 1 && vehicleDriverToSearch.waitingForVehicleDriverAhead == null && vehicleDriverToSearch.waitingForVehicleDriverBehind == null && vehicleDriverToSearch.isWaitingOnSensorRays)
            {
                isInDeadlock = true;
                break;
            }
            vehiclesToSearch.Remove(vehicleDriverToSearch);
        }
        return isInDeadlock;
    }

    /// <summary>
    /// Updates the deadlock value which is used to find out when to release the current deadlock.
    /// </summary>
    /// <param name="isInDeadlock">True if the vehicle is in deadlock</param>
    private void UpdateDeadlockTime(bool isInDeadlock)
    {
        if (isInDeadlock)
        {
            currentDeadlockSeconds = currentDeadlockSeconds + Time.fixedDeltaTime;
        }
        else
        {
            currentDeadlockSeconds = 0;
        }
    }

    /// <summary>
    /// Checks if the vehicle should release the deadlock
    /// </summary>
    /// <returns>True if the deadlock should be released</returns>
    private bool ShouldReleaseDeadlock()
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        if (currentDeadlockSeconds < vehicleSettings.releaseDeadlockAfterSeconds)
        {
            return false;
        }
        List<Vehicle> vehicleReleaseList = this.GenerateVehicleReleaseList();
        while (vehicleReleaseList.Count > 0)
        {
            Vehicle vehicleToCheck = vehicleReleaseList.First();
            if (vehicle.vehicleDriver.Equals(vehicleToCheck.vehicleDriver))
            {
                return true;
            }
            if (vehicleToCheck.vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().IsReleasingDeadlock() || !vehicleToCheck.vehicleDriver.isWaitingOnSensorRays)
            {
                return false;
            }
            vehicleReleaseList.Remove(vehicleToCheck);
        }
        // This is the last vehicle so should release
        return true;
    }

    public List<Vehicle> GenerateVehicleReleaseList()
    {
        return GameObject.FindObjectsOfType<Vehicle>().Where(v => v.vehicleDriver.vehicleSensors.GetSensor<DeadlockSensor>().deadlock).OrderByDescending(v => v.gameObject.GetInstanceID()).ToList();
    }

    public bool IsReleasingDeadlock()
    {
        return deadlockReleaseAtTime + attemptAnotherDeadlockReleaseAfter > Time.time;
    }
}
