using UnityEngine;

public class RotateRangeSweep : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 120f;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}