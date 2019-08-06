using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class CarEngineTests
{

    private List<Type> engineTypeList;

    public CarEngineTests()
    {
        engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(carEngine12));
        engineTypeList.Add(typeof(CarEngine2));
        engineTypeList.Add(typeof(CarEngine4));
        engineTypeList.Add(typeof(CarEngine5));
        engineTypeList.Add(typeof(CarEngine6));
        engineTypeList.Add(typeof(newCarEngine2));
        engineTypeList.Add(typeof(VehicleEngine));
        engineTypeList.Add(typeof(VehicleEngine1));
        engineTypeList.Add(typeof(VehicleEngine3));

    }

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

    public Rigidbody SpawnCar(CarFactory carFactory, carFactory2 carFactory2, carfactory3 carfactory3, carfactory4 carfactory4, Type carEngineScriptType)
    {

        if (carFactory.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory.car1, carFactory.spawnSpot1, Quaternion.Euler(Vector3.up * 90));
        }
        if (carFactory.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory.car3, carFactory.spawnSpot3, Quaternion.Euler(Vector3.up * 90));
        }
        if (carFactory2.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory2.car1, carFactory2.spawnSpot1, Quaternion.Euler(Vector3.up * 270));
        }
        if (carFactory2.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carFactory2.car3, carFactory2.spawnSpot3, Quaternion.Euler(Vector3.up * 270));
        }
        if (carfactory3.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carfactory3.car1, carfactory3.spawnSpot1, Quaternion.identity);
        }
        if (carfactory3.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carfactory3.car3, carfactory3.spawnSpot3, Quaternion.identity);
        }
        if (carfactory4.car1.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carfactory4.car1, carfactory4.spawnSpot1, Quaternion.Euler(Vector3.up * 180));
        }
        if (carfactory4.car3.gameObject.GetComponent(carEngineScriptType) != null)
        {
            return GameObject.Instantiate(carfactory4.car3, carfactory4.spawnSpot3, Quaternion.Euler(Vector3.up * 180));
        }

        return null;
    }

    [UnityTest]
    public IEnumerator CarEngineGoTest()
    {

        yield return null;

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        carFactory2 carFactory2 = (carFactory2)GameObject.FindObjectOfType(typeof(carFactory2));
        carfactory3 carfactory3 = (carfactory3)GameObject.FindObjectOfType(typeof(carfactory3));
        carfactory4 carfactory4 = (carfactory4)GameObject.FindObjectOfType(typeof(carfactory4));

        foreach (Type engineType in engineTypeList)
        {

            yield return null;

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            yield return null;

            if (engineType == typeof(carEngine12))
            {
                carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
                Assert.AreEqual(carEngine12.maxMotorTorque, carEngine12.WheelFL.motorTorque);
                Assert.AreEqual(carEngine12.maxMotorTorque, carEngine12.WheelFR.motorTorque);
                Assert.AreEqual(0, carEngine12.WheelFL.brakeTorque);
                Assert.AreEqual(0, carEngine12.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine2))
            {
                CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(carEngine2.maxMotorTorque, carEngine2.WheelFL.motorTorque);
                Assert.AreEqual(carEngine2.maxMotorTorque, carEngine2.WheelFR.motorTorque);
                Assert.AreEqual(0, carEngine2.WheelFL.brakeTorque);
                Assert.AreEqual(0, carEngine2.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine4))
            {
                CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
                Assert.AreEqual(carEngine4.maxMotorTorque, carEngine4.WheelFL.motorTorque);
                Assert.AreEqual(carEngine4.maxMotorTorque, carEngine4.WheelFR.motorTorque);
                Assert.AreEqual(0, carEngine4.WheelFL.brakeTorque);
                Assert.AreEqual(0, carEngine4.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine5))
            {
                CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
                Assert.AreEqual(carEngine5.maxMotorTorque, carEngine5.WheelFL.motorTorque);
                Assert.AreEqual(carEngine5.maxMotorTorque, carEngine5.WheelFR.motorTorque);
                Assert.AreEqual(0, carEngine5.WheelFL.brakeTorque);
                Assert.AreEqual(0, carEngine5.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine6))
            {
                CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
                Assert.AreEqual(carEngine6.maxMotorTorque, carEngine6.WheelFL.motorTorque);
                Assert.AreEqual(carEngine6.maxMotorTorque, carEngine6.WheelFR.motorTorque);
                Assert.AreEqual(0, carEngine6.WheelFL.brakeTorque);
                Assert.AreEqual(0, carEngine6.WheelFR.brakeTorque);
            }

            if (engineType == typeof(newCarEngine2))
            {
                newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(newCarEngine2.maxMotorTorque, newCarEngine2.WheelFL.motorTorque);
                Assert.AreEqual(newCarEngine2.maxMotorTorque, newCarEngine2.WheelFR.motorTorque);
                Assert.AreEqual(0, newCarEngine2.WheelFL.brakeTorque);
                Assert.AreEqual(0, newCarEngine2.WheelFR.brakeTorque);
            }

            if (engineType == typeof(VehicleEngine))
            {
                VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
                Assert.AreEqual(vehicleEngine.maxMotorTorque, vehicleEngine.WheelFL.motorTorque);
                Assert.AreEqual(vehicleEngine.maxMotorTorque, vehicleEngine.WheelFR.motorTorque);
                Assert.AreEqual(0, vehicleEngine.WheelFL.brakeTorque);
                Assert.AreEqual(0, vehicleEngine.WheelFR.brakeTorque);
            }

            if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                Assert.AreEqual(vehicleEngine1.maxMotorTorque, vehicleEngine1.WheelFL.motorTorque);
                Assert.AreEqual(vehicleEngine1.maxMotorTorque, vehicleEngine1.WheelFR.motorTorque);
                Assert.AreEqual(0, vehicleEngine1.WheelFL.brakeTorque);
                Assert.AreEqual(0, vehicleEngine1.WheelFR.brakeTorque);
            }

            if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
                Assert.AreEqual(vehicleEngine3.maxMotorTorque, vehicleEngine3.WheelFL.motorTorque);
                Assert.AreEqual(vehicleEngine3.maxMotorTorque, vehicleEngine3.WheelFR.motorTorque);
                Assert.AreEqual(0, vehicleEngine3.WheelFL.brakeTorque);
                Assert.AreEqual(0, vehicleEngine3.WheelFR.brakeTorque);
            }

            GameObject.Destroy(car);

        }

    }

    [UnityTest]
    public IEnumerator CarEngineStopTest()
    {

        yield return null;

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        carFactory2 carFactory2 = (carFactory2)GameObject.FindObjectOfType(typeof(carFactory2));
        carfactory3 carfactory3 = (carfactory3)GameObject.FindObjectOfType(typeof(carfactory3));
        carfactory4 carfactory4 = (carfactory4)GameObject.FindObjectOfType(typeof(carfactory4));

        foreach (Type engineType in engineTypeList)
        {

            yield return null;

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            car.tag = "hap";

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            if (engineType == typeof(carEngine12))
            {
                carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
                Assert.AreEqual(0, carEngine12.WheelFL.motorTorque);
                Assert.AreEqual(0, carEngine12.WheelFR.motorTorque);
                Assert.AreEqual(carEngine12.maxBrakeTorque, carEngine12.WheelFL.brakeTorque);
                Assert.AreEqual(carEngine12.maxBrakeTorque, carEngine12.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine2))
            {
                CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(0, carEngine2.WheelFL.motorTorque);
                Assert.AreEqual(0, carEngine2.WheelFR.motorTorque);
                Assert.AreEqual(carEngine2.maxBrakeTorque, carEngine2.WheelFL.brakeTorque);
                Assert.AreEqual(carEngine2.maxBrakeTorque, carEngine2.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine4))
            {
                CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
                Assert.AreEqual(0, carEngine4.WheelFL.motorTorque);
                Assert.AreEqual(0, carEngine4.WheelFR.motorTorque);
                Assert.AreEqual(carEngine4.maxBrakeTorque, carEngine4.WheelFL.brakeTorque);
                Assert.AreEqual(carEngine4.maxBrakeTorque, carEngine4.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine5))
            {
                CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
                Assert.AreEqual(0, carEngine5.WheelFL.motorTorque);
                Assert.AreEqual(0, carEngine5.WheelFR.motorTorque);
                Assert.AreEqual(carEngine5.maxBrakeTorque, carEngine5.WheelFL.brakeTorque);
                Assert.AreEqual(carEngine5.maxBrakeTorque, carEngine5.WheelFR.brakeTorque);
            }

            if (engineType == typeof(CarEngine6))
            {
                CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
                Assert.AreEqual(0, carEngine6.WheelFL.motorTorque);
                Assert.AreEqual(0, carEngine6.WheelFR.motorTorque);
                Assert.AreEqual(carEngine6.maxBrakeTorque, carEngine6.WheelFL.brakeTorque);
                Assert.AreEqual(carEngine6.maxBrakeTorque, carEngine6.WheelFR.brakeTorque);
            }

            if (engineType == typeof(newCarEngine2))
            {
                newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
                Assert.AreEqual(0, newCarEngine2.WheelFL.motorTorque);
                Assert.AreEqual(0, newCarEngine2.WheelFR.motorTorque);
                Assert.AreEqual(newCarEngine2.maxBrakeTorque, newCarEngine2.WheelFL.brakeTorque);
                Assert.AreEqual(newCarEngine2.maxBrakeTorque, newCarEngine2.WheelFR.brakeTorque);
            }

            /*
            if (engineType == typeof(VehicleEngine))
            {
                VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
                Assert.AreEqual(0, vehicleEngine.WheelFL.motorTorque);
                Assert.AreEqual(0, vehicleEngine.WheelFR.motorTorque);
                Assert.AreEqual(vehicleEngine.maxBrakeTorque, vehicleEngine.WheelFL.brakeTorque);
                Assert.AreEqual(vehicleEngine.maxBrakeTorque, vehicleEngine.WheelFR.brakeTorque);
            }
            */

            /*
            if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                Assert.AreEqual(0, vehicleEngine1.WheelFL.motorTorque);
                Assert.AreEqual(0, vehicleEngine1.WheelFR.motorTorque);
                Assert.AreEqual(vehicleEngine1.maxBrakeTorque, vehicleEngine1.WheelFL.brakeTorque);
                Assert.AreEqual(vehicleEngine1.maxBrakeTorque, vehicleEngine1.WheelFR.brakeTorque);
            }
            */

            if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
                Assert.AreEqual(0, vehicleEngine3.WheelFL.motorTorque);
                Assert.AreEqual(0, vehicleEngine3.WheelFR.motorTorque);
                Assert.AreEqual(vehicleEngine3.maxBrakeTorque, vehicleEngine3.WheelFL.brakeTorque);
                Assert.AreEqual(vehicleEngine3.maxBrakeTorque, vehicleEngine3.WheelFR.brakeTorque);
            }

            GameObject.Destroy(car);

        }

    }

}
