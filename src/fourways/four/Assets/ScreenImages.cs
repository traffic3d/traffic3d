using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenImages : MonoBehaviour {
    public string folder = "ScreenshotMovieOutput";
    public int frameRate = 5;
    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    public GameObject CarCount;

  

    void Start () {

        Time.captureFramerate = frameRate;
        System.IO.Directory.CreateDirectory(folder);
        //StartCoroutine(screenshotsAndTextFile(1f));


    }
	
	
	void Update () {
        //if (Time.time > nextActionTime)
        //{
            //nextActionTime += period;
            // execute block of code here
            takeScreenshot();
            writeTextFile();
            //Debug.Log("take screenshot");

        //}
        

    }
    IEnumerator screenshotsAndTextFile(float interval) {
        while(true)
        {
            takeScreenshot();
            writeTextFile();
            yield return new WaitForSeconds(interval);
        }
    }

    public void takeScreenshot() {
      
        string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
        ScreenCapture.CaptureScreenshot(name);
    }

        public void writeTextFile()                            //(string path, string name)
        {
        //CountCars countcarsscript = (CountCars)CarCount.GetComponent(typeof(CountCars));
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("car") as GameObject[];
        //print(objectsWithTag.Length);
        //System.IO.File.AppendAllText("ScreenshotMovieOutput/info.txt", string.Format("{0:D04}", Time.frameCount) + "," + objectsWithTag.Length + System.Environment.NewLine);
        System.IO.File.AppendAllText("ScreenshotMovieOutput/info.csv", string.Format("{0:D04}", Time.frameCount) + "," + CarCounter.getCarCount() + System.Environment.NewLine);
        }

}

//COUNTER counterscript = (COUNTER)Counter.GetComponent(typeof(COUNTER));
// counterscript.increment_counter();