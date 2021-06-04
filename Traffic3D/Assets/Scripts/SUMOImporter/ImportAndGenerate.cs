using Assets.Scripts.SUMOImporter.NetFileComponents;
using SplineMesh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class ImportAndGenerate
{

    static GameObject network;

    public static Dictionary<string, NetFileJunction> junctions;
    public static Dictionary<string, NetFileLane> lanes;
    public static Dictionary<string, NetFileEdge> edges;
    public static Dictionary<string, Shape> shapes;
    public static Dictionary<string, List<string>> routes;
    public static Dictionary<string, tlLogicType> trafficLightPrograms;
    private static List<GameObject> buildings;

    public static List<Vector3[]> polygons;

    static string sumoFilesPath;
    static string sumoBinPath;

    static float xmin;
    static float xmax;
    static float ymin;
    static float ymax;

    static Terrain Terrain;
    static TerrainData terrainData;

    static float numPlants;

    static float scaleLength = 2;
    static float scaleWidth = 2;

    static float meshScaleX = 3.3f;
    static float uvScaleV = 5;
    static float uvScaleU = 1;

    static float junctionHeight = 0.01f;
    static float trafficLightDistance = 2f;
    static float minLengthForStreetLamp = 12;
    static float streeLampDistance = 6f;

    static int maxLanesForBuildings = 2;
    static float lengthFromRoadToBuilding = 15f;
    static float lengthFromBuildingToBuildingAlongRoad = 20f;

    static float cameraHeight = 14f;
    static float cameraDistance = 20f;
    static float cameraTiltDown = 35f;

    static Boolean grassEnabled = true;
    static Boolean treesEnabled = true;

    static string[] plants = { "tree01", "tree02", "tree03", "tree04", "bush01", "bush02", "bush03", "bush04", "bush05", "bush06" };

    public static void parseXMLfiles(string sumoFilesPath)
    {
        ImportAndGenerate.sumoFilesPath = sumoFilesPath;

        network = new GameObject("StreetNetwork");

        string netFilePath = sumoFilesPath + "/map.net.xml";
        string shapesFilePath = sumoFilesPath + "/map.poly.xml";
        string routesFilePath = sumoFilesPath + "/map.rou.xml";

        lanes = new Dictionary<string, NetFileLane>();
        edges = new Dictionary<string, NetFileEdge>();
        junctions = new Dictionary<string, NetFileJunction>();
        shapes = new Dictionary<string, Shape>();
        routes = new Dictionary<string, List<string>>();
        trafficLightPrograms = new Dictionary<string, tlLogicType>();
        buildings = new List<GameObject>();

        netType netFile;
        XmlSerializer serializer = new XmlSerializer(typeof(netType));
        FileStream fs = new FileStream(netFilePath, FileMode.OpenOrCreate);
        TextReader rd = new StreamReader(fs);
        netFile = (netType)serializer.Deserialize(rd);

        // Get all junctions and preinstanciate lanes
        foreach (junctionType junction in netFile.junction)
        {
            if (junction.type != junctionTypeType.@internal)
            {
                NetFileJunction j = new NetFileJunction(junction.id, junction.type, junction.x, junction.y, junction.z, junction.incLanes, junction.shape);

                // Add to global list
                if (!junctions.ContainsKey(j.id))
                    junctions.Add(j.id, j);
            }
        }

        // Get all edges and complete lane objects
        foreach (edgeType edge in netFile.edge)
        {
            if (!edge.functionSpecified)
            {
                // Only non-internal edges
                NetFileEdge e = new NetFileEdge(edge.id, edge.from, edge.to, edge.priority, edge.shape);

                // Add to global list
                if (!edges.ContainsKey(edge.id))
                    edges.Add(edge.id, e);

                foreach (laneType l in edge.lane)
                {
                    // Add all lanes which belong to this edge
                    e.addLane(l.id, l.index, l.speed, l.length, l.shape);
                }
            }
        }

        // Get all traffic light programs
        if (netFile.tlLogic != null)
        {
            foreach (tlLogicType tlLogic in netFile.tlLogic)
            {
                trafficLightPrograms.Add(tlLogic.id, tlLogic);
            }
        }

        // Get map boundaries
        string[] boundaries = netFile.location.convBoundary.Split(',');
        xmin = float.Parse(boundaries[0]);
        ymin = float.Parse(boundaries[1]);
        xmax = float.Parse(boundaries[2]);
        ymax = float.Parse(boundaries[3]);

        // Reset
        xmin = 0;
        ymin = 0;
        xmax = 0;
        ymax = 0;


        // Now import polygons/shapes for buildings information
        additionalType additional;
        serializer = new XmlSerializer(typeof(additionalType));
        fs = new FileStream(shapesFilePath, FileMode.OpenOrCreate);
        rd = new StreamReader(fs);
        try
        {
            additional = (additionalType)serializer.Deserialize(rd);

            // Get all junctions and preinstanciate lanes
            foreach (object item in additional.Items)
            {
                if (item.GetType() == typeof(polygonType))
                {
                    Shape shape = new Shape();
                    polygonType polygon = (polygonType)item;
                    foreach (String s in polygon.shape.Split(' '))
                    {
                        shape.addCoordPair(Convert.ToDouble(s.Split(',')[0]), Convert.ToDouble(s.Split(',')[1]));
                    }
                    shape.removeLastCoordPairAndFixOrder();
                    shapes.Add(polygon.id, shape);
                }
            }
        }
        catch (Exception e)
        {
        }

        // Import all routes
        XmlDocument routeXmlFile = new XmlDocument();
        routeXmlFile.Load(routesFilePath);

        XmlNodeList routeXml = routeXmlFile.GetElementsByTagName("route");

        foreach (XmlNode route in routeXml)
        {
            string[] edges = route.Attributes["edges"].Value.Split(' ');
            string id = route.Attributes["id"].Value;
            routes.Add(id, edges.ToList());
        }

    }

    public static void CreateStreetNetwork()
    {
        polygons = new List<Vector3[]>();

        bool linearOption = true;

        int laneCounter = 0;
        int streetLightCounter = 0;
        int buildingCounter = 0;

        GameObject[] buildingsToPlace = Resources.LoadAll<GameObject>("Models/Buildings");

        MonoBehaviour.print("Inserting 3d Streets");

        foreach (NetFileEdge e in edges.Values)
        {

            foreach (NetFileLane l in e.getLanes())
            {
                int lineCounter = 0;
                GameObject spline = new GameObject("LaneSegment_" + l.id);
                spline.AddComponent<RoadWay>();
                spline.transform.SetParent(network.transform);

                Spline splineObject = spline.AddComponent<Spline>();
                SplineMeshTiling splineMeshTiling = spline.AddComponent<SplineMeshTiling>();

                Vector3 nextDirection = new Vector3((float)l.shape[0][0] - xmin, 0, (float)l.shape[0][1] - ymin) - new Vector3((float)l.shape[1][0] - xmin, 0, (float)l.shape[1][1] - ymin);
                Vector3 lastPosition = new Vector3((float)l.shape[0][0] - xmin, 0, (float)l.shape[0][1] - ymin);
                splineObject.AddNode(new SplineNode(lastPosition, lastPosition - nextDirection.normalized));
                bool isFirst = true;
                foreach (double[] coordPair in l.shape)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        continue;
                    }
                    nextDirection = lastPosition - new Vector3((float)coordPair[0] - xmin, 0, (float)coordPair[1] - ymin);
                    splineObject.AddNode(new SplineNode(new Vector3((float)coordPair[0] - xmin, 0, (float)coordPair[1] - ymin), new Vector3((float)coordPair[0] - xmin, 0, (float)coordPair[1] - ymin) - nextDirection.normalized));
                    lastPosition = new Vector3((float)coordPair[0] - xmin, 0, (float)coordPair[1] - ymin);
                }

                int nodeCounter = 0;
                foreach (SplineNode splineNode in splineObject.nodes)
                {
                    GameObject node = new GameObject("Node_" + nodeCounter);
                    node.transform.SetParent(spline.transform);
                    node.transform.position = splineNode.Position;
                    nodeCounter++;
                }

                // Add meshes
                Material material = Resources.Load<Material>("Materials/planeMaterial");
                MeshRenderer mRenderer = mRenderer = spline.GetComponent<MeshRenderer>();
                if (mRenderer == null)
                {
                    mRenderer = spline.AddComponent<MeshRenderer>();
                }
                mRenderer.material = material;

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

                splineMeshTiling.material = Resources.Load<Material>("Materials/planeMaterial");
                splineMeshTiling.mesh = plane.GetComponent<MeshFilter>().mesh;
                splineMeshTiling.updateInPlayMode = true;
                splineMeshTiling.scale = new Vector3(0.3f, 1, 0.3f);
                GameObject.Destroy(plane);

                // (1.1) Add Lanes to polygon list for tree placement check
                for (int i = 0; i < l.shape.Count - 1; i++)
                {
                    double length = Math.Sqrt(Math.Pow(l.shape[i][0] - xmin - (l.shape[i + 1][0] - xmin), 2) + Math.Pow(l.shape[i][1] - ymin - (l.shape[i + 1][1] - ymin), 2));
                    // Calc the position (in line with the lane)
                    float x1 = (float)l.shape[i][0] - xmin;
                    float y1 = (float)l.shape[i][1] - ymin;
                    float x2 = (float)l.shape[i + 1][0] - xmin;
                    float y2 = (float)l.shape[i + 1][1] - ymin;
                    double Dx = x2 - x1;
                    double Dy = y2 - y1;
                    double D = Math.Sqrt(Dx * Dx + Dy * Dy);
                    double W = 10;
                    Dx = 0.5 * W * Dx / D;
                    Dy = 0.5 * W * Dy / D;
                    Vector3[] polygon = new Vector3[] { new Vector3((float)(x1 - Dy), 0, (float)(y1 + Dx)),
                                                    new Vector3((float)(x1 + Dy), 0, (float)(y1 - Dx)),
                                                    new Vector3((float)(x2 + Dy), 0, (float)(y2 - Dx)),
                                                    new Vector3((float)(x2 - Dy), 0, (float)(y2 + Dx)) };
                    polygons.Add(polygon);


                    // (2) Add Street Lamps (only if long enough)
                    if (length >= minLengthForStreetLamp && e.getLanes().Count == 1)
                    {
                        float angle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;

                        // Allway located at the middle of a street
                        double ratioRotPoint = 0.5;
                        double ratio = 0.5 + streeLampDistance / length;

                        float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                        float yDest = (float)((1 - ratio) * y1 + ratio * y2);

                        float xRotDest = (float)((1 - ratioRotPoint) * x1 + ratioRotPoint * x2);
                        float yRotDest = (float)((1 - ratioRotPoint) * y1 + ratioRotPoint * y2);

                        GameObject streetLampPrefab = Resources.Load<GameObject>("Models/streetlight");
                        GameObject streetLamp = GameObject.Instantiate(streetLampPrefab, new Vector3(xDest, 0, yDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                        streetLamp.name = "StreetLight_" + streetLightCounter++;
                        streetLamp.transform.SetParent(network.transform);
                        streetLamp.transform.RotateAround(new Vector3(xRotDest, 0, yRotDest), Vector3.up, -90.0f);
                        streetLamp.transform.Rotate(Vector3.up, -angle);
                    }

                    if (buildingsToPlace.Length > 0 && length >= lengthFromBuildingToBuildingAlongRoad && e.getLanes().Count <= maxLanesForBuildings)
                    {
                        // Get the angle perpendicular to the road, the number of buildings along the road and the exact length between them.
                        float angle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;
                        int amountOfBuildings = (int)(length / lengthFromBuildingToBuildingAlongRoad);
                        double lengthBetweenBuildings = length / (amountOfBuildings + 1);

                        for (int buildingNum = 0; buildingNum < amountOfBuildings; buildingNum++)
                        {
                            GameObject building = buildingsToPlace[RandomNumberGenerator.GetInstance().Range(0, buildingsToPlace.Length)];

                            // Find the ratio along the road e.g. the building is halfway down the road and then work out the x and y coords for that point on the road.
                            double ratio = (lengthBetweenBuildings * (buildingNum + 1)) / length;
                            float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                            float yDest = (float)((1 - ratio) * y1 + ratio * y2);

                            GameObject buildingCreated = GameObject.Instantiate(building, new Vector3(xDest, 0, yDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                            buildingCreated.transform.SetParent(network.transform);
                            // Rotate and place building on road then move forward away from the road by a constant. 
                            buildingCreated.transform.Rotate(Vector3.up, -angle);
                            buildingCreated.transform.position = buildingCreated.transform.position + buildingCreated.transform.forward * lengthFromRoadToBuilding;
                            Physics.SyncTransforms();
                            Collider[] otherCollisions = Physics.OverlapBox(buildingCreated.gameObject.GetComponentInChildren<BoxCollider>().bounds.center, buildingCreated.gameObject.GetComponentInChildren<BoxCollider>().bounds.extents, buildingCreated.transform.rotation);
                            bool isBuildingAlready = false;
                            foreach (Collider collider in otherCollisions)
                            {
                                if (collider.transform.IsChildOf(buildingCreated.transform))
                                {
                                    continue;
                                }
                                foreach (GameObject otherBuilding in buildings)
                                {
                                    if (collider.transform.IsChildOf(otherBuilding.transform))
                                    {
                                        isBuildingAlready = true;
                                        break;
                                    }
                                }
                            }
                            if (!isBuildingAlready)
                            {
                                buildingCreated.name = "Building_" + buildingCounter++;
                                buildings.Add(buildingCreated);
                            }
                            else
                            {
                                GameObject.Destroy(buildingCreated);
                            }
                        }
                    }
                }

            }


        }

        // (3) Draw all Junction areas ------------------------------------
        MonoBehaviour.print("Inserting 3d Junctions");

        int junctionCounter = 0;
        foreach (NetFileJunction j in junctions.Values)
        {
            List<int> indices = new List<int>();

            Vector2[] vertices2D = new Vector2[j.shape.Count];
            for (int i = 0; i < j.shape.Count; i++)
            {
                vertices2D[i] = new Vector3((float)(j.shape[i])[0] - xmin, (float)(j.shape[i])[1] - ymin);
            }

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D);
            List<int> bottomIndices = new List<int>(tr.Triangulate());
            indices.AddRange(bottomIndices);


            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
            }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Bounds bounds = mesh.bounds;
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / bounds.size.x, vertices[i].z / bounds.size.z);
            }
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Set up game object with mesh;
            GameObject junction3D = new GameObject("junction_" + junctionCounter++);
            MeshRenderer r = (MeshRenderer)junction3D.AddComponent(typeof(MeshRenderer));
            tlLogicType tlLogicType = trafficLightPrograms.ToList().Find(e => e.Key.Equals(j.id) && e.Value.Items.Length > 0).Value;
            if (tlLogicType != null)
            {
                Junction junction = junction3D.AddComponent<Junction>();
                junction.junctionId = j.id;
                // Create Camera object for the junction.
                GameObject cameraGameObject = new GameObject("junction_camera_" + j.id);
                cameraGameObject.transform.SetParent(junction3D.transform);
                // Get the center position of the Junction.
                cameraGameObject.transform.position = mesh.bounds.center;
                // Move the camera back for "cameraDistance" and up for "cameraHeight".
                cameraGameObject.transform.position = cameraGameObject.transform.position + (cameraGameObject.transform.TransformDirection(Vector3.back) * cameraDistance) + (cameraGameObject.transform.TransformDirection(Vector3.up) * cameraHeight);
                // Look the camera down by "cameraTiltDown".
                cameraGameObject.transform.Rotate(new Vector3(cameraTiltDown, 0, 0));
                // Add the Camera component to the object, assign it to the junction and add the CameraManager component.
                Camera camera = cameraGameObject.AddComponent<Camera>();
                junction.junctionCamera = camera;
                cameraGameObject.AddComponent<CameraManager>();
            }
            Material material = Resources.Load<Material>("Materials/sidewalk");
            r.material = material;
            MeshFilter filter = junction3D.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            junction3D.transform.SetParent(network.transform);

            // (3.1) Add junctions to polygon list for tree placement check
            polygons.Add(vertices);
        }

        // (4) Draw Traffic Lights
        MonoBehaviour.print("Inserting 3d Traffic Lights");

        foreach (NetFileJunction j in junctions.Values)
        {
            if (j.type == junctionTypeType.traffic_light)
            {
                int index = 0;
                foreach (NetFileLane l in j.incLanes)
                {
                    // Calc the position (in line with the lane)
                    float x1 = (float)l.shape[0][0] - xmin;
                    float y1 = (float)l.shape[0][1] - ymin;
                    float x2 = (float)l.shape[1][0] - xmin;
                    float y2 = (float)l.shape[1][1] - ymin;
                    float length = (float)Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));
                    float angle = Mathf.Atan2(y2 - y1, x2 - x1) * 180 / Mathf.PI;

                    double ratio = (length - trafficLightDistance) / length;

                    float xDest = (float)((1 - ratio) * x1 + ratio * x2);
                    float yDest = (float)((1 - ratio) * y1 + ratio * y2);

                    string trafficLightId = l.id;
                    // Insert the 3d object, rotate from lane 90° to the right side and then orientate the traffic light towards the vehicles
                    GameObject trafficLightPrefab = Resources.Load<GameObject>("Models/TrafficLight");
                    GameObject trafficLight = GameObject.Instantiate(trafficLightPrefab, new Vector3(xDest, 0, yDest), Quaternion.Euler(new Vector3(0, 0, 0)));
                    trafficLight.name = "TrafficLight_" + trafficLightId;
                    trafficLight.GetComponentInChildren<TrafficLight>().trafficLightId = trafficLightId;
                    trafficLight.transform.SetParent(network.transform);
                    trafficLight.transform.RotateAround(new Vector3(x2, 0, y2), Vector3.up, -90.0f);
                    trafficLight.transform.Rotate(Vector3.up, -angle);

                    // Insert traffic light index as empty GameObject into traffic light
                    GameObject TLindex = new GameObject("index");
                    GameObject TLindexVal = new GameObject(Convert.ToString(index));
                    TLindexVal.transform.SetParent(TLindex.transform);
                    TLindex.transform.SetParent(trafficLight.transform);
                    index++;
                }
            }
        }

    }

    static Mesh CreateMesh(Vector2[] poly, float height)
    {
        float extrusionZMinus = 0;
        float extrusionZPlus = Mathf.Max(height, 0);

        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length * 2];

        for (int i = 0; i < poly.Length; i++)
        {
            vertices[i].x = poly[i].x;
            vertices[i].y = extrusionZMinus;
            vertices[i].z = poly[i].y; // front vertex
            vertices[i + poly.Length].x = poly[i].x;
            vertices[i + poly.Length].y = extrusionZPlus;
            vertices[i + poly.Length].z = poly[i].y;  // back vertex    
        }
        int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
        int count_tris = 0;
        for (int i = 0; i < tris.Length; i += 3)
        {
            triangles[i] = tris[i + 2];
            triangles[i + 1] = tris[i + 1];
            triangles[i + 2] = tris[i + 0];
        } // front vertices
        count_tris += tris.Length;
        for (int i = 0; i < tris.Length; i += 3)
        {
            triangles[count_tris + i + 2] = tris[i + 2] + poly.Length;
            triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
            triangles[count_tris + i + 0] = tris[i] + poly.Length;
        } // back vertices
        count_tris += tris.Length;
        for (int i = 0; i < poly.Length; i++)
        {
            // triangles around the perimeter of the object
            int n = (i + 1) % poly.Length;
            triangles[count_tris + 0] = i;
            triangles[count_tris + 1] = n;
            triangles[count_tris + 2] = i + poly.Length;
            triangles[count_tris + 3] = n;
            triangles[count_tris + 4] = n + poly.Length;
            triangles[count_tris + 5] = i + poly.Length;
            count_tris += 6;
        }
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds(); ;
        return m;

    }

    private static float[] getCornerCoordinatesFromPixel(int xIndex, int yIndex, int pixelWidth, int pixelHeight, float xTerrain, float yTerrain, float lengthTerrain, float widthTerrain)
    {
        float[] coord = new float[2];
        if (lengthTerrain <= widthTerrain)
        {
            coord[0] = (float)((float)(yIndex) / (float)(pixelHeight - 1)) * Math.Min(widthTerrain, lengthTerrain) + Math.Max(xTerrain, yTerrain);
            coord[1] = (float)((float)(xIndex) / (float)(pixelWidth - 1)) * Math.Max(widthTerrain, lengthTerrain) + Math.Min(xTerrain, yTerrain);
        }
        else
        {
            coord[0] = (float)((float)(yIndex) / (float)(pixelHeight - 1)) * Math.Max(widthTerrain, lengthTerrain) + Math.Min(xTerrain, yTerrain);
            coord[1] = (float)((float)(xIndex) / (float)(pixelWidth - 1)) * Math.Min(widthTerrain, lengthTerrain) + Math.Max(xTerrain, yTerrain);
        }
        return coord;
    }

}
