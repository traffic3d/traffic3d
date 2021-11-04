using System;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSensor : ISensor
{
    private Vehicle vehicle;

    public SurfaceSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {

    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleSettings vehicleSettings = this.vehicle.vehicleSettings;
        WheelHit hit;
        if (vehicleSettings.wheelColliderFrontLeft.GetGroundHit(out hit))
        {
            float staticFriction = hit.collider.material.staticFriction;
            float dyanmicFriction = hit.collider.material.dynamicFriction;
            foreach (WheelCollider wheelCollider in vehicle.gameObject.GetComponentsInChildren<WheelCollider>())
            {
                WheelFrictionCurve wheelFrictionCurveForward = CloneWheelFrictionCurve(wheelCollider.forwardFriction);
                wheelFrictionCurveForward.extremumValue = staticFriction;
                wheelFrictionCurveForward.asymptoteValue = dyanmicFriction;
                WheelFrictionCurve wheelFrictionCurveSideways = CloneWheelFrictionCurve(wheelCollider.sidewaysFriction);
                wheelFrictionCurveSideways.extremumValue = staticFriction;
                wheelFrictionCurveSideways.asymptoteValue = dyanmicFriction;
                wheelCollider.forwardFriction = wheelFrictionCurveForward;
                wheelCollider.sidewaysFriction = wheelFrictionCurveSideways;
            }
            vehicle.vehicleEngine.currentMotorTorque = Math.Min(vehicleSettings.maxMotorTorque, vehicleSettings.maxMotorTorque * dyanmicFriction);
        }
    }

    private WheelFrictionCurve CloneWheelFrictionCurve(WheelFrictionCurve wheelFrictionCurveToClone)
    {
        WheelFrictionCurve newWheelFrictionCurve = new WheelFrictionCurve();
        newWheelFrictionCurve.extremumValue = wheelFrictionCurveToClone.extremumValue;
        newWheelFrictionCurve.extremumSlip = wheelFrictionCurveToClone.extremumSlip;
        newWheelFrictionCurve.asymptoteValue = wheelFrictionCurveToClone.asymptoteValue;
        newWheelFrictionCurve.asymptoteSlip = wheelFrictionCurveToClone.asymptoteSlip;
        newWheelFrictionCurve.stiffness = wheelFrictionCurveToClone.stiffness;
        return newWheelFrictionCurve;
    }
}
