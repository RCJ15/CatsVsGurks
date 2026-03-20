using UnityEngine;

public class Gurk : Unit
{
    public override Team Team => Team.Enemy;

    public float WaveWeight => waveWeight;

    public static int GurksRemaining { get; private set; }

    [Space]
    [SerializeField] private Vector2 rawrPitch = new Vector2(0.8f, 1.2f);
    [SerializeField] protected Vector2Int value = new Vector2Int(50, 100);
    [SerializeField] private float waveWeight = 1;

    private PlayerBase _playerBase;

    private Vector3 randPos;

    protected override void Awake()
    {
        base.Awake();

        InvokeRepeating(nameof(SetRandPos), 0f, 5f);

        GurksRemaining++;
    }

    private void OnDestroy()
    {
        GurksRemaining--;
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

        SfxPlayer.PlaySfx("GurkRawr", transform.position, 0.5f, Random.Range(rawrPitch.x, rawrPitch.y));
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
            if (_playerBase == null)
            {
                _playerBase = PlayerBase.Instance;

                return Vector3.zero;
            }

            return  _playerBase.transform.position;
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
