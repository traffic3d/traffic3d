using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera camera;
    private bool renderScreenshot = false;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void OnPostRender()
    {
        if (renderScreenshot)
        {
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            texture.Apply();
            PythonManager.GetInstance().SetScreenshot(camera, texture);
            renderScreenshot = false;
        }
    }

    public void SetRenderScreenshot()
    {
        renderScreenshot = true;
    }
}
