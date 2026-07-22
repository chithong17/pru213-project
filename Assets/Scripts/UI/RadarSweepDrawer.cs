using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RadarSweepDrawer : MonoBehaviour
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float sweepAngle = 55f;
    [SerializeField] private int segments = 18;
    [SerializeField] private Color sweepColor = new Color(0.1f, 0.95f, 1f, 0.18f);

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh
        {
            name = "RadarSweepMesh"
        };

        GetComponent<MeshFilter>().mesh = mesh;
        EnsureMaterial();
        DrawSweep();
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        DrawSweep();
    }

    public void SetSorting(int sortingLayerId, int sortingOrder)
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        meshRenderer.sortingLayerID = sortingLayerId;
        meshRenderer.sortingOrder = sortingOrder;
    }

    private void EnsureMaterial()
    {
        if (meshRenderer.sharedMaterial != null)
        {
            return;
        }

        Shader shader = Shader.Find("Sprites/Default");
        meshRenderer.material = new Material(shader);
    }

    private void DrawSweep()
    {
        if (mesh == null)
        {
            return;
        }

        int safeSegments = Mathf.Max(3, segments);
        Vector3[] vertices = new Vector3[safeSegments + 2];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[safeSegments * 3];

        vertices[0] = Vector3.zero;
        colors[0] = sweepColor;

        float startAngle = -sweepAngle * 0.5f;
        float angleStep = sweepAngle / safeSegments;

        for (int i = 0; i <= safeSegments; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            int vertexIndex = i + 1;

            vertices[vertexIndex] = new Vector3(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius,
                0f
            );

            float edgeAlpha = Mathf.Lerp(0.04f, sweepColor.a, i / (float)safeSegments);
            colors[vertexIndex] = new Color(
                sweepColor.r,
                sweepColor.g,
                sweepColor.b,
                edgeAlpha
            );
        }

        for (int i = 0; i < safeSegments; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
}
