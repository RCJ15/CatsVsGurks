using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    [SerializeField] private Vector3 size;

    private GlobalTowerSettings _globalTowerSettings;

    private MeshRenderer[] _renderers;

    private void Start()
    {
        _globalTowerSettings = GlobalTowerSettings.Instance;

        _renderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void Update()
    {
        
    }

    private void SetValid(bool valid)
    {
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
    }
#endif
}
