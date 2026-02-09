using UnityEngine;

public class GlobalUnitSettings : MonoBehaviour
{
    public static GlobalUnitSettings Instance { get; private set; }

    public float DestinationDist => destinationDist;
    public float DestinationDistSqr { get; private set; }
    public float SampleSearchDistance => sampleSearchDistance;

    [SerializeField] private float destinationDist = 0.1f;
    [SerializeField] private float sampleSearchDistance = 15f;

    private void Awake()
    {
        Instance = this;
        DestinationDistSqr = DestinationDist * DestinationDist;
    }
}
