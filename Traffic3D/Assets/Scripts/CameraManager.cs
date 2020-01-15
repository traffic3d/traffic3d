using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int frameRate = 2;

    void Start()
    {
        Time.captureFramerate = frameRate;
    }

    private void OnPostRender()
    {
        if (Settings.IsHeadlessMode())
        {
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            texture.Apply();
            PythonManager.GetInstance().SetScreenshot(texture);
        }
    }
}
