using UnityEngine;

public class TowerShop : MonoBehaviour
{
    public static TowerShop Instance { get; private set; }

    private bool _open;
    [SerializeField] private Camera cam;
    [SerializeField] private float rotationDelta;

    [SerializeField] private OVRInput.RawButton toggleShopInput;

    private BuyTowerButton[] _buttons;
    private Quaternion _rotation;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _buttons = GetComponentsInChildren<BuyTowerButton>(true);

        foreach (var button in _buttons)
        {
            button.Toggle(false, true);
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(toggleShopInput)

#if UNITY_EDITOR
            || UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame
#endif

            )
        {
            _open = !_open;

            if (_open)
            {
                _rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            }

            // On Open
            foreach (var button in _buttons)
            {
                button.Toggle(_open);
            }
        }

        // Follow
        transform.position = cam.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, rotationDelta * Time.deltaTime);
    }
}
