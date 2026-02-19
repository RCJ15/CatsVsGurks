using UnityEngine;
using UnityEngine.InputSystem;

public class LaserPointer : MonoBehaviour
{
    public static LaserPointer Instance { get; private set; }

    public static Vector3 Position
    {
        get => Instance.transform.position;
        set => Instance.transform.position = value;
    }

    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private InputActionReference mousePosInputAction;

    private void Awake()
    {
        Instance = this;
    }
}
