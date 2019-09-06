using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{

    private static int carCount = 0;
    public ArrayList counts = new ArrayList();
    public GameObject Camera;
    public int currentNode = 0;
    public List<Transform> nodes;
    public Vector3 spawnSpot = new Vector3(-37.83f, 11.28f, 14.96f);
    public Rigidbody vehicle;
    public Vector3 CameraPosition;

    public ScreenImages p = null;

    // Use this for initialization
    void Start()
    {
        p = Camera.GetComponent<ScreenImages>();
        carCount = 1;
    }

    public void IncrementCarCount()
    {
        carCount += 1;
        Debug.Log(carCount);
    }

    public void DecrementCarCount()
    {
        carCount -= 1;
    }

    public void AddCurrentCounterToList()
    {
        counts.Add(carCount);
    }

    private void GenerateVehicle()
    {
        if (Instantiate(vehicle, spawnSpot, Quaternion.identity))
        {

        }
    }

    private void CameraRange()
    {
        if (Vector3.Distance(vehicle.transform.position, Camera.transform.position) < 8f)
        {

        }

    }

    public int GetCarCount()
    {
        return carCount;
    }

    public void WriteTextFile(string path, string name)
    {
        //TODO: add name of the image then the carCount

        System.IO.File.AppendAllText("ScreenshotMovieOutput/info.txt", name + " " + carCount.ToString() + System.Environment.NewLine);
    }
}