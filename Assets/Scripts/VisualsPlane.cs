using UnityEngine;

public class VisualsPlane : MonoBehaviour
{
    public static VisualsPlane Instance { get; private set; }

    public static Transform Transform { get; private set; }

    private void Awake()
    {
        Instance = this;

        Transform = transform;
    }

    public static Vector3 TransformPoint(Vector3 point)
    {
        return Transform.TransformPoint(SimulationPlane.Transform.InverseTransformPoint(point));
    }

    public static Vector3 TransformDirection(Vector3 direction)
    {
        return Transform.TransformDirection(SimulationPlane.Transform.InverseTransformDirection(direction));
    }

    public static Quaternion TransformRotation(Vector3 forward, Vector3 up)
    {
        forward = TransformDirection(forward);
        up = TransformDirection(up);

        return Quaternion.LookRotation(forward, up);
    }
}
