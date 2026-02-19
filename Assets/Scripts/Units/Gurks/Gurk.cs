using UnityEngine;

public class Gurk : Unit
{
    public override Team Team => Team.Enemy;

    [Space]
    [SerializeField] protected Vector2Int value = new Vector2Int(50, 100);

    private PlayerBase _playerBase;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        _playerBase = PlayerBase.Instance;
    }

    protected override Vector3 DetermineTarget()
    {
        return _playerBase.transform.position;
    }
}
