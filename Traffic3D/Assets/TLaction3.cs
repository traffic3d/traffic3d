using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction3 : MonoBehaviour {


	public Material material2;  //red
	public Material material3; //green
	public Material material4; //amber
	public Material material5; //black

	public Material CM;

	void Start () {

		CM = GetComponent<Renderer> ().material;
	}
	

/*	void Update()
	{
		if (CM == material2) {
			print ("TLaction3 red");
		}
	} */

	public void materialchangeblack()
	{
		CM = material5;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material5;
		GetComponent<Renderer> ().materials = myarr;	
	}

	public void materialchangeRED3()
	{
		CM = material2;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material2;
		GetComponent<Renderer> ().materials = myarr;	
	}


	public void materialchangeGREEN3()
	{

		CM = material3;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material3;
		GetComponent<Renderer> ().materials = myarr;	
	}

	public void materialchangeAMBER3()
	{
		CM = material4;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material4;
		GetComponent<Renderer> ().materials = myarr;
	}


}
