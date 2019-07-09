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

    }
	
	
	void Update () {
            // execute block of code here
            takeScreenshot();
            writeTextFile();
        

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

        public void writeTextFile()
        {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("car") as GameObject[];
        System.IO.File.AppendAllText("ScreenshotMovieOutput/info.csv", string.Format("{0:D04}", Time.frameCount) + "," + CarCounter.getCarCount() + System.Environment.NewLine);
        }

}