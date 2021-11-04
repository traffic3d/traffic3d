using System.Collections.Generic;
using System.Linq;

public class SpeedLimitSensor : ISensor
{
    private Vehicle vehicle;

    public SpeedLimitSensor(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public void Start()
    {
        this.vehicle.vehicleDriver.vehicleNavigation.nextNodeEvent.AddListener(roadNode => this.Run(null));
        this.Run(null);
    }

    public void Run(Dictionary<string, object> args)
    {
        VehicleNavigation vehicleNavigation = this.vehicle.vehicleDriver.vehicleNavigation;
        RoadNode roadNode = vehicleNavigation.path.nodes[vehicleNavigation.currentNodeNumber].GetComponent<RoadNode>();
        List<RoadWay> roadWays = new List<RoadWay>();
        if (vehicleNavigation.currentNodeNumber + 1 < vehicleNavigation.path.nodes.Count)
        {
            RoadNode nextRoadNode = vehicleNavigation.path.nodes[vehicleNavigation.currentNodeNumber + 1].GetComponent<RoadNode>();
            roadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNodes(roadNode, nextRoadNode);
        }
        else if (vehicleNavigation.currentNodeNumber > 0)
        {
            RoadNode previousRoadNode = vehicleNavigation.path.nodes[vehicleNavigation.currentNodeNumber - 1].GetComponent<RoadNode>();
            roadWays = RoadNetworkManager.GetInstance().GetRoadWaysFromNodes(previousRoadNode, roadNode);
        }
        if (roadWays.Count == 0)
        {
            return;
        }
        // Unlikely to get multiple road ways so just pick the first one.
        vehicle.vehicleSettings.maxSpeed = roadWays.First().speedLimit;
    }
}
