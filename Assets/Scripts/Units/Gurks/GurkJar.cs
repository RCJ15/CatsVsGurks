using UnityEngine;

public class GurkJar : Gurk
{
    protected override void Attack()
    {
        base.Attack();

        Die();
    }

    public override void Die()
    {
        base.Die();
    }
}
