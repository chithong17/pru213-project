using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCirclePreview : MonoBehaviour
{
    public float radius = 2f;
    public int segments = 50;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawCircle(radius);
    }

    public void DrawCircle(float newRadius)
    {
        radius = newRadius;

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.positionCount = segments + 1;
        float angle = 0f;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }
}