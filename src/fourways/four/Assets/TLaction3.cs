using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction3 : MonoBehaviour
{


    public Material material1;  //default red
    public Material material2;  //red
    public Material material3; //green
    public Material material4; //amber

    public Material CM;

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


    public void materialchangeRED3()
    {
        CM = material2;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material2;
        GetComponent<Renderer>().materials = myarr;
    }


    public void materialchangeGREEN3()
    {

        CM = material3;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material3;
        GetComponent<Renderer>().materials = myarr;

    }
}
