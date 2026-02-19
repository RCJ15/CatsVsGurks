using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalTowerSettings : MonoBehaviour
{
    public static GlobalTowerSettings Instance { get; private set; }

    public InputActionReference PlacementInputAction => placementInputAction;

    [SerializeField] private InputActionReference placementInputAction;

    private void Awake()
    {
        Instance = this;
    }
}
