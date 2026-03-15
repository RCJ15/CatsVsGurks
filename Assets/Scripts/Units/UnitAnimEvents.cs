using UnityEngine;

public class UnitAnimEvents : MonoBehaviour
{
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponentInParent<Unit>(true);
    }

    public void TriggerAnimEvent(string name)
    {
        _unit.TriggerAnimEvent(name);
    }
}
