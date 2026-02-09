using UnityEngine;

public class SimulationPlane : MonoBehaviour
{
    public static SimulationPlane Instance { get; private set; }

    public static Transform Transform { get; private set; }

    private void Awake()
    {
        Instance = this;

        Transform = transform;
    }
}
