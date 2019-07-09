using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class journeyTimeCARCOUNTER : MonoBehaviour {

	public static int journeyCARs = 0;


	void Start () {
		
	}
	

	void Update () {
		
	}

	public static int getjourneyCARsCount()
	{
		return journeyCARs;

		}

	public static void incrementjourneyCARsCount()
	{
		journeyCARs++;

	}

}
