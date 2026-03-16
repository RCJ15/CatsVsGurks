using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalTowerSettings : MonoBehaviour
{
    public static GlobalTowerSettings Instance { get; private set; }

    public Material ValidPreviewMaterial => validPreviewMaterial;
    public Material InvalidPreviewMaterial => invalidPreviewMaterial;

    [SerializeField] private Material validPreviewMaterial;
    [SerializeField] private Material invalidPreviewMaterial;

    private void Awake()
    {
        Instance = this;
    }
}
