using UnityEngine;

public class Gurk : Unit
{
    public override Team Team => Team.Enemy;

    public float WaveWeight => waveWeight;

    [Space]
    [SerializeField] protected Vector2Int value = new Vector2Int(50, 100);
    [SerializeField] private float waveWeight = 1;

    private PlayerBase _playerBase;

    private Vector3 randPos;

    protected override void Awake()
    {
        base.Awake();

        InvokeRepeating(nameof(SetRandPos), 0f, 5f);
    }

    private void SetRandPos()
    {
        randPos = Random.insideUnitCircle * 45f;

        randPos.y = 0;
        randPos.z = randPos.y;
    }

    protected override void Start()
    {
        base.Start();

        _playerBase = PlayerBase.Instance;
    }

    protected override Entity DetermineEntityTarget()
    {
        Entity result = base.DetermineEntityTarget();

        /*
        if (result == null)
        {
            result = _playerBase;
        }
        */

        return result;
    }

    protected override void FoundEntityTarget()
    {

    }

    protected override Vector3? DetermineTargetPos()
    {
        if (_entityTarget == null)
        {
            return _playerBase.transform.position;
        }

        return base.DetermineTargetPos();
    }

    public override void Die()
    {
        base.Die();

        // Give money to player
        Player.Money += Random.Range(value.x, value.y);
    }
}
