using System;
using System.Collections.Generic;

public class VehicleSensors
{
    private Vehicle vehicle;

    private Dictionary<SensorType, ISensor> vehicleSensors;
    private Dictionary<Type, SensorType> typesToSensorTypes;
    private List<SensorType> orderedFixedUpdateSensors;

    public VehicleSensors(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.vehicleSensors = new Dictionary<SensorType, ISensor>()
        {
            // Fixed update sensors
            { SensorType.SURFACE_SENSOR, new SurfaceSensor(vehicle) },
            { SensorType.CORNER_SPEED_SENSOR, new CornerSpeedSensor(vehicle) },
            { SensorType.STOP_LINE_SENSOR, new StopLineSensor(vehicle) },
            { SensorType.STOP_NODE_SENSOR, new StopNodeSensor(vehicle) },
            { SensorType.FRONT_COLLISION_SENSOR, new FrontCollisionSensor(vehicle) },
            { SensorType.TRAFFIC_LIGHT_SENSOR, new TrafficLightSensor(vehicle) },
            // Triggered sensors
            { SensorType.SPEED_LIMIT_SENSOR, new SpeedLimitSensor(vehicle) },
            { SensorType.MERGE_SENSOR, new MergeSensor(vehicle) },
            { SensorType.DEADLOCK_SENSOR, new DeadlockSensor(vehicle) }
        };
        // Runs fixed update method from top to bottom.
        this.orderedFixedUpdateSensors = new List<SensorType>()
        {
            SensorType.SURFACE_SENSOR,
            SensorType.CORNER_SPEED_SENSOR,
            SensorType.STOP_NODE_SENSOR,
            SensorType.STOP_LINE_SENSOR,
            SensorType.FRONT_COLLISION_SENSOR,
            SensorType.TRAFFIC_LIGHT_SENSOR
        };
        this.typesToSensorTypes = new Dictionary<Type, SensorType>();
        foreach (KeyValuePair<SensorType, ISensor> pair in this.vehicleSensors)
        {
            typesToSensorTypes.Add(pair.Value.GetType(), pair.Key);
        }
    }

    public void StartSensors()
    {
        foreach (KeyValuePair<SensorType, ISensor> pair in this.vehicleSensors)
        {
            pair.Value.Start();
        }
    }

    public void RunFixedUpdate()
    {
        foreach (SensorType sensorType in this.orderedFixedUpdateSensors)
        {
            this.vehicleSensors[sensorType].Run(null);
        }
    }

    public void DrawSensorGizmos()
    {
        foreach (KeyValuePair<SensorType, ISensor> pair in this.vehicleSensors)
        {
            if (pair.Value is IGizmos)
            {
                ((IGizmos)pair.Value).OnDrawGizmos();
            }
        }
    }

    public T GetSensor<T>()
    {
        return (T)this.vehicleSensors[typesToSensorTypes[typeof(T)]];
    }

}
