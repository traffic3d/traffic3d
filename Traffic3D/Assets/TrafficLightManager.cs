using System.Collections;

using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

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

        trafficLightRed1 = GameObject.Find("SphereTL1").GetComponent<TrafficLightRed1>();

        trafficLightGreen1 = GameObject.Find("SphereTL11").GetComponent<TrafficLightGreen1>();

        trafficLightRed2 = GameObject.Find("SphereTL2").GetComponent<TrafficLightRed2>();

        trafficLightGreen2 = GameObject.Find("SphereTL21").GetComponent<TrafficLightGreen2>();

        trafficLightRed3 = GameObject.Find("SphereTL3").GetComponent<TrafficLightRed3>();

        trafficLightGreen3 = GameObject.Find("SphereTL31").GetComponent<TrafficLightGreen3>();

        trafficLightRed4 = GameObject.Find("SphereTL4").GetComponent<TrafficLightRed4>();

        trafficLightGreen4 = GameObject.Find("SphereTL41").GetComponent<TrafficLightGreen4>();

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
