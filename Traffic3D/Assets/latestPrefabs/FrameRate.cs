using UnityEngine;

public class FrameRate : MonoBehaviour
{

    public int frameRate = 2;

    void Start()
    {
        Time.captureFramerate = frameRate;
    }

}
