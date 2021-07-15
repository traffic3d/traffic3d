using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

[Category("Tests")]
public class StopLineTests
{
    private readonly string mapFile = "Assets/newYork.txt";
    private OpenStreetMapReader osmMapReader;
    private PathGenerator pathGenerator;

    private List<Vector3> stopLinePositionsInNewYork = new List<Vector3>() {
        new Vector3(534.4388f, 1f, -172.5799f),
        new Vector3(-419.9208f, 1f, 263.5909f),
        new Vector3(-416.8399f, 1f, -192.4006f)
    };
    private List<Vector3> nonStopLinePositionsInNewYork = new List<Vector3>() {
        // Cross road positions
        new Vector3(499.8731f, 1f, 129.8849f),
        new Vector3(118.289f, 1f, 387.9873f),
        new Vector3(141.3472f, 1f, -223.3162f),
        // Start of roads
        new Vector3(-543.4092f, 1f, 369.2653f),
        new Vector3(-89.21917f, 1f, 407.489f)
    };
    private const float stopLineErrorAllowance = 0.1f;
    private const int numOfStopLinesInNewYork = 34;

    [SetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        osmMapReader = new OpenStreetMapReader();
        osmMapReader.ImportFile(mapFile);

        RoadGenerator roadGenerator = new RoadGenerator(osmMapReader, null);
        roadGenerator.GenerateRoads();
        pathGenerator = new PathGenerator(osmMapReader, true);
        pathGenerator.AddPathsToRoads(roadGenerator.GetWayObjects());
        RoadNetworkManager.GetInstance().Reload();
    }

    /// <summary>
    /// Check whether a stop line has been placed in a correct position.
    /// </summary>
    [Test]
    public void CorrectPlacementOfStopLineInMap()
    {
        foreach(Vector3 position in stopLinePositionsInNewYork)
        {
            Assert.True(
                RoadNetworkManager.GetInstance().GetNodes().Exists(r => r.GetComponent<StopLine>() != null && IsWithinDistance(position, r.transform.position)),
                "No stop line in correct location - Correct Location: " + position.ToString()
            );
        }
    }

    /// <summary>
    /// Check whether stop lines have been placed in incorrect positions.
    /// </summary>
    [Test]
    public void CheckForIncorrectPlacementOfStopLinesInMap()
    {
        foreach (Vector3 position in nonStopLinePositionsInNewYork)
        {
            Assert.True(
                RoadNetworkManager.GetInstance().GetNodes().Exists(r => r.GetComponent<StopLine>() == null && IsWithinDistance(position, r.transform.position)),
                "A stop line is in an incorrect location - Incorrect Location: " + position.ToString()
            );
        }
    }

    /// <summary>
    /// Checks count of stop lines within the map.
    /// </summary>
    [Test]
    public void CorrectNumberOfStopLinesInMap()
    {
        Assert.AreEqual(numOfStopLinesInNewYork, RoadNetworkManager.GetInstance().GetNodes().Count(r => r.GetComponent<StopLine>() != null));
    }

    /// <summary>
    /// Checks if a position is within another position with an allowance of error `stopLineErrorAllowance`.
    /// </summary>
    /// <param name="origin">The origin position</param>
    /// <param name="positionToCheck">The position to check</param>
    /// <returns>True if the origin is within the position to check with an allowance of error `stopLineErrorAllowance`.</returns>
    private bool IsWithinDistance(Vector3 origin, Vector3 positionToCheck)
    {
        return Vector3.Distance(origin, positionToCheck) < stopLineErrorAllowance;
    }
}
