using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeArcDrawer : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float arcAngle = 60f;   // độ dài cung sáng
    [SerializeField] private int segments = 30;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        DrawArc();
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        DrawArc();
    }

    public void DrawArc()
    {
        if (line == null) line = GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.loop = false;

        float startAngle = -arcAngle * 0.5f;
        float angleStep = arcAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}