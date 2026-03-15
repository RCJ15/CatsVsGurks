using UnityEngine;

public class Cat : Unit
{
    public override Team Team => Team.Player;

    private LaserPointer _laserPointer;

    private bool _inAttractionRange;
    private bool _attracted;
    private float _attractionTimer;
    private float _attractionDuration;
    private float _ignoreEntitiesDuration;

    protected override void Start()
    {
        base.Start();

        _laserPointer = LaserPointer.Instance;
    }

    protected override void Update()
    {
        base.Update();

        Vector3 point = LaserPointer.Point;

        float sqrDist = (transform.position - point).sqrMagnitude;
        _inAttractionRange = sqrDist <= _laserPointer.SqrAttractionRange;

        // In range
        if (_inAttractionRange)
        {
            _attractionTimer += Time.deltaTime;

            if (_attractionTimer >= _laserPointer.TimeUntilAttracted)
            {
                _attractionTimer = _laserPointer.TimeUntilAttracted;

                if (!_attracted)
                {
                    _attractionDuration = _laserPointer.AttractionDuration;
                    _ignoreEntitiesDuration = _laserPointer.AttractionDuration;
                    _attracted = true;

                    _entityTarget = null;
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

        if (_ignoreEntitiesDuration > 0)
        {
            _ignoreEntitiesDuration -= Time.deltaTime;
        }
    }

    protected override Entity DetermineEntityTarget()
    {
        if (_ignoreEntitiesDuration > 0)
        {
            return null;
        }

        return base.DetermineEntityTarget();
    }

    protected override Vector3? DetermineTarget()
    {
        if (_attractionDuration > 0)
        {
            return LaserPointer.Point;
        }

        return null;
    }
}
