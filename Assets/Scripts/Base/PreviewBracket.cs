using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PreviewBracket : MonoBehaviour
{
    public float markerSize = 1f;
    public float cornerLength = 0.22f;
    public float lineWidth = 0.07f;
    public Color bracketColor = new Color(1f, 0.96f, 0.72f, 1f); // Màu vàng nhạt giống lúc hover
    public int markerSortingOrder = 5000;

    public Vector3 bracketOffset = new Vector3(0f, -0.4f, 0f);

    private LineRenderer[] cornerRenderers;

    private void Awake()
    {
        // Tắt cái hình vuông trắng chướng mắt đi
        GetComponent<SpriteRenderer>().enabled = false;
        CreateCorners();
    }

    private void OnEnable()
    {
        if (cornerRenderers != null)
        {
            foreach (var lr in cornerRenderers) lr.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (cornerRenderers != null)
        {
            foreach (var lr in cornerRenderers) lr.enabled = false;
        }
    }

    private void CreateCorners()
    {
        cornerRenderers = new LineRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject cornerObj = new GameObject("Corner_" + i);
            cornerObj.transform.SetParent(transform, false);

            LineRenderer lr = cornerObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = 3;
            lr.startWidth = lineWidth * 1.15f;
            lr.endWidth = lineWidth * 1.15f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = bracketColor;
            lr.endColor = bracketColor;
            lr.sortingOrder = markerSortingOrder;

            cornerRenderers[i] = lr;
        }

        float half = markerSize * 0.5f;

        // Góc trên trái
        SetCornerPositions(cornerRenderers[0], new Vector3(-half + cornerLength, half, 0f), new Vector3(-half, half, 0f), new Vector3(-half, half - cornerLength, 0f));
        // Góc trên phải
        SetCornerPositions(cornerRenderers[1], new Vector3(half - cornerLength, half, 0f), new Vector3(half, half, 0f), new Vector3(half, half - cornerLength, 0f));
        // Góc dưới trái
        SetCornerPositions(cornerRenderers[2], new Vector3(-half + cornerLength, -half, 0f), new Vector3(-half, -half, 0f), new Vector3(-half, -half + cornerLength, 0f));
        // Góc dưới phải
        SetCornerPositions(cornerRenderers[3], new Vector3(half - cornerLength, -half, 0f), new Vector3(half, -half, 0f), new Vector3(half, -half + cornerLength, 0f));
    }

    private void SetCornerPositions(LineRenderer lr, Vector3 start, Vector3 middle, Vector3 end)
    {
        lr.SetPosition(0, start + bracketOffset);
        lr.SetPosition(1, middle + bracketOffset);
        lr.SetPosition(2, end + bracketOffset);
    }
}