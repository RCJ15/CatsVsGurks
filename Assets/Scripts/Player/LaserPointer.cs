using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    // Move to settings script?
    public static bool LeftHanded { get; set; } = false;

    private static readonly RaycastHit[] _hits = new RaycastHit[1];

    public static LaserPointer Instance { get; private set; }

    public static Vector3 Point { get; private set; }

    public static Color CurrentColor { get; private set; } = Color.Red;

    [SerializeField] private GameObject tutorial;

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
    public TowerPreview TowerPreview { get; set; }

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
    [SerializeField] private OVRInput.RawButton restartButton;
    private float _tempRestartTimer;
    private bool _tempRestarting;

    [Header("Attraction")]
    [SerializeField] private float attractionRange;
    [SerializeField] private float forcedAttractionRange;
    [SerializeField] private float timeUntilAttracted;
    [SerializeField] private float attractionDuration;

    [SerializeField] private LineRenderer[] lasers;

    private void Awake()
    {
        Instance = this;

        SqrAttractionRange = attractionRange * attractionRange;
        SqrForcedAttractionRange = forcedAttractionRange * forcedAttractionRange;

        lasers = GetComponentsInChildren<LineRenderer>(true);

        lasers[0].enabled = false;
        lasers[1].enabled = false;
        lasers[2].enabled = false;
        lasers[3].enabled = false;

        ChangeColorLocal(CurrentColor);
    }

    private void Update()
    {
        // -- TEMP
        if (UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
        {
            ChangeColor(Color.Red);
        }

        if (UnityEngine.InputSystem.Keyboard.current.yKey.wasPressedThisFrame)
        {
            ChangeColor(Color.Yellow);
        }

        if (UnityEngine.InputSystem.Keyboard.current.uKey.wasPressedThisFrame)
        {
            ChangeColor(Color.Green);
        }

        if (UnityEngine.InputSystem.Keyboard.current.iKey.wasPressedThisFrame)
        {
            ChangeColor(Color.Blue);
        }

        _tempRestarting = OVRInput.Get(restartButton)

#if UNITY_EDITOR
            || UnityEngine.InputSystem.Keyboard.current.enterKey.isPressed
#endif
            ;

        if (_tempRestarting)
        {
            _tempRestartTimer += Time.deltaTime;
        }
        else
        {
            _tempRestartTimer = 0;
        }

        if (_tempRestartTimer > 2f)
        {
            Debug.Log("RESTART!!!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        // -- TEMP END

        // PRESS
        OVRInput.RawButton button = LeftHanded ? confirmLeftButton : confirmRightButton;

        if (TowerPreview != null)
        {
            if (OVRInput.GetDown(button))
            {
                if (TowerPreview.Valid)
                {
                    // Place tower
                    Instantiate(TowerToPlace, TowerPreview.transform.position, TowerPreview.transform.rotation);

                    Destroy(TowerPreview.gameObject);
                    TowerPreview = null;

                    if(tutorial.GetComponent<TutorialText>().currentTextIndex == 4)
                        StartCoroutine(tutorial.GetComponent<TutorialText>().Spawn());
                }
                else
                {
                    // Not valid placement
                }
            }
        }
        else
        {

            if (CurrentClickable != null)
            {
                if (OVRInput.GetDown(button))
                {
                    CurrentClickable.OnClickDown();
                }

                if (OVRInput.GetUp(button))
                {
                    CurrentClickable.OnClickUp();
                }
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
            foreach (var laser in lasers)
            {
                laser.SetPosition(0, hand.position);
                laser.SetPosition(1, hitPoint);
            }

            Point = SimulationPlane.TransformPoint(hitPoint);
            return;
        }

        hit = _hits[0];
        Point = hit.point;

        foreach (var laser in lasers)
        {
            laser.SetPosition(0, hand.position);
            laser.SetPosition(1, VisualsPlane.TransformPoint(hit.point));
        }

        if (TowerPreview != null)
        {
            TowerPreview.Position = Point;

            Vector3 dir = SimulationPlane.TransformPoint(hand.position) - TowerPreview.Position;
            dir.y = 0;
            TowerPreview.transform.forward = dir;
        }
    }

    public static void ChangeColor(Color color)
    {
        CurrentColor = color;

        Instance.ChangeColorLocal(color);
    }

    private void ChangeColorLocal(Color color)
    {
        CurrentColor = color;

        lasers[0].enabled = false;
        lasers[1].enabled = false;
        lasers[2].enabled = false;
        lasers[3].enabled = false;

        lasers[(int)color].enabled = true;
    }

    public enum Color
    {
        Red,
        Yellow,
        Green,
        Blue,
    }
}
