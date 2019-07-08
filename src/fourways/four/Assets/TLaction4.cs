using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction4 : MonoBehaviour {


	public Material material1;  //default red
	public Material material2;  //red
	public Material material3; //green
	public Material material4; //amber

	public Material CM;

	void Start () {
		CM = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void defaultmaterial()
	{
		CM = material1;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material1;
		GetComponent<Renderer> ().materials = myarr;
	}

	public void materialchangeRED4()
	{
		CM = material2;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material2;
		GetComponent<Renderer> ().materials = myarr;	
	}


	public void materialchangeGREEN4()
	{
		//		if (CM == material4) {


		//			if (!Waited (2)) {
		//				return;
		//			}

		CM = material3;
		Material[] myarr = GetComponent<Renderer> ().materials;
		myarr [0] = material3;
		GetComponent<Renderer> ().materials = myarr;	
	}

}
