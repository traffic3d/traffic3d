using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory3 : MonoBehaviour
{

    public Rigidbody car1;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot3;
    public int carTypeSwitch;

    void Start()
    {
        carTypeSwitch = 0;
        StartCoroutine(GenerateCars());
    }

    IEnumerator GenerateCars()
    {

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(23, 33));

            if (CarFactoryCounter3.carCount < Random.Range(1, 3))
            {
                if (carTypeSwitch == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.identity);
                    carTypeSwitch = 1;
                    CarFactoryCounter3.IncrementCarCount();
                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.identity);
                    carTypeSwitch = 0;
                    CarFactoryCounter3.IncrementCarCount();
                }

            }


        }
    }

}