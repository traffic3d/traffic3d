using UnityEngine;

public class VehicleDriver : MonoBehaviour
{
    public Vehicle vehicle;
    public VehicleSettings vehicleSettings;
    public VehicleNavigation vehicleNavigation;
    public VehicleSensors vehicleSensors;
    public bool isWaitingOnSensorRays = false;
    public float startTime;
    public Vector3 startPos;
    public bool densityCountTriggered = false;
    public bool debug = false;
    public VehicleDriver waitingForVehicleDriverAhead = null;
    public VehicleDriver waitingForVehicleDriverBehind = null;

    private void Awake()
    {
        vehicle = gameObject.GetComponent<Vehicle>();
        vehicleSettings = gameObject.GetComponent<VehicleSettings>();
        vehicleNavigation = gameObject.AddComponent<VehicleNavigation>();
        this.vehicleSensors = new VehicleSensors(vehicle);
    }

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        startTime = Time.time;
        startPos = transform.position;
        vehicleSensors.StartSensors();
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "car")
        {
            other.gameObject.tag = "collided";
        }
    }

    /// <summary>
    /// The main fixed loop of the vehicle object.
    /// Used for methods that need fixed timings for accurate physics when speeding up the simulation.
    /// The main loop updates multiple components of the vehicle such as:
    /// * Vehicle Sensors - Updates multiple properties of the vehicle such as target speed depending on certain conditions.
    /// * Vehicle Navigation - Updates the direction of the vehicle by using the path the vehicle is following.
    /// </summary>
    private void FixedUpdate()
    {
        if (vehicleNavigation.path == null)
        {
            return;
        }
        vehicleNavigation.CheckNextNode();
        if (vehicleNavigation.IsAtDestination())
        {
            vehicle.DestroyVehicle();
            return;
        }
        vehicleNavigation.ApplySteer();
        // Reset vehicle target speed
        vehicle.vehicleEngine.SetTargetSpeed(vehicleSettings.maxSpeed);
        vehicleSensors.RunFixedUpdate();
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            vehicleSensors.DrawSensorGizmos();
        }
    }
}

