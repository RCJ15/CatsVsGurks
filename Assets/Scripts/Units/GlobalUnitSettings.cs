using UnityEngine;

public class GlobalUnitSettings : MonoBehaviour
{
    public static GlobalUnitSettings Instance { get; private set; }

    public float DestinationDist => destinationDist;
    public float DestinationDistSqr { get; private set; }
    public float SampleSearchDistance => sampleSearchDistance;

    public LayerMask LineOfSightLayer => lineOfSightLayer;
    public LayerMask ObstacleLayer => obstacleLayer;
    public int LineOfSightRaysPerSide => lineOfSightRaysPerSide;
    public float LineOfSightRayWidth => lineOfSightRayWidth;

    public float PathfindUpdateTimer => pathfindUpdateTimer;
    public int ForcePathUpdateIndex => forcePathUpdateIndex;
    public int FindBetterPointFrameCount => findBetterPointFrameCount;

    [SerializeField] private float destinationDist = 0.1f;
    [SerializeField] private float sampleSearchDistance = 15f;

    [Space]
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private int lineOfSightRaysPerSide = 1;
    [SerializeField] private float lineOfSightRayWidth = 0.25f;

    [Space]
    [SerializeField] private float pathfindUpdateTimer = 0.2f;
    [SerializeField] private int forcePathUpdateIndex = 5;
    [SerializeField] private int findBetterPointFrameCount = 10;

    private void Awake()
    {
        Instance = this;
        DestinationDistSqr = DestinationDist * DestinationDist;
    }
}
