using NUnit.Framework;
using UnityEngine;

[Category("Tests")]
public class PedestrianPathCreatorTests
{
    private const float tolerance = 0.005f;
    private PedestrianPathCreator pedestrianPathCreator;

    [SetUp]
    public void SetUp()
    {
        GameObject pedestrianPathCreatorObj = GameObject.Instantiate(new GameObject());
        pedestrianPathCreatorObj.AddComponent<PedestrianPathCreator>();
        pedestrianPathCreator = pedestrianPathCreatorObj.GetComponent<PedestrianPathCreator>();
    }
}
