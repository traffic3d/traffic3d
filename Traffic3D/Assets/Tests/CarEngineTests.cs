using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class CarEngineTests
{

    public const int STOP_LIGHT_TIME = 20;
    public const int TIME_OUT_DESTROY_TIME = 60;

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
            if (engineType == typeof(VehicleEngine) || engineType == typeof(VehicleEngine1) || engineType == typeof(VehicleEngine3))
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

            if (engineType == typeof(carEngine12))
            {
                carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
                carEngine12.r.currentMaterial = carEngine12.r.redMaterial;
                carEngine12.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine2))
            {
                CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
                carEngine2.m.currentMaterial = carEngine2.m.redMaterial;
                carEngine2.m.enabled = false;
            }
            else if (engineType == typeof(CarEngine4))
            {
                CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
                carEngine4.r.currrentMaterial = carEngine4.r.redMaterial;
                carEngine4.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine5))
            {
                CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
                carEngine5.r.currrentMaterial = carEngine5.r.redMaterial;
                carEngine5.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine6))
            {
                CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
                carEngine6.p.currentMaterial = carEngine6.p.redMaterial;
                carEngine6.p.enabled = false;
            }
            else if (engineType == typeof(newCarEngine2))
            {
                newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
                newCarEngine2.q.currentMaterial = newCarEngine2.q.redMaterial;
                newCarEngine2.q.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine))
            {
                VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
                vehicleEngine.u.currentMaterial = vehicleEngine.u.redMaterial;
                vehicleEngine.u.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                vehicleEngine1.u.currentMaterial = vehicleEngine1.u.redMaterial;
                vehicleEngine1.u.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
                vehicleEngine3.m.currentMaterial = vehicleEngine3.m.redMaterial;
                vehicleEngine3.m.enabled = false;
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

            if (engineType == typeof(carEngine12))
            {
                carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
                carEngine12.r.currentMaterial = carEngine12.r.greenMaterial;
                carEngine12.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine2))
            {
                CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
                carEngine2.m.currentMaterial = carEngine2.m.greenMaterial;
                carEngine2.m.enabled = false;
            }
            else if (engineType == typeof(CarEngine4))
            {
                CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
                carEngine4.r.currrentMaterial = carEngine4.r.greenMaterial;
                carEngine4.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine5))
            {
                CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
                carEngine5.r.currrentMaterial = carEngine5.r.greenMaterial;
                carEngine5.r.enabled = false;
            }
            else if (engineType == typeof(CarEngine6))
            {
                CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
                carEngine6.p.currentMaterial = carEngine6.p.greenMaterial;
                carEngine6.p.enabled = false;
            }
            else if (engineType == typeof(newCarEngine2))
            {
                newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
                newCarEngine2.q.currentMaterial = newCarEngine2.q.greenMaterial;
                newCarEngine2.q.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine))
            {
                VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
                vehicleEngine.u.currentMaterial = vehicleEngine.u.greenMaterial;
                vehicleEngine.u.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine1))
            {
                VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
                vehicleEngine1.u.currentMaterial = vehicleEngine1.u.greenMaterial;
                vehicleEngine1.u.enabled = false;
            }
            else if (engineType == typeof(VehicleEngine3))
            {
                VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
                vehicleEngine3.m.currentMaterial = vehicleEngine3.m.greenMaterial;
                vehicleEngine3.m.enabled = false;
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

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));
        redlight.enabled = false;
        redlight.StopAllCoroutines();

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

        if (engineType == typeof(carEngine12))
        {
            carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
            if (carEngine12.WheelFL.motorTorque == 0 && carEngine12.WheelFR.motorTorque == 0 &&
                carEngine12.maxBrakeTorque == carEngine12.WheelFL.brakeTorque && carEngine12.maxBrakeTorque == carEngine12.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(CarEngine2))
        {
            CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
            if (carEngine2.WheelFL.motorTorque == 0 && carEngine2.WheelFR.motorTorque == 0 &&
                carEngine2.maxBrakeTorque == carEngine2.WheelFL.brakeTorque && carEngine2.maxBrakeTorque == carEngine2.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(CarEngine4))
        {
            CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
            if (carEngine4.WheelFL.motorTorque == 0 && carEngine4.WheelFR.motorTorque == 0 &&
                carEngine4.maxBrakeTorque == carEngine4.WheelFL.brakeTorque && carEngine4.maxBrakeTorque == carEngine4.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(CarEngine5))
        {
            CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
            if (carEngine5.WheelFL.motorTorque == 0 && carEngine5.WheelFR.motorTorque == 0 &&
                carEngine5.maxBrakeTorque == carEngine5.WheelFL.brakeTorque && carEngine5.maxBrakeTorque == carEngine5.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(CarEngine6))
        {
            CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
            if (carEngine6.WheelFL.motorTorque == 0 && carEngine6.WheelFR.motorTorque == 0 &&
                carEngine6.maxBrakeTorque == carEngine6.WheelFL.brakeTorque && carEngine6.maxBrakeTorque == carEngine6.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(newCarEngine2))
        {
            newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
            if (newCarEngine2.WheelFL.motorTorque == 0 && newCarEngine2.WheelFR.motorTorque == 0 &&
                newCarEngine2.maxBrakeTorque == newCarEngine2.WheelFL.brakeTorque && newCarEngine2.maxBrakeTorque == newCarEngine2.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine))
        {
            VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
            if (vehicleEngine.WheelFL.motorTorque == 0 && vehicleEngine.WheelFR.motorTorque == 0 &&
                vehicleEngine.maxBrakeTorque == vehicleEngine.WheelFL.brakeTorque && vehicleEngine.maxBrakeTorque == vehicleEngine.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine1))
        {
            VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
            if (vehicleEngine1.WheelFL.motorTorque == 0 && vehicleEngine1.WheelFR.motorTorque == 0 &&
                vehicleEngine1.maxBrakeTorque2 == vehicleEngine1.WheelFL.brakeTorque && vehicleEngine1.maxBrakeTorque2 == vehicleEngine1.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(VehicleEngine3))
        {
            VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
            if (vehicleEngine3.WheelFL.motorTorque == 0 && vehicleEngine3.WheelFR.motorTorque == 0 &&
                vehicleEngine3.maxBrakeTorque == vehicleEngine3.WheelFL.brakeTorque && vehicleEngine3.maxBrakeTorque == vehicleEngine3.WheelFR.brakeTorque)
            {
                return CarStatus.STOPPED;
            }
        }

        if (engineType == typeof(carEngine12))
        {
            carEngine12 carEngine12 = (carEngine12)car.GetComponent(engineType);
            if (carEngine12.maxMotorTorque == carEngine12.WheelFL.motorTorque && carEngine12.maxMotorTorque == carEngine12.WheelFR.motorTorque &&
                carEngine12.WheelFL.brakeTorque == 0 && carEngine12.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(CarEngine2))
        {
            CarEngine2 carEngine2 = (CarEngine2)car.GetComponent(engineType);
            if (carEngine2.maxMotorTorque == carEngine2.WheelFL.motorTorque && carEngine2.maxMotorTorque == carEngine2.WheelFR.motorTorque &&
                carEngine2.WheelFL.brakeTorque == 0 && carEngine2.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(CarEngine4))
        {
            CarEngine4 carEngine4 = (CarEngine4)car.GetComponent(engineType);
            if (carEngine4.maxMotorTorque == carEngine4.WheelFL.motorTorque && carEngine4.maxMotorTorque == carEngine4.WheelFR.motorTorque &&
                carEngine4.WheelFL.brakeTorque == 0 && carEngine4.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(CarEngine5))
        {
            CarEngine5 carEngine5 = (CarEngine5)car.GetComponent(engineType);
            if (carEngine5.maxMotorTorque == carEngine5.WheelFL.motorTorque && carEngine5.maxMotorTorque == carEngine5.WheelFR.motorTorque &&
                carEngine5.WheelFL.brakeTorque == 0 && carEngine5.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(CarEngine6))
        {
            CarEngine6 carEngine6 = (CarEngine6)car.GetComponent(engineType);
            if (carEngine6.maxMotorTorque == carEngine6.WheelFL.motorTorque && carEngine6.maxMotorTorque == carEngine6.WheelFR.motorTorque &&
                carEngine6.WheelFL.brakeTorque == 0 && carEngine6.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(newCarEngine2))
        {
            newCarEngine2 newCarEngine2 = (newCarEngine2)car.GetComponent(engineType);
            if (newCarEngine2.maxMotorTorque == newCarEngine2.WheelFL.motorTorque && newCarEngine2.maxMotorTorque == newCarEngine2.WheelFR.motorTorque &&
                newCarEngine2.WheelFL.brakeTorque == 0 && newCarEngine2.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine))
        {
            VehicleEngine vehicleEngine = (VehicleEngine)car.GetComponent(engineType);
            if (vehicleEngine.maxMotorTorque == vehicleEngine.WheelFL.motorTorque && vehicleEngine.maxMotorTorque == vehicleEngine.WheelFR.motorTorque &&
                vehicleEngine.WheelFL.brakeTorque == 0 && vehicleEngine.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine1))
        {
            VehicleEngine1 vehicleEngine1 = (VehicleEngine1)car.GetComponent(engineType);
            if (vehicleEngine1.maxMotorTorque == vehicleEngine1.WheelFL.motorTorque && vehicleEngine1.maxMotorTorque == vehicleEngine1.WheelFR.motorTorque &&
                vehicleEngine1.WheelFL.brakeTorque == 0 && vehicleEngine1.WheelFR.brakeTorque == 0)
            {
                return CarStatus.DRIVING;
            }
        }

        if (engineType == typeof(VehicleEngine3))
        {
            VehicleEngine3 vehicleEngine3 = (VehicleEngine3)car.GetComponent(engineType);
            if (vehicleEngine3.maxMotorTorque == vehicleEngine3.WheelFL.motorTorque && vehicleEngine3.maxMotorTorque == vehicleEngine3.WheelFR.motorTorque &&
                vehicleEngine3.WheelFL.brakeTorque == 0 && vehicleEngine3.WheelFR.brakeTorque == 0)
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
