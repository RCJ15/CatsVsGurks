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

    public float AttractionRange => attractionRange;
    public float SqrAttractionRange { get; private set; }
    public float TimeUntilAttracted => timeUntilAttracted;
    public float AttractionDuration => attractionDuration;

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
    [SerializeField] private float timeUntilAttracted;
    [SerializeField] private float attractionDuration;

    private void Awake()
    {
        Instance = this;

        SqrAttractionRange = attractionRange * attractionRange;
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

        // Raycast
        position = SimulationPlane.TransformPoint(position);
        forward = SimulationPlane.TransformDirection(forward);

        Ray ray = new Ray(position, forward);

        bool success = Physics.RaycastNonAlloc(ray, _hits, maxDistance, hitLayer) > 0;

        if (!success)
        {
            Vector3 hitPoint = hand.position + (hand.forward * maxDistance);
            laser.SetPosition(0, hand.position);
            laser.SetPosition(1, hitPoint);

            Point = SimulationPlane.TransformPoint(hitPoint);
            CurrentClickable = null;
            return;
        }

        laser.enabled = true;

        // Check for clickable object
        RaycastHit hit = _hits[0];

        Point = hit.point;

        laser.SetPosition(0, hand.position);
        laser.SetPosition(1, VisualsPlane.TransformPoint(hit.point));

        if (!ClickableObject.ColliderToClickable.TryGetValue(hit.collider, out ClickableObject clickable))
        {
            CurrentClickable = null;
            return;
        }

        CurrentClickable = clickable;
    }
}
