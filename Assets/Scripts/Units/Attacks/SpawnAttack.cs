using UnityEngine;

public class SpawnAttack : UnitAttack
{
    [SerializeField] private Gurk[] spawn;

    protected override void Awake()
    {
        base.Awake();

        foreach (Gurk gurk in spawn)
        {
            Gurk newGurk = Instantiate(gurk, transform.position, Quaternion.Euler(0, Random.Range(0, 360f), 0));
        }
    }

    private void Start()
    {
        Die();
    }
}
