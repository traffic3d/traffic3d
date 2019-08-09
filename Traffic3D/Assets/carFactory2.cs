using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory2 : MonoBehaviour
{

    public Rigidbody car1;
    public Rigidbody car2;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot2;
    public Vector3 spawnSpot3;
    public int carTypeSwitch = 0;

    // Use this for initialization
    void Start()
    {
        Random.seed = 456;
        carTypeSwitch = 0;
        StartCoroutine(GenerateCars());
    }

    IEnumerator GenerateCars()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(35, 40));
            if (CarFactoryCounter2.carCount < Random.Range(1, 3))
            {
                if (carTypeSwitch == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 270));
                    CarFactoryCounter2.IncrementCarCount();
                    carTypeSwitch = 1;
                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 270));
                    CarFactoryCounter2.IncrementCarCount();
                    carTypeSwitch = 0;
                }

            }

        }

    }

}
