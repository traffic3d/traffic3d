using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class VehicleEngineTests
{

    public const int STOP_LIGHT_TIME = 20;
    public const int TIME_OUT_DESTROY_TIME = 60;
    public const int TIME_OUT_STOP_TIME = 60;

    private List<Type> engineTypeList;

    public VehicleEngineTests()
    {
        engineTypeList = new List<Type>();
        engineTypeList.Add(typeof(VehicleEngine8));
        engineTypeList.Add(typeof(VehicleEngine3));
        engineTypeList.Add(typeof(VehicleEngine4));
        engineTypeList.Add(typeof(VehicleEngine5));
        engineTypeList.Add(typeof(VehicleEngine6));
        engineTypeList.Add(typeof(VehicleEngine7));
        engineTypeList.Add(typeof(VehicleEngine2));
        engineTypeList.Add(typeof(VehicleEngine1));

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

    [TearDown]
    public void TearDown()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));
    }

    public Rigidbody SpawnCar(VehicleFactory vehicleFactory, Type carEngineScriptType)
    {

        return vehicleFactory.SpawnVehicle(carEngineScriptType, vehicleFactory.GetRandomUnusedPath());

    }

    [UnityTest]
    public IEnumerator CarEngineGoTest()
    {

        yield return null;

        DisableLoops();

        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));

        foreach (Type engineType in engineTypeList)
        {

            yield return new WaitForSeconds(1);

            Rigidbody car = SpawnCar(vehicleFactory, engineType);

            // Car may not be being used by factory.
            if (car == null)
            {
                Debug.Log("Car engine not in use: " + engineType);
                continue;
            }

            yield return null;

            Assert.True(GetCarStatus(car, engineType) == CarStatus.DRIVING);

            GameObject.Destroy(car);

        }

    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 20)]
    public IEnumerator CarEngineStopTest()
    {

        yield return null;

        DisableLoops();

        VehicleFactory carFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));

        TrafficLightManager.GetInstance().SetAllToRed();

        foreach (Type engineType in engineTypeList)
        {

            Rigidbody car = SpawnCar(carFactory, engineType);

            yield return null;

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            yield return new WaitForSeconds(STOP_LIGHT_TIME);

            Assert.True(GetCarStatus(car, engineType) == CarStatus.STOPPED);

            // Move out of scene as destroy doesn't seem to destroy properly.
            car.transform.position = new Vector3(0, -1000, 0);
            GameObject.Destroy(car);

            yield return new WaitForSeconds(5);

        }

    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 20)]
    public IEnumerator CarEngineDestroyTest()
    {

        yield return null;

        DisableLoops();

        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));

        Dictionary<Type, Rigidbody> carList = new Dictionary<Type, Rigidbody>();

        foreach(TrafficLight trafficLight in TrafficLightManager.GetInstance().trafficLights)
        {
            trafficLight.SetColour(TrafficLight.LightColour.GREEN);
        }

        foreach (Type engineType in engineTypeList)
        {

            Rigidbody car = SpawnCar(vehicleFactory, engineType);

            yield return null;

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            carList.Add(engineType, car);

            bool carIsDestroyed = false;
            for(int i = 0; i <= TIME_OUT_DESTROY_TIME; i = i + 5)
            {

                yield return new WaitForSeconds(5);

                // Check if car is destroyed
                if(car == null)
                {
                    carIsDestroyed = true;
                    break;
                }

            }

            Assert.True(carIsDestroyed);

        }

    }

    private void DisableLoops()
    {

        // Optimize time by removing unneeded particles
        foreach (ParticleSystem particleSystem in GameObject.FindObjectsOfType<ParticleSystem>())
        {
            particleSystem.Stop();
        }

        TrafficLightManager trafficLightManager = (TrafficLightManager) GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.enabled = false;
        trafficLightManager.StopAllCoroutines();

        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        vehicleFactory.StopAllCoroutines();

    }

    private CarStatus GetCarStatus(Rigidbody car, Type engineType)
    {

        if (engineType == typeof(VehicleEngine8))
        {
            VehicleEngine8 vehicleEngine8 = (VehicleEngine8)car.GetComponent(engineType);
            if (vehicleEngine8.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine8.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine8.maxBrakeTorque == vehicleEngine8.wheelColliderFrontLeft.brakeTorque && vehicleEngine8.maxBrakeTorque == vehicleEngine8.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine3))
        {
            VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
            if (vehicleEngine3.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine3.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine3.maxBrakeTorque == vehicleEngine3.wheelColliderFrontLeft.brakeTorque && vehicleEngine3.maxBrakeTorque == vehicleEngine3.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine4))
        {
            VehicleEngine4 vehicleEngine4 = (VehicleEngine4)car.GetComponent(engineType);
            if (vehicleEngine4.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine4.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine4.maxBrakeTorque == vehicleEngine4.wheelColliderFrontLeft.brakeTorque && vehicleEngine4.maxBrakeTorque == vehicleEngine4.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine5))
        {
            VehicleEngine5 vehicleEngine5 = (VehicleEngine5)car.GetComponent(engineType);
            if (vehicleEngine5.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine5.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine5.maxBrakeTorque == vehicleEngine5.wheelColliderFrontLeft.brakeTorque && vehicleEngine5.maxBrakeTorque == vehicleEngine5.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine6))
        {
            VehicleEngine6 vehicleEngine6 = (VehicleEngine6)car.GetComponent(engineType);
            if (vehicleEngine6.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine6.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine6.maxBrakeTorque == vehicleEngine6.wheelColliderFrontLeft.brakeTorque && vehicleEngine6.maxBrakeTorque == vehicleEngine6.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine7))
        {
            VehicleEngine7 VehicleEngine7 = (VehicleEngine7)car.GetComponent(engineType);
            if (VehicleEngine7.wheelColliderFrontLeft.motorTorque == 0 && VehicleEngine7.wheelColliderFrontRight.motorTorque == 0 &&
                VehicleEngine7.maxBrakeTorque == VehicleEngine7.wheelColliderFrontLeft.brakeTorque && VehicleEngine7.maxBrakeTorque == VehicleEngine7.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine2))
        {
            VehicleEngine2 vehicleEngine2 = (VehicleEngine2)car.GetComponent(engineType);
            if (vehicleEngine2.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine2.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine2.maxBrakeTorque == vehicleEngine2.wheelColliderFrontLeft.brakeTorque && vehicleEngine2.maxBrakeTorque == vehicleEngine2.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine1))
        {
            VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
            if (vehicleEngine1.wheelColliderFrontLeft.motorTorque == 0 && vehicleEngine1.wheelColliderFrontRight.motorTorque == 0 &&
                vehicleEngine1.maxBrakeTorque2 == vehicleEngine1.wheelColliderFrontLeft.brakeTorque && vehicleEngine1.maxBrakeTorque2 == vehicleEngine1.wheelColliderFrontRight.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine8))
        {
            VehicleEngine8 vehicleEngine8 = (VehicleEngine8)car.GetComponent(engineType);
            if (vehicleEngine8.maxMotorTorque == vehicleEngine8.wheelColliderFrontLeft.motorTorque && vehicleEngine8.maxMotorTorque == vehicleEngine8.wheelColliderFrontRight.motorTorque &&
                vehicleEngine8.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine8.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine3))
        {
            VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
            if (vehicleEngine3.maxMotorTorque == vehicleEngine3.wheelColliderFrontLeft.motorTorque && vehicleEngine3.maxMotorTorque == vehicleEngine3.wheelColliderFrontRight.motorTorque &&
                vehicleEngine3.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine3.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine4))
        {
            VehicleEngine4 vehicleEngine4 = (VehicleEngine4)car.GetComponent(engineType);
            if (vehicleEngine4.maxMotorTorque == vehicleEngine4.wheelColliderFrontLeft.motorTorque && vehicleEngine4.maxMotorTorque == vehicleEngine4.wheelColliderFrontRight.motorTorque &&
                vehicleEngine4.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine4.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine5))
        {
            VehicleEngine5 vehicleEngine5 = (VehicleEngine5)car.GetComponent(engineType);
            if (vehicleEngine5.maxMotorTorque == vehicleEngine5.wheelColliderFrontLeft.motorTorque && vehicleEngine5.maxMotorTorque == vehicleEngine5.wheelColliderFrontRight.motorTorque &&
                vehicleEngine5.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine5.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine6))
        {
            VehicleEngine6 vehicleEngine6 = (VehicleEngine6)car.GetComponent(engineType);
            if (vehicleEngine6.maxMotorTorque == vehicleEngine6.wheelColliderFrontLeft.motorTorque && vehicleEngine6.maxMotorTorque == vehicleEngine6.wheelColliderFrontRight.motorTorque &&
                vehicleEngine6.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine6.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine7))
        {
            VehicleEngine7 vehicleEngine7 = (VehicleEngine7)car.GetComponent(engineType);
            if (vehicleEngine7.maxMotorTorque == vehicleEngine7.wheelColliderFrontLeft.motorTorque && vehicleEngine7.maxMotorTorque == vehicleEngine7.wheelColliderFrontRight.motorTorque &&
                vehicleEngine7.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine7.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine2))
        {
            VehicleEngine2 vehicleEngine2 = (VehicleEngine2)car.GetComponent(engineType);
            if (vehicleEngine2.maxMotorTorque == vehicleEngine2.wheelColliderFrontLeft.motorTorque && vehicleEngine2.maxMotorTorque == vehicleEngine2.wheelColliderFrontRight.motorTorque &&
                vehicleEngine2.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine2.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine1))
        {
            VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
            if (vehicleEngine1.maxMotorTorque == vehicleEngine1.wheelColliderFrontLeft.motorTorque && vehicleEngine1.maxMotorTorque == vehicleEngine1.wheelColliderFrontRight.motorTorque &&
                vehicleEngine1.wheelColliderFrontLeft.brakeTorque == 0 && vehicleEngine1.wheelColliderFrontRight.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        return CarStatus.UNKNOWN;

    }

    public enum CarStatus
    {
        DRIVING,
        STOPPED,
        UNKNOWN
    }

}
