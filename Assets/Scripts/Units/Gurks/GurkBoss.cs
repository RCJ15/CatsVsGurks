using UnityEngine;

public class GurkBoss : Gurk
{
    [SerializeField] private UnitAttack altAttack;

    private int _attackIndex;

    protected override void Attack()
    {
        _attackIndex++;

        if (_attackIndex == 2)
        {
            SpawnAttack(altAttack);
            _attackIndex = 0;
        }

        base.Attack();
    }
}
