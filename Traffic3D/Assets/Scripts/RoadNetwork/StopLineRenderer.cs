using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLineRenderer : MonoBehaviour
{
    private GameObject render = null;
    private float heightOffset = 0.01f;
    public float renderWidth = RoadGenerator.defaultLaneWidth;

    public void RenderStopLine()
    {
        if (render != null)
        {
            DestroyImmediate(render);
        }
        render = Instantiate(Resources.Load<GameObject>("Models/StopLineRender"), transform);
        render.transform.Translate(Vector3.down * (1 - heightOffset), Space.Self);
        Vector3 scale = render.transform.localScale;
        render.transform.localScale = new Vector3(scale.x * renderWidth, scale.y, scale.z);
    }
}
