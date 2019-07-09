using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction2 : MonoBehaviour
{

    public Material material1;  //default red
    public Material material2;  //red
    public Material material3;    //green
    public Material material4;    //amber material

    public Material CM;

    public float timer = 0;
    private float timerMax = 0;


    void Start()
    {

        CM = GetComponent<Renderer>().material;

    }

    public void defaultmaterial()
    {
        CM = material1;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material1;
        GetComponent<Renderer>().materials = myarr;
    }


    public void materialchangeRED2()
    {
        CM = material2;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material2;
        GetComponent<Renderer>().materials = myarr;
    }



    public void materialchangeAMBER()
    {
        CM = material4;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material4;
        GetComponent<Renderer>().materials = myarr;
    }


    public void amberwait2()
    {
        if (CM == material4)
        {
            if (!Waited(2))
            {
                return;

            }

            CM = material3;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = material3;
            GetComponent<Renderer>().materials = myarr;

        }
    }



    public void materialchangeGREEN2()
    {

        CM = material3;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material3;
        GetComponent<Renderer>().materials = myarr;
    }

    void Update()
    {

    }
    private bool Waited(float seconds)
    {
        timerMax = seconds;
        timer += Time.deltaTime;

        if (timer >= timerMax)
        {
            timer = 0;
            return true;
        }
        return false;
    }


}
