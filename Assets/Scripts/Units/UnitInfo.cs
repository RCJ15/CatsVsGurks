using UnityEngine;

[CreateAssetMenu(fileName = "Unit Info", menuName = "Create Unit")]
public class UnitInfo : ScriptableObject
{
    [SerializeField] private string displayName;

    [Space]
    [SerializeField] private float hp = 100;
    [SerializeField] private float attack;
    [SerializeField] private float defense;
    [SerializeField] private float speed;

    [SerializeField] private string ai;
}
