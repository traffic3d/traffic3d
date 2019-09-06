using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Diagnostics;

public class TrafficLightManager : MonoBehaviour
{

    public GameObject trafficLightObjectRed1;
    public GameObject trafficLightObjectGreen1;
    public GameObject trafficLightObjectRed2;
    public GameObject TrafficLightObjectGreen2;
    public GameObject trafficLightObjectRed3;
    public GameObject trafficLightObjectGreen3;
    public GameObject trafficLightObjectRed4;
    public GameObject trafficLightObjectGreen4;

    public TrafficLightRed1 trafficLightRed1 = null;
    public TrafficLightGreen1 trafficLightGreen1 = null;
    public TrafficLightRed2 trafficLightRed2 = null;
    public TrafficLightGreen2 trafficLightGreen2 = null;
    public TrafficLightRed3 trafficLightRed3 = null;
    public TrafficLightGreen3 trafficLightGreen3 = null;
    public TrafficLightRed4 trafficLightRed4 = null;
    public TrafficLightGreen4 trafficLightGreen4 = null;

    void Start()
    {

        print("Start Traffic Light System");

        trafficLightObjectRed1 = GameObject.Find("SphereTL1");
        trafficLightRed1 = trafficLightObjectRed1.GetComponent<TrafficLightRed1>();

        trafficLightObjectGreen1 = GameObject.Find("SphereTL11");
        trafficLightGreen1 = trafficLightObjectGreen1.GetComponent<TrafficLightGreen1>();

        trafficLightObjectRed2 = GameObject.Find("SphereTL2");
        trafficLightRed2 = trafficLightObjectRed2.GetComponent<TrafficLightRed2>();

        TrafficLightObjectGreen2 = GameObject.Find("SphereTL21");
        trafficLightGreen2 = TrafficLightObjectGreen2.GetComponent<TrafficLightGreen2>();

        trafficLightObjectRed3 = GameObject.Find("SphereTL3");
        trafficLightRed3 = trafficLightObjectRed3.GetComponent<TrafficLightRed3>();

        trafficLightObjectGreen3 = GameObject.Find("SphereTL31");
        trafficLightGreen3 = trafficLightObjectGreen3.GetComponent<TrafficLightGreen3>();

        trafficLightObjectRed4 = GameObject.Find("SphereTL4");
        trafficLightRed4 = trafficLightObjectRed4.GetComponent<TrafficLightRed4>();

        trafficLightObjectGreen4 = GameObject.Find("SphereTL41");
        trafficLightGreen4 = trafficLightObjectGreen4.GetComponent<TrafficLightGreen4>();

        StartCoroutine(MainLoop());

    }


    public IEnumerator MainLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FirstEvent());
            yield return StartCoroutine(SecondEvent());
            yield return StartCoroutine(ThirdEvent());
            yield return StartCoroutine(FourthEvent());
            yield return StartCoroutine(FifthEvent());
        }
    }

    public IEnumerator FirstEvent()
    {
        trafficLightRed1.SetToRedMaterial();
        trafficLightGreen1.SetToBlackMaterial();
        trafficLightRed4.SetToRedMaterial();
        trafficLightGreen4.SetToBlackMaterial();
        trafficLightRed2.SetToRedMaterial();
        trafficLightGreen2.SetToBlackMaterial();

        trafficLightRed3.SetToRedMaterial();
        trafficLightGreen3.SetToBlackMaterial();
        yield return new WaitForSeconds(200);


    }

    public IEnumerator SecondEvent()
    {
        trafficLightRed3.SetToRedMaterial();
        trafficLightGreen3.SetToBlackMaterial();
        trafficLightRed1.SetToBlackMaterial();
        trafficLightGreen1.SetToGreenMaterial();
        trafficLightRed4.SetToRedMaterial();
        trafficLightGreen4.SetToBlackMaterial();

        trafficLightRed2.SetToRedMaterial();
        trafficLightGreen2.SetToBlackMaterial();
        yield return new WaitForSeconds(19);

    }

    public IEnumerator ThirdEvent()
    {
        trafficLightRed4.SetToRedMaterial();
        trafficLightGreen4.SetToBlackMaterial();
        trafficLightRed1.SetToRedMaterial();
        trafficLightGreen1.SetToBlackMaterial();
        trafficLightRed3.SetToBlackMaterial();
        trafficLightGreen3.SetToGreenMaterial();

        trafficLightRed2.SetToRedMaterial();
        trafficLightGreen2.SetToBlackMaterial();

        yield return new WaitForSeconds(19);

    }

    public IEnumerator FourthEvent()
    {
        trafficLightRed1.SetToRedMaterial();
        trafficLightGreen1.SetToBlackMaterial();
        trafficLightRed4.SetToRedMaterial();
        trafficLightGreen4.SetToBlackMaterial();
        trafficLightRed3.SetToRedMaterial();
        trafficLightGreen3.SetToBlackMaterial();

        trafficLightRed2.SetToBlackMaterial();
        trafficLightGreen2.SetToGreenMaterial();

        yield return new WaitForSeconds(19);


    }


    public IEnumerator FifthEvent()
    {
        trafficLightGreen4.SetToGreenMaterial();
        trafficLightRed4.SetToBlackMaterial();
        trafficLightRed1.SetToRedMaterial();
        trafficLightGreen1.SetToBlackMaterial();
        trafficLightRed3.SetToRedMaterial();
        trafficLightGreen3.SetToBlackMaterial();
        trafficLightRed2.SetToRedMaterial();
        trafficLightGreen2.SetToBlackMaterial();
        yield return new WaitForSeconds(19);


    }

}
