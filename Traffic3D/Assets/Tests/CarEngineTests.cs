using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class CarEngineTests
{

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public Rigidbody SpawnCar(CarFactory carFactory, carFactory2 carFactory2, Type carEngineScriptType)
    {

        if (carFactory.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory.car1, carFactory.spawnSpot1, Quaternion.Euler(Vector3.up * 45));
        }
        if (carFactory.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory.car3, carFactory.spawnSpot3, Quaternion.Euler(Vector3.up * 45));
        }
        if (carFactory2.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory2.car1, carFactory2.spawnSpot1, Quaternion.Euler(Vector3.up * 300));
        }
        if (carFactory2.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory2.car3, carFactory2.spawnSpot3, Quaternion.Euler(Vector3.up * 300));
        }

        return null;
    }

    [UnityTest]
    public IEnumerator CarEngineKeepGoingTest()
    {

        yield return null;

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        carFactory2 carFactory2 = (carFactory2)GameObject.FindObjectOfType(typeof(carFactory2));

        List<Type> engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(carEngine12));
        engineTypeList.Add(typeof(CarEngine2));
        engineTypeList.Add(typeof(CarEngine6));
        engineTypeList.Add(typeof(newCarEngine2));

        foreach (Type engineType in engineTypeList)
        {

            yield return null;

            Rigidbody car = SpawnCar(carFactory, carFactory2, engineType);
            car.tag = "rid";

            yield return null;
            
            if (engineType == typeof(carEngine12))
            {
                carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
                Assert.AreEqual(carEngine12.WheelFL.motorTorque, carEngine12.maxMotorTorque);
                Assert.AreEqual(carEngine12.WheelFR.motorTorque, carEngine12.maxMotorTorque);
                Assert.AreEqual(carEngine12.WheelFL.brakeTorque, 0);
                Assert.AreEqual(carEngine12.WheelFR.brakeTorque, 0);
            }

            if (engineType == typeof(CarEngine2))
            {
                CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(carEngine2.WheelFL.motorTorque, carEngine2.maxMotorTorque);
                Assert.AreEqual(carEngine2.WheelFR.motorTorque, carEngine2.maxMotorTorque);
                Assert.AreEqual(carEngine2.WheelFL.brakeTorque, 0);
                Assert.AreEqual(carEngine2.WheelFR.brakeTorque, 0);
            }

            if (engineType == typeof(CarEngine6))
            {
                CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
                Assert.AreEqual(carEngine6.WheelFL.motorTorque, carEngine6.maxMotorTorque);
                Assert.AreEqual(carEngine6.WheelFR.motorTorque, carEngine6.maxMotorTorque);
                Assert.AreEqual(carEngine6.WheelFL.brakeTorque, 0);
                Assert.AreEqual(carEngine6.WheelFR.brakeTorque, 0);
            }

            if (engineType == typeof(newCarEngine2))
            {
                newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(newCarEngine2.WheelFL.motorTorque, newCarEngine2.maxMotorTorque);
                Assert.AreEqual(newCarEngine2.WheelFR.motorTorque, newCarEngine2.maxMotorTorque);
                Assert.AreEqual(newCarEngine2.WheelFL.brakeTorque, 0);
                Assert.AreEqual(newCarEngine2.WheelFR.brakeTorque, 0);
            }

            GameObject.Destroy(car);

        }

    }

}
