using UnityEngine;

public class ObjectiveArrowMovement : MonoBehaviour
{
    [Header("'Bobbing' Settings")]
    [Tooltip("How far, in local units, the arrow moves up (+) and down (−) from its starting position.")]
    public float amplitude = 0.5f;

    [Tooltip("How many full up-and-down cycles per second.")]
    public float frequency = 1f;

    [Header("Rotation Settings")]
    [Tooltip("Whether the arrow should spin continuously.")]
    public bool enableRotation = true;

    [Tooltip("Rotation axis in local space.")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Degrees per second to rotate around the axis.")]
    public float rotationSpeed = 90f;

    // cached start position
    Vector3 _startLocalPos;

    void Awake()
    {
        _startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // Bobbing
        float t = Mathf.Sin(Time.time * Mathf.PI * 2f * frequency);
        Vector3 pos = _startLocalPos;
        pos.y += amplitude * t;
        transform.localPosition = pos;

        // Optional spinning
        if (enableRotation && rotationSpeed != 0f)
        {
            transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}