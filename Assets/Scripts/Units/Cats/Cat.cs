using UnityEngine;

public class Cat : Unit
{
    public override Team Team => Team.Player;

    [Space]
    [SerializeField] private LaserPointer.Color color = LaserPointer.Color.Red;
    [SerializeField] private bool continouslyFollowLaser;

    public bool MatchingColor => HiveMQSubscriber.Instance == null || !HiveMQSubscriber.Instance.Connected || color == LaserPointer.CurrentColor;

    private LaserPointer _laserPointer;

    private bool _inAttractionRange;
    private bool _attracted;
    private float _attractionTimer;
    private float _attractionDuration;

    protected override void Start()
    {
        base.Start();

        _laserPointer = LaserPointer.Instance;

        SfxPlayer.PlaySfx("Meow", transform.position);
    }

    protected override void Update()
    {
        base.Update();

        Vector3 point = LaserPointer.Point;

        float sqrDist = (transform.position - point).sqrMagnitude;
        _inAttractionRange = sqrDist <= _laserPointer.SqrAttractionRange && MatchingColor;

        bool forced = sqrDist <= _laserPointer.SqrForcedAttractionRange;

        // In range
        if (_inAttractionRange && (forced || _entityTarget == null))
        {
            _attractionTimer += Time.deltaTime;

            if (_attractionTimer >= _laserPointer.TimeUntilAttracted)
            {
                _attractionTimer = _laserPointer.TimeUntilAttracted;

                if (!_attracted)
                {
                    _attractionDuration = _laserPointer.AttractionDuration;
                    _attracted = true;

                    SfxPlayer.PlaySfx("Meow", transform.position);
                }
            }
        }
        else
        {
            _attractionTimer -= Time.deltaTime;

            if (_attractionTimer <= 0)
            {
                _attractionTimer = 0;
                _attracted = false;
            }

            if (_attractionDuration > 0)
            {
                _attractionDuration -= Time.deltaTime;
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();

        // Angry meow
        if (Random.Range(0, 2) == 0)
        {
            SfxPlayer.PlaySfx("AngryMeow", transform.position);
        }
        else
        {
            SfxPlayer.PlaySfx("Hiss", transform.position);
        }
    }

    public override void Hurt(float damage, Entity from)
    {
        base.Hurt(damage, from);

        SfxPlayer.PlaySfx("SadMeow", transform.position);
    }

    protected override void FoundEntityTarget()
    {
        _attractionDuration = 0;
        _attractionTimer = 0;
        _attracted = false;
    }

    protected override Vector3? DetermineTargetPos()
    {
        if (
            ((continouslyFollowLaser && _attractionDuration > 0) 
            ||
            (!continouslyFollowLaser && _entityTarget == null && _attractionDuration > 0))
            &&
            _laserPointer.TowerPreview == null && !TowerShop.Open && MatchingColor
            )
        {
            return LaserPointer.Point;
        }

        return base.DetermineTargetPos();
    }

    protected override bool TargetPosIsForced()
    {
        return _attractionDuration > 0;
    }
}
