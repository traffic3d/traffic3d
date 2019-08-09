using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baselineLoop : MonoBehaviour
{

    public GameObject trafficlight1;
    public GameObject trafficlight2;

    public TrafficLightRed1 m = null;
    public TrafficLightRed2 n = null;



    void Start()
    {
        trafficlight1 = GameObject.Find("TrafficLight1");
        m = trafficlight1.GetComponent<TrafficLightRed1>();

        trafficlight2 = GameObject.Find("TrafficLight2");
        n = trafficlight2.GetComponent<TrafficLightRed2>();

        StartCoroutine(looping());
    }


    public IEnumerator looping()
    {

        while (true)
        {
            yield return StartCoroutine(one());
            yield return StartCoroutine(wait1());
            yield return StartCoroutine(two());
            yield return StartCoroutine(wait2());
        }
    }
    public IEnumerator one()
    {
        n.SetToRedMaterial();
        yield return new WaitForSeconds(6);
        m.SetToGreenMaterial();
        yield return null;
    }

    public IEnumerator wait1()
    {
        yield return new WaitForSeconds(5);
    }

    public IEnumerator two()
    {
        m.SetToRedMaterial();
        yield return new WaitForSeconds(6);
        n.SetToGreenMaterial();
        yield return null;
    }

    public IEnumerator wait2()
    {
        yield return new WaitForSeconds(5);
    }

    void Update()
    {

    }
}
