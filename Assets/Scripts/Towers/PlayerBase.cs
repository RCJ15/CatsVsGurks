using UnityEngine;

public class PlayerBase : Tower
{
    public static PlayerBase Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Instance = this;

        Player.MaxHP = MaxHP;
        Player.HP = HP;
    }

    public override void Hurt(float damage, Entity from)
    {
        base.Hurt(damage, from);

        Player.MaxHP = MaxHP;
        Player.HP = HP;
    }

    public override void Die()
    {
        base.Die();

        Player.MaxHP = MaxHP;
        Player.HP = 0;

        // Death
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
