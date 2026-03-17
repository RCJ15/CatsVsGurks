using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public bool Valid { get; private set; }

    [SerializeField] private Vector2 size;

    private GlobalTowerSettings _globalTowerSettings;

    private MeshRenderer[] _renderers;

    private void Start()
    {
        _globalTowerSettings = GlobalTowerSettings.Instance;

        _renderers = GetComponentsInChildren<MeshRenderer>(true);

        SetValid(false, true);
    }

    private void Update()
    {
        SetValid(Physics.CheckBox(transform.position, new Vector3(size.x, 100, size.y), transform.rotation, _globalTowerSettings.TowerPreviewLayerCheck));
    }

    private void SetValid(bool valid, bool forced = false)
    {
        if (Valid == valid && !forced) return;

        Valid = valid;

        if (valid)
        {
            SetMaterial(_globalTowerSettings.ValidPreviewMaterial);
        }
        else
        {
            SetMaterial(_globalTowerSettings.InvalidPreviewMaterial);
        }
    }

    private void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderers in _renderers)
        {
            int length = renderers.sharedMaterials.Length;

            for (int i = 0; i < length; i++)
            {
                renderers.sharedMaterials[i] = material;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, new(size.x, 10, size.y));
    }
#endif
}
