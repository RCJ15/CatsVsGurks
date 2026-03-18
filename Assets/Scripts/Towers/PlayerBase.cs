using UnityEngine;

public class PlayerBase : Tower
{
    public static PlayerBase Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        Player.MaxHP = MaxHP;
        Player.HP = HP;
    }

    private void Update()
    {
        if (VisualsPlane.Instance != null && !VisualsPlane.Instance.FieldPlaced)
        {
            Vector3 dir = HeadPosition.Pos - transform.position;
            dir.y = 0;
            transform.forward = dir;
        }
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

        SfxPlayer.PlaySfx("CatLose", 1);

        // Death
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
