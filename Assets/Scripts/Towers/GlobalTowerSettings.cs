using UnityEngine;

public class GlobalTowerSettings : MonoBehaviour
{
    public static GlobalTowerSettings Instance { get; private set; }

    public LayerMask TowerPreviewLayerCheck => towerPreviewLayerCheck;
    public Material ValidPreviewMaterial => validPreviewMaterial;
    public Material InvalidPreviewMaterial => invalidPreviewMaterial;

    [SerializeField] private LayerMask towerPreviewLayerCheck;
    [SerializeField] private Material validPreviewMaterial;
    [SerializeField] private Material invalidPreviewMaterial;

    private void Awake()
    {
        Instance = this;
    }
}
