using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public bool Valid { get; private set; }

    public Vector3 Position
    {
        get => transform.position;
        set
        {
            if (_simulationPlane != null)
            {
                value.y = _simulationPlane.transform.position.y;
            }

            transform.position = value;
        }
    }

    private static readonly Collider[] _colliders = new Collider[1];

    [SerializeField] private Vector2 size;

    private GlobalTowerSettings _globalTowerSettings;

    private MeshRenderer[] _renderers;
    private SimulationPlane _simulationPlane;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<MeshRenderer>(true);
    }

    private void Start()
    {
        _globalTowerSettings = GlobalTowerSettings.Instance;
        _simulationPlane = SimulationPlane.Instance;

        SetValid(false, true);
    }

    private void Update()
    {
        int hitAmount = Physics.OverlapBoxNonAlloc(transform.position, new Vector3(size.x / 2f, 1000, size.y / 2f), _colliders, transform.rotation, _globalTowerSettings.TowerPreviewLayerCheck);

        SetValid(hitAmount <= 0);
    }

    private void SetValid(bool valid, bool forced = false)
    {
        if (Valid == valid && !forced)
        {
            return;
        }

        Valid = valid;

        SetMaterial(valid ? _globalTowerSettings.ValidPreviewMaterial : _globalTowerSettings.InvalidPreviewMaterial);
    }

    private void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.material = material;
            /*
            int length = renderer.materials.Length;

            for (int i = 0; i < length; i++)
            {
                renderer.materials[i] = material;
            }
            */
        }

        Debug.Log("SET MATERIAL TO " + material.name);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Valid ? Color.green : Color.red;

        Gizmos.DrawWireCube(transform.position, new(size.x, 10, size.y));
    }
#endif
}
