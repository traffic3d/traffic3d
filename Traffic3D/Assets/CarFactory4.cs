using System.Collections;
using UnityEngine;

public class CarFactory4 : MonoBehaviour
{

    public Rigidbody car1;
    public Rigidbody car3;
    public Vector3 spawnSpot1;
    public Vector3 spawnSpot3;
    int carTypeSwitch = 0;

    // Use this for initialization
    void Start()
    {
        Random.seed = 78;
        carTypeSwitch = 0;
        StartCoroutine(GenerateCars());
    }

    IEnumerator GenerateCars()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(25, 32));

            if (CarFactoryCounter4.carCount < Random.Range(1, 3))
            {
                if (carTypeSwitch == 0)
                {
                    Instantiate(car1, spawnSpot1, Quaternion.Euler(Vector3.up * 180));
                    carTypeSwitch = 1;
                    CarFactoryCounter4.IncrementCarCount();
                }
                else
                {
                    Instantiate(car3, spawnSpot3, Quaternion.Euler(Vector3.up * 180));
                    carTypeSwitch = 0;
                    CarFactoryCounter4.IncrementCarCount();
                }

            }

        }

    }

}
