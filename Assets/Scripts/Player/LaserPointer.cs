using UnityEngine;
using UnityEngine.InputSystem;

public class LaserPointer : MonoBehaviour
{
    public static LaserPointer Instance { get; private set; }

    public static Vector3 Position { get; private set; }

    private Camera _camera;

    [SerializeField] private Transform dot;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private InputActionReference mousePosInputAction;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _camera = CameraInstance.Cam;
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(mousePosInputAction.action.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitLayer))
        {
            dot.gameObject.SetActive(true);

            Position = hit.point;
            dot.position = Position;
            dot.up = hit.normal;
        }
        else
        {
            dot.gameObject.SetActive(false);
        }
    }
}
