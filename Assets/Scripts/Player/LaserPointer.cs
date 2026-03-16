using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    // Move to settings script?
    public static bool LeftHanded { get; set; } = false;

    private static readonly RaycastHit[] _hits = new RaycastHit[1];

    public static LaserPointer Instance { get; private set; }

    public static Vector3 Point { get; private set; }

    public static ClickableObject CurrentClickable
    {
        get => _currentClickable;
        set
        {
            if (_currentClickable == value)
            {
                return;
            }

            if (_currentClickable != null)
            {
                _currentClickable.OnDeselect();
            }

            _currentClickable = value;

            if (_currentClickable != null)
            {
                _currentClickable.OnSelect();
            }
        }
    }
    private static ClickableObject _currentClickable;

    public Tower TowerToPlace { get; set; }
    public MeshRenderer TowerPreview { get; set; }

    public float AttractionRange => attractionRange;
    public float SqrAttractionRange { get; private set; }
    public float ForcedAttractionRange => attractionRange;
    public float SqrForcedAttractionRange { get; private set; }
    public float TimeUntilAttracted => timeUntilAttracted;
    public float AttractionDuration => attractionDuration;

    [SerializeField] private LayerMask clickablesLayer;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float maxDistance;

    [Space]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;

    [Space]
    [SerializeField] private OVRInput.RawButton confirmRightButton;
    [SerializeField] private OVRInput.RawButton confirmLeftButton;

    [Space]
    [SerializeField] private LineRenderer laser;

    [Header("Attraction")]
    [SerializeField] private float attractionRange;
    [SerializeField] private float forcedAttractionRange;
    [SerializeField] private float timeUntilAttracted;
    [SerializeField] private float attractionDuration;

    private void Awake()
    {
        Instance = this;

        SqrAttractionRange = attractionRange * attractionRange;
        SqrForcedAttractionRange = forcedAttractionRange * forcedAttractionRange;
    }

    private void Update()
    {
        // PRESS
        if (CurrentClickable != null)
        {
            OVRInput.RawButton button = LeftHanded ? confirmLeftButton : confirmRightButton;

            if (OVRInput.GetDown(button))
            {
                CurrentClickable.OnClickDown();
            }

            if (OVRInput.GetUp(button))
            {
                CurrentClickable.OnClickUp();
            }
        }

        // Determine hand
        Transform hand = LeftHanded ? leftHand : rightHand;

        Vector3 position = hand.position;
        Vector3 forward = hand.forward;

        // Shoot ray for clickable objects
        RaycastHit hit;
        Ray ray = new Ray(position, forward);

        bool success = Physics.RaycastNonAlloc(ray, _hits, maxDistance, clickablesLayer) > 0;

        if (success)
        {
            // Check for clickable object
            hit = _hits[0];

            if (ClickableObject.ColliderToClickable.TryGetValue(hit.collider, out ClickableObject clickable))
            {
                CurrentClickable = clickable;
            }
            else
            {
                CurrentClickable = null;
            }
        }
        else
        {
            CurrentClickable = null;
        }

        // Raycast for laser
        position = SimulationPlane.TransformPoint(position);
        forward = SimulationPlane.TransformDirection(forward);

        ray = new Ray(position, forward);

        success = Physics.RaycastNonAlloc(ray, _hits, maxDistance, hitLayer) > 0;

        if (!success)
        {
            Vector3 hitPoint = hand.position + (hand.forward * maxDistance);
            laser.SetPosition(0, hand.position);
            laser.SetPosition(1, hitPoint);

            Point = SimulationPlane.TransformPoint(hitPoint);
            return;
        }

        laser.enabled = true;

        hit = _hits[0];
        Point = hit.point;

        laser.SetPosition(0, hand.position);
        laser.SetPosition(1, VisualsPlane.TransformPoint(hit.point));
    }
}
