using UnityEngine;

public class GlobalUnitSettings : MonoBehaviour
{
    public static GlobalUnitSettings Instance { get; private set; }

    public float ReachedPointDist => reachedPointDist;
    public float ReachedPointDistSqr { get; private set; }
    public float ReachedDestinationDist => reachedDestinationDist;
    public float ReachedDestinationDistSqr { get; private set; }
    public float UnitKnockbackDelta => unitKnockbackDelta;

    public float TimeBtwWanderingPoints => timeBtwWanderingPoints;
    public float WanderingRange => WanderingRange;

    public LayerMask LineOfSightLayer => lineOfSightLayer;
    public LayerMask ObstacleLayer => obstacleLayer;
    public int LineOfSightRaysPerSide => lineOfSightRaysPerSide;
    public float LineOfSightRayWidth => lineOfSightRayWidth;

    public float PathfindUpdateTimer => pathfindUpdateTimer;
    public int ForcePathUpdateIndex => forcePathUpdateIndex;
    public int FindBetterPointFrameCount => findBetterPointFrameCount;

    [SerializeField] private float reachedPointDist = 0.1f;
    [SerializeField] private float reachedDestinationDist = 0.5f;
    [SerializeField] private float unitKnockbackDelta;

    [Header("Wandering State")]
    [SerializeField] private float timeBtwWanderingPoints;
    [SerializeField] private float wanderingRange;

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
        ReachedPointDistSqr = ReachedPointDist * ReachedPointDist;
        ReachedDestinationDistSqr = ReachedDestinationDist * ReachedDestinationDist;
    }
}
