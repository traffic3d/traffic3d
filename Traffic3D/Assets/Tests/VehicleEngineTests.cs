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

    public Rigidbody SpawnCar(CarFactory carFactory, CarFactory2 carFactory2, CarFactory3 carfactory3, CarFactory4 carfactory4, Type carEngineScriptType)
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

        DisableLoops();

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        CarFactory2 carFactory2 = (CarFactory2)GameObject.FindObjectOfType(typeof(CarFactory2));
        CarFactory3 carfactory3 = (CarFactory3)GameObject.FindObjectOfType(typeof(CarFactory3));
        CarFactory4 carfactory4 = (CarFactory4)GameObject.FindObjectOfType(typeof(CarFactory4));

        foreach (Type engineType in engineTypeList)
        {

            yield return null;

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

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
    public IEnumerator CarEngineEngineOffTest()
    {

        yield return null;

        DisableLoops();

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        CarFactory2 carFactory2 = (CarFactory2)GameObject.FindObjectOfType(typeof(CarFactory2));
        CarFactory3 carfactory3 = (CarFactory3)GameObject.FindObjectOfType(typeof(CarFactory3));
        CarFactory4 carfactory4 = (CarFactory4)GameObject.FindObjectOfType(typeof(CarFactory4));

        foreach (Type engineType in engineTypeList)
        {

            // Vehicle Engine Scripts do not support engine off method.
            if (engineType == typeof(VehicleEngine2) || engineType == typeof(VehicleEngine1))
            {
                continue;
            }

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            car.tag = "hap";

            yield return new WaitForFixedUpdate();

            Assert.True(GetCarStatus(car, engineType) == CarStatus.STOPPED);

            GameObject.Destroy(car);

        }

    }

    [UnityTest]
    public IEnumerator CarEngineStopTest()
    {

        yield return null;

        DisableLoops();

        Debug.Log("CarEngineStopTest - start");

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        CarFactory2 carFactory2 = (CarFactory2)GameObject.FindObjectOfType(typeof(CarFactory2));
        CarFactory3 carfactory3 = (CarFactory3)GameObject.FindObjectOfType(typeof(CarFactory3));
        CarFactory4 carfactory4 = (CarFactory4)GameObject.FindObjectOfType(typeof(CarFactory4));

        Dictionary<Type, Rigidbody> carList = new Dictionary<Type, Rigidbody>();

        foreach (Type engineType in engineTypeList)
        {

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

            yield return null;

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            carList.Add(engineType, car);

            if (engineType == typeof(VehicleEngine8))
            {
                VehicleEngine8 vehicleEngine9 = (VehicleEngine8)car.GetComponent(engineType);
                vehicleEngine9.trafficLightRed2.currentMaterial = vehicleEngine9.trafficLightRed2.redMaterial;
                vehicleEngine9.trafficLightRed2.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine7 = (VehicleEngine3)car.GetComponent(engineType);
                vehicleEngine7.trafficLightRed1.currentMaterial = vehicleEngine7.trafficLightRed1.redMaterial;
                vehicleEngine7.trafficLightRed1.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine4))
            {
                VehicleEngine4 vehicleEngine4 = (VehicleEngine4)car.GetComponent(engineType);
                vehicleEngine4.trafficLightRed4.currentMaterial = vehicleEngine4.trafficLightRed4.redMaterial;
                vehicleEngine4.trafficLightRed4.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine5))
            {
                VehicleEngine5 vehicleEngine5 = (VehicleEngine5)car.GetComponent(engineType);
                vehicleEngine5.trafficLightRed4.currentMaterial = vehicleEngine5.trafficLightRed4.redMaterial;
                vehicleEngine5.trafficLightRed4.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine6))
            {
                VehicleEngine6 vehicleEngine6 = (VehicleEngine6)car.GetComponent(engineType);
                vehicleEngine6.trafficLightRed2.currentMaterial = vehicleEngine6.trafficLightRed2.redMaterial;
                vehicleEngine6.trafficLightRed2.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine7))
            {
                VehicleEngine7 VehicleEngine7 = (VehicleEngine7)car.GetComponent(engineType);
                VehicleEngine7.trafficLightRed1.currentMaterial = VehicleEngine7.trafficLightRed1.redMaterial;
                VehicleEngine7.trafficLightRed1.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine2))
            {
                VehicleEngine2 vehicleEngine2 = (VehicleEngine2)car.GetComponent(engineType);
                vehicleEngine2.trafficLightRed3.currentMaterial = vehicleEngine2.trafficLightRed3.redMaterial;
                vehicleEngine2.trafficLightRed3.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                vehicleEngine1.trafficLightRed3.currentMaterial = vehicleEngine1.trafficLightRed3.redMaterial;
                vehicleEngine1.trafficLightRed3.enabled = false;
            }

            Debug.Log("First Status: " + GetCarStatus(car, engineType) + " - " + engineType);

        }

        yield return new WaitForSeconds(STOP_LIGHT_TIME);

        foreach (KeyValuePair<Type, Rigidbody> entry in carList)
        {
            CarStatus carStatus = GetCarStatus(entry.Value, entry.Key);
            Debug.Log("Last Status: " + carStatus + " - " + entry.Key);
            Assert.True(carStatus == CarStatus.STOPPED);
            GameObject.Destroy(entry.Value);
        }

        Debug.Log("CarEngineStopTest - end");

    }

    [UnityTest]
    [Timeout(TIME_OUT_DESTROY_TIME * 1000 * 20)]
    public IEnumerator CarEngineDestroyTest()
    {

        yield return null;

        DisableLoops();

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        CarFactory2 carFactory2 = (CarFactory2)GameObject.FindObjectOfType(typeof(CarFactory2));
        CarFactory3 carfactory3 = (CarFactory3)GameObject.FindObjectOfType(typeof(CarFactory3));
        CarFactory4 carfactory4 = (CarFactory4)GameObject.FindObjectOfType(typeof(CarFactory4));

        Dictionary<Type, Rigidbody> carList = new Dictionary<Type, Rigidbody>();

        foreach (Type engineType in engineTypeList)
        {

            Rigidbody car = SpawnCar(carFactory, carFactory2, carfactory3, carfactory4, engineType);

            yield return null;

            // Car may not be being used by factory.
            if (car == null)
            {
                continue;
            }

            carList.Add(engineType, car);

            if (engineType == typeof(VehicleEngine8))
            {
                VehicleEngine8 vehicleEngine8 = (VehicleEngine8)car.GetComponent(engineType);
                vehicleEngine8.trafficLightRed2.currentMaterial = vehicleEngine8.trafficLightRed2.blackMaterial;
                vehicleEngine8.trafficLightRed2.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
                vehicleEngine3.trafficLightRed1.currentMaterial = vehicleEngine3.trafficLightRed1.blackMaterial;
                vehicleEngine3.trafficLightRed1.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine4))
            {
                VehicleEngine4 vehicleEngine4 = (VehicleEngine4)car.GetComponent(engineType);
                vehicleEngine4.trafficLightRed4.currentMaterial = vehicleEngine4.trafficLightRed4.blackMaterial;
                vehicleEngine4.trafficLightRed4.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine5))
            {
                VehicleEngine5 vehicleEngine5 = (VehicleEngine5)car.GetComponent(engineType);
                vehicleEngine5.trafficLightRed4.currentMaterial = vehicleEngine5.trafficLightRed4.blackMaterial;
                vehicleEngine5.trafficLightRed4.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine6))
            {
                VehicleEngine6 vehicleEngine6 = (VehicleEngine6)car.GetComponent(engineType);
                vehicleEngine6.trafficLightRed2.currentMaterial = vehicleEngine6.trafficLightRed2.blackMaterial;
                vehicleEngine6.trafficLightRed2.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine7))
            {
                VehicleEngine7 vehicleEngine7 = (VehicleEngine7)car.GetComponent(engineType);
                vehicleEngine7.trafficLightRed1.currentMaterial = vehicleEngine7.trafficLightRed1.blackMaterial;
                vehicleEngine7.trafficLightRed1.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine2))
            {
                VehicleEngine2 vehicleEngine2 = (VehicleEngine2)car.GetComponent(engineType);
                vehicleEngine2.trafficLightRed3.currentMaterial = vehicleEngine2.trafficLightRed3.blackMaterial;
                vehicleEngine2.trafficLightRed3.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                vehicleEngine1.trafficLightRed3.currentMaterial = vehicleEngine1.trafficLightRed3.blackMaterial;
                vehicleEngine1.trafficLightRed3.enabled = false;
            }

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

        CarFactory carFactory = (CarFactory)GameObject.FindObjectOfType(typeof(CarFactory));
        CarFactory2 carFactory2 = (CarFactory2)GameObject.FindObjectOfType(typeof(CarFactory2));
        CarFactory3 carfactory3 = (CarFactory3)GameObject.FindObjectOfType(typeof(CarFactory3));
        CarFactory4 carfactory4 = (CarFactory4)GameObject.FindObjectOfType(typeof(CarFactory4));
        carFactory.StopAllCoroutines();
        carFactory2.StopAllCoroutines();
        carfactory3.StopAllCoroutines();
        carfactory4.StopAllCoroutines();

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
