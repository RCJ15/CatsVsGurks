using UnityEngine;

public class GurkJar : Gurk
{
    protected override void Attack()
    {
        Die();
    }

    public override void Die()
    {
        SpawnAttack(attack);

        base.Die();
    }
}
