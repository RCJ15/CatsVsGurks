using UnityEngine;

public class GurkJar : Gurk
{
    [SerializeField] private Gurk spawnOnDeath;

    protected override void Attack()
    {
        base.Attack();

        Die();
    }


}
