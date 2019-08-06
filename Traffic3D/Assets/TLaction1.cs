using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction1 : MonoBehaviour
{


    public Material material1;  //default red
    public Material material2;  //red
    public Material material3; //green
    public Material material4; //amber
    public Material material5; //black

    public Material CM;

    public float timer = 0;   //we donot need the timer for now
    public float timerMax = 0; // we dont use this for now

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




    public void materialchangeRED1()
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

    public void amberwait()
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


    public void materialchangeGREEN1()
    {

        CM = material3;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material3;
        GetComponent<Renderer>().materials = myarr;
    }

    public void materialchangeblack()
    {

        CM = material5;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material5;
        GetComponent<Renderer>().materials = myarr;
    }


    public void materialchangeAMBER1()
    {
        CM = material4;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material4;
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
