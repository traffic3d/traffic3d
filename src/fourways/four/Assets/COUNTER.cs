using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COUNTER : MonoBehaviour
{

    private static int carCount = 0;
    public ArrayList counts = new ArrayList();
    public GameObject Camera;
    public int currentNode = 0;
    public List<Transform> nodes;
    public Vector3 spawnSpot = new Vector3(-37.83f, 11.28f, 14.96f);
    public Rigidbody VEHICLE;
    public Vector3 CameraPosition;

    public ScreenImages p = null;
    // Use this for initialization
    void Start()
    {
        p = Camera.GetComponent<ScreenImages>();
        carCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //cars_in_lane();
        //CameraRange();
    }


    public void increment_counter(){
        carCount += 1;
        Debug.Log(carCount);
    }

    public void decrement_counter()
    {
        carCount -= 1;
    }

    public void save_counter_take_image()
    {
        counts.Add(carCount);
    }

    private void cars_in_lane()
    {
        if (Instantiate(VEHICLE, spawnSpot, Quaternion.identity))
        {
            //Counter += 1;
        }
    }

    
    private void CameraRange()

    {
        if (Vector3.Distance(VEHICLE.transform.position, Camera.transform.position) < 8f)
        {
            //Counter -= 1;
        }

    }

    public int getCounter() {
        return carCount;
    }

    public void writeTextFile(string path, string name) {
        //TODO: add name of the image then the carCount

        // System.IO.File.WriteAllText(path, carCount.ToString());
        System.IO.File.AppendAllText("ScreenshotMovieOutput/info.txt",name+" "+ carCount.ToString()+ System.Environment.NewLine);
        // System.IO.File.ReadAllText(path);
        //Debug.Log("save text file " + carCount.ToString());
        //Debug.Log(carCount);
    }
}