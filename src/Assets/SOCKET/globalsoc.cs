using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;


public class globalsoc : MonoBehaviour {

	//public const int port = 13030;
	//public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


	void Start () {

		print("scene1 socket one loaded");
		//socket.Connect ("localhost", port);
		//print("established tcp connection with python");


		changeScene2 ();
	}
	

	public void changeScene2()

	{
		SceneManager.LoadSceneAsync("Scene2");
		//print ("scene2 Loaded");
	}


	void Update () {
		
	}
}
