using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class SimulationControllerTests
{
    [SetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    [Test]
    public void WindowDisplayTest()
    {
        SimulationControllerUI window = SimulationControllerUI.DisplayOsmGui();
        Assert.AreEqual(window, EditorWindow.focusedWindow);
        window.Close();
    }

    [UnityTest]
    public IEnumerator NoOptionsTest()
    {
        SimulationControllerUI window = SimulationControllerUI.DisplayOsmGui();
        yield return null;
        Assert.IsEmpty(window.GetDisplayedOptions());
        window.Close();
    }

    [UnityTest]
    public IEnumerator OneOptionsTest()
    {
        List<Type> defaultOptionList = new List<Type>();
        defaultOptionList.Add(typeof(VehicleFactory));
        defaultOptionList.Add(typeof(PedestrianFactory));
        defaultOptionList.Add(typeof(EnvironmentSettings));
        defaultOptionList.Add(typeof(GraphManager));

        GameObject vehicleFactory = new GameObject();
        vehicleFactory.AddComponent<VehicleFactory>();

        SimulationControllerUI window = SimulationControllerUI.DisplayOsmGui();
        yield return null;

        Assert.IsNotEmpty(window.GetDisplayedOptions());
        Assert.AreEqual(typeof(VehicleFactory), window.GetDisplayedOptions()[0]);

        GameObject.DestroyImmediate(vehicleFactory.gameObject);
    }

    [UnityTest]
    public IEnumerator MultipleOptionsTest()
    {
        List<Type> defaultOptionList = new List<Type>();
        defaultOptionList.Add(typeof(VehicleFactory));
        defaultOptionList.Add(typeof(PedestrianFactory));
        defaultOptionList.Add(typeof(EnvironmentSettings));
        defaultOptionList.Add(typeof(GraphManager));

        GameObject vehicleFactory = new GameObject();
        vehicleFactory.AddComponent<VehicleFactory>();
        GameObject pedestrianFactory = new GameObject();
        pedestrianFactory.AddComponent<PedestrianFactory>();
        GameObject environmentSettings = new GameObject();
        environmentSettings.AddComponent<EnvironmentSettings>();
        GameObject graphManager = new GameObject();
        graphManager.AddComponent<GraphManager>();

        SimulationControllerUI window = SimulationControllerUI.DisplayOsmGui();
        yield return null;
        Assert.IsNotEmpty(window.GetDisplayedOptions());
        Assert.AreEqual(defaultOptionList.Count, window.GetDisplayedOptions().Count);
        foreach(Type type in window.GetDisplayedOptions())
        {
            defaultOptionList.Remove(type);
        }
        Assert.IsEmpty(defaultOptionList);
        GameObject.DestroyImmediate(vehicleFactory);
        GameObject.DestroyImmediate(pedestrianFactory);
        GameObject.DestroyImmediate(environmentSettings);
        GameObject.DestroyImmediate(graphManager);
    }


}
