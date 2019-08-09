using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;

public class redlight : MonoBehaviour
{

    public GameObject trafficlight1;
    public GameObject trafficlight11;
    public GameObject trafficlight2;
    public GameObject trafficlight21;
    public GameObject trafficlight3;
    public GameObject trafficlight31;
    public GameObject trafficlight4;
    public GameObject trafficlight41;

    public TrafficLightRed1 m = null;
    public TrafficLightGreen1 m1 = null;
    public TrafficLightRed2 n = null;
    public TrafficLightGreen2 n1 = null;
    public TrafficLightRed3 u = null;
    public TrafficLightGreen3 u1 = null;
    public TrafficLightRed4 v = null;
    public TrafficLightGreen4 v1 = null;




    void Start()
    {

        print("start LOOPING");
        trafficlight1 = GameObject.Find("SphereTL1");   //TrafficLight1
        m = trafficlight1.GetComponent<TrafficLightRed1>();

        trafficlight11 = GameObject.Find("SphereTL11");   //TrafficLight1
        m1 = trafficlight11.GetComponent<TrafficLightGreen1>();

        trafficlight2 = GameObject.Find("SphereTL2");  //TrafficLight2
        n = trafficlight2.GetComponent<TrafficLightRed2>();

        trafficlight21 = GameObject.Find("SphereTL21");  //TrafficLight2
        n1 = trafficlight21.GetComponent<TrafficLightGreen2>();

        trafficlight3 = GameObject.Find("SphereTL3");
        u = trafficlight3.GetComponent<TrafficLightRed3>();

        trafficlight31 = GameObject.Find("SphereTL31");
        u1 = trafficlight31.GetComponent<TrafficLightGreen3>();

        trafficlight4 = GameObject.Find("SphereTL4");
        v = trafficlight4.GetComponent<TrafficLightRed4>();
        trafficlight41 = GameObject.Find("SphereTL41");
        v1 = trafficlight41.GetComponent<TrafficLightGreen4>();



        StartCoroutine(looping());

    }


    public IEnumerator looping()
    {
        while (true)
        {
            yield return StartCoroutine(one());
            yield return StartCoroutine(two());
            yield return StartCoroutine(third());
            yield return StartCoroutine(four());
            yield return StartCoroutine(five());
        }


    }

    public IEnumerator one()
    {
        m.SetToRedMaterial();
        m1.SetToBlackMaterial();
        v.SetToRedMaterial();
        v1.SetToBlackMaterial();
        n.SetToRedMaterial();
        n1.SetToBlackMaterial();

        u.SetToRedMaterial();
        u1.SetToBlackMaterial();
        yield return new WaitForSeconds(200);


    }

    public IEnumerator two()
    {
        u.SetToRedMaterial();
        u1.SetToBlackMaterial();
        m.SetToBlackMaterial();
        m1.SetToGreenMaterial();
        v.SetToRedMaterial();
        v1.SetToBlackMaterial();

        n.SetToRedMaterial();
        n1.SetToBlackMaterial();
        yield return new WaitForSeconds(19);

    }

    public IEnumerator third()
    {
        v.SetToRedMaterial();
        v1.SetToBlackMaterial();
        m.SetToRedMaterial();
        m1.SetToBlackMaterial();
        u.SetToBlackMaterial();
        u1.SetToGreenMaterial();

        n.SetToRedMaterial();
        n1.SetToBlackMaterial();

        yield return new WaitForSeconds(19);

    }

    public IEnumerator four()
    {
        m.SetToRedMaterial();
        m1.SetToBlackMaterial();
        v.SetToRedMaterial();
        v1.SetToBlackMaterial();
        u.SetToRedMaterial();
        u1.SetToBlackMaterial();

        n.SetToBlackMaterial();
        n1.SetToGreenMaterial();

        yield return new WaitForSeconds(19);


    }


    public IEnumerator five()
    {
        v1.SetToGreenMaterial();
        v.SetToBlackMaterial();
        m.SetToRedMaterial();
        m1.SetToBlackMaterial();
        u.SetToRedMaterial();
        u1.SetToBlackMaterial();
        n.SetToRedMaterial();
        n1.SetToBlackMaterial();
        yield return new WaitForSeconds(19);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
