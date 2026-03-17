using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Unit : Entity
{
    private static readonly RaycastHit[] _hit = new RaycastHit[5];

    public static readonly Dictionary<Team, List<Unit>> AllUnits = new();

    public Animator Animator { get; private set; }
    public Action<string> OnAnimEvent { get; set; }
    public bool CanMove { get; set; } = true;

    private Vector3 _targetPos;
    private bool _reachedTarget;
    private List<Entity> _entityTargetList = new();
    protected Entity _entityTarget;
    private bool _entityTargetInRange;

    [Space]
    [SerializeField] private bool knockbackResitance;
    [SerializeField] private string unitName;

    #region Statistics
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float Damage
    {
        get => damage;
        set => damage = value;
    }

    public float AttackDelay
    {
        get => attackDelay;
        set => attackDelay = value;
    }

    public float Range
    {
        get => range;
        set => range = value;
    }
    #endregion

    [Header("Stats")]
    [Tooltip("How many tiles per second this unit moves")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float turnSpeed = 2f;
    private Vector3 _lookDirection;
    private float _currentAngle;

    [Tooltip("How many seconds it takes for this unit to accelerate to max speed")]
    [SerializeField] private float timeToAccelerate = 0.1f;
    private float _accelerationRate;
    [Tooltip("How many seconds it takes for this unit to decelerate to zero speed")]
    [SerializeField] private float timeToDecelerate = 0.05f;
    private float _decelerationRate;
    private float _currentVelocity;

    [Space]
    [SerializeField] private UnitAttack attack;
    [Tooltip("Damage, self explanatory")]
    [SerializeField] private float damage = 25;
    [Tooltip("Seconds of delay between each attack")]
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackAnimSpeed = 1f;
    private float _attackCooldown;
    [Tooltip("How close/far a unit needs to be before this unit will attempt to attack")]
    [SerializeField] private float range = 1;
    [Tooltip("How close/far a unit needs to be before this unit will give chase")]
    [SerializeField] private float chaseRange = 10;
    protected float _sqrRange;
    protected float _sqrChaseRange;

    [SerializeField] private Rigidbody rb;

    [Header("Effects")]
    [SerializeField] private ParticleSystem walkParticles;

    private PathfindingManager _manager;
    private GlobalUnitSettings _globalUnitSettings;

    private List<Vector3> _path = new();
    private int _pathCount = 1;
    private int _pathIndex = 0;

    private CancellationTokenSource _token = new();

    private bool _pathfinding;
    private bool _updatePath;
    private int _forcePathUpdateIndex;

    private bool _hasTargetLineOfSight;

    private int _findBetterPointFrame;
    private Vector3? _betterPoint = null;

    private Vector3 _knockbackForce;

    protected override void Awake()
    {
        base.Awake();

        UpdateAccelerationRates();

        _path.Add(rb.position);
        _lookDirection = transform.forward;

        _sqrRange = range * range;
        _sqrChaseRange = chaseRange * chaseRange;

        _attackCooldown = attackDelay;

        Animator = GetComponentInChildren<Animator>(true);

        SetAnimFloat("AttackSpeed", attackAnimSpeed);

        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack);
    }

    protected override void Start()
    {
        base.Start();

        _manager = PathfindingManager.Instance;
        _globalUnitSettings = GlobalUnitSettings.Instance;

        Debug.Log(_globalUnitSettings);

        BeginInvokeRepeating();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (AllUnits.TryGetValue(Team, out var list))
        {
            list.Add(this);
        }
        else
        {
            AllUnits[Team] = new List<Unit>() { this };
        }

        if (_globalUnitSettings != null)
        {
            BeginInvokeRepeating();
        }
    }

    private void BeginInvokeRepeating()
    {
        InvokeRepeating(nameof(UpdatePathfinding), _globalUnitSettings.PathfindUpdateTimer, _globalUnitSettings.PathfindUpdateTimer);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (AllUnits.TryGetValue(Team, out var list))
        {
            list.Remove(this);
        }

        CancelInvoke(nameof(UpdatePathfinding));
    }

    private void UpdateAccelerationRates()
    {
        SetAccelerationRate(ref _accelerationRate, timeToAccelerate);
        SetAccelerationRate(ref _decelerationRate, timeToDecelerate);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        UpdateAccelerationRates();
    }
#endif

    private void OnDestroy()
    {
        _token.Cancel();

        _token.Dispose();
    }

    protected virtual void Update()
    {
        if (_entityTarget != null && !_entityTarget.Targettable)
        {
            _entityTarget = null;
        }

        if (_entityTarget == null)
        {
            _entityTarget = DetermineEntityTarget();

            if (_entityTarget != null)
            {
                FoundEntityTarget();
            }
        }

        Vector3 pos;

        Vector3? targetPos = DetermineTargetPos();

        if (targetPos.HasValue)
        {
            pos = targetPos.Value;
        }
        else
        {
            pos = transform.position;
        }

        if (pos != _targetPos)
        {
            _targetPos = pos;
            _updatePath = true;
        }

        _hasTargetLineOfSight = HasLineOfSight(_targetPos);

        // Attack logic
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }

        if (_attackCooldown <= 0 && _entityTargetInRange)
        {
            Attack();
        }
    }

    protected abstract void FoundEntityTarget();

    public bool HasLineOfSight(Vector3 pos) => HasLineOfSight(pos, out _);
    public bool HasLineOfSight(Vector3 pos, out float dist, LayerMask? layerOverride = null)
    {
        Vector3 thisPos = transform.position;
        Vector3 direction = pos - thisPos;
        direction.y = 0;
        direction.Normalize();

        dist = Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(thisPos.x, thisPos.z));
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);

        for (int i = -_globalUnitSettings.LineOfSightRaysPerSide; i <= _globalUnitSettings.LineOfSightRaysPerSide; i++)
        {
            float t = (float)i / (float)_globalUnitSettings.LineOfSightRaysPerSide;

            Ray ray = new Ray(transform.position + perpendicular * (t * _globalUnitSettings.LineOfSightRayWidth), direction);

            int count = Physics.RaycastNonAlloc(ray, _hit, dist, layerOverride.HasValue ? layerOverride.Value : _globalUnitSettings.LineOfSightLayer);

            for (int j = 0; j < count; j++)
            {
                if (_hit[j].rigidbody != rb)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void UpdatePathfinding()
    {
        if (_reachedTarget)
        {
            return;
        }

        bool forced = _forcePathUpdateIndex >= _globalUnitSettings.ForcePathUpdateIndex;

        if (((!_hasTargetLineOfSight && _updatePath) || forced) && !_pathfinding)
        {
            Pathfind(_token.Token);
            _updatePath = false;

            if (forced)
            {
                _forcePathUpdateIndex = 0;
            }
        }

        _forcePathUpdateIndex++;
    }

    protected virtual Entity DetermineEntityTarget()
    {
        _entityTargetList.Clear();

        Team oppositeTeam = Team == Team.Player ? Team.Enemy : Team.Player;
        if (AllUnits.TryGetValue(oppositeTeam, out var list))
        {
            _entityTargetList.AddRange(list);
        }

        Entity result = null;
        float closestDist = float.MaxValue;

        foreach (Entity entity in _entityTargetList)
        {
            float sqrDist = (transform.position - entity.transform.position).sqrMagnitude;

            if (sqrDist < _sqrChaseRange && sqrDist < closestDist)
            {
                closestDist = sqrDist;
                result = entity;
            }
        }

        return result;
    }

    protected virtual Vector3? DetermineTargetPos()
    {
        return _entityTarget != null ? _entityTarget.transform.position : null;
    }

    protected virtual bool TargetPosIsForced()
    {
        return false;
    }

    protected virtual void LateUpdate()
    {
        float delta = turnSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(_lookDirection.z, _lookDirection.x) * Mathf.Rad2Deg;

        _currentAngle = Mathf.LerpAngle(_currentAngle, angle, delta);

        Vector3 dir = new(Mathf.Cos(_currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(_currentAngle * Mathf.Deg2Rad));

        transform.forward = dir;

        // Reached Destination
        Vector3 pos = transform.position;
        pos.y = _targetPos.y;

        float sqrDist = (pos - _targetPos).sqrMagnitude;

        if (_entityTarget != null)
        {
            _entityTargetInRange = sqrDist <= _sqrRange + (_entityTarget.ExtraSize * _entityTarget.ExtraSize);
        }
        else
        {
            _entityTargetInRange = false;
        }

        _reachedTarget = (!TargetPosIsForced() && _entityTargetInRange) || sqrDist <= _globalUnitSettings.ReachedDestinationDist;

        if (_entityTargetInRange)
        {
            _lookDirection = _entityTarget.transform.position - transform.position;
            _lookDirection.y = 0;
            _lookDirection.Normalize();
        }

        /*
        _currentDirection = Vector3.Slerp(_currentDirection, _desiredDirection, turnSpeed * Time.deltaTime);
        _currentDirection.Normalize();

        transform.forward = _currentDirection;
        */
    }

    protected virtual void Attack()
    {
        _attackCooldown = attackDelay;

        SpawnAttack(attack);
    }

    protected void SpawnAttack(UnitAttack attack)
    {
        UnitAttack spawnedAttack = Instantiate(attack, transform.position, transform.rotation);
        spawnedAttack.User = this;
        spawnedAttack.Target = _entityTarget;

        spawnedAttack.SubscribeToDeath();
    }

    private async void Pathfind(CancellationToken token)
    {
        _pathfinding = true;

        try
        {
            var result = await _manager.FindPath(transform.position, _targetPos, token);

            if (result != null)
            {
                await Awaitable.MainThreadAsync();

                _path = result;
                _path.RemoveAt(0);
                _pathIndex = 0;
                _pathCount = Mathf.Max(_path.Count - 1, 0);
                _betterPoint = null;
                _findBetterPointFrame = 0;
            }
        }
        catch (OperationCanceledException)
        {

        }

        _pathfinding = false;
    }

    protected virtual void FixedUpdate()
    {
        float targetVelocity;
        Vector3 moveTo;
        Vector3 moveDirection;

        if (CanMove)
        {
            if (!_reachedTarget)
            {
                if (_hasTargetLineOfSight || _pathIndex >= _pathCount)
                {
                    moveTo = _targetPos;
                }
                else
                {
                    int index = _pathIndex;

                    // Find better path (should prevent going in circles)
                    if (_findBetterPointFrame <= 0 && _pathIndex < _pathCount)
                    {
                        _findBetterPointFrame = _globalUnitSettings.FindBetterPointFrameCount;

                        bool foundBetterPath = false;

                        for (int i = _pathIndex + 1; i < _pathCount; i++)
                        {
                            Vector3 pos = _path[i];

                            if (HasLineOfSight(pos, out _, _globalUnitSettings.ObstacleLayer))
                            {
                                foundBetterPath = true;
                                index = i;
                            }
                        }

                        if (foundBetterPath)
                        {
                            _pathIndex = index;
                        }
                        else if (_pathIndex < _pathCount - 1)
                        {
                            Vector3 p1 = _path[_pathIndex];
                            Vector3 p2 = _path[_pathIndex + 1];

                            float nextPointDist = Vector2.Distance(new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z));

                            int points = Mathf.RoundToInt(nextPointDist);

                            for (int i = 0; i < points; i++)
                            {
                                float t = (float)i / (float)points;

                                Vector3 point = Vector3.Lerp(p1, p2, t);

                                if (HasLineOfSight(point, out _, _globalUnitSettings.ObstacleLayer))
                                {
                                    _betterPoint = point;
                                }
                            }
                        }
                    }

                    _findBetterPointFrame--;

                    if (_betterPoint.HasValue)
                    {
                        moveTo = _betterPoint.Value;
                    }
                    else if (_pathCount > 0)
                    {
                        moveTo = _path[Mathf.Min(index, _pathCount - 1)];
                    }
                    else
                    {
                        moveTo = rb.position;
                    }
                }

                float sqrDist = (rb.position - moveTo).sqrMagnitude;
                bool reachedTarget = sqrDist <= _globalUnitSettings.ReachedPointDistSqr;

                if (reachedTarget)
                {
                    _pathIndex = Mathf.Clamp(_pathIndex + 1, 0, _pathCount);
                    _betterPoint = null;
                    _findBetterPointFrame = 0;
                }

                if (_hasTargetLineOfSight)
                {
                    targetVelocity = reachedTarget ? 0 : speed;
                }
                else
                {
                    targetVelocity = _pathIndex >= _pathCount ? 0 : speed;
                }
            }
            else
            {
                targetVelocity = 0;
                moveTo = _targetPos;
            }

            moveDirection = moveTo - rb.position;
            moveDirection.y = 0;
            moveDirection.Normalize();

            if (!_entityTargetInRange)
            {
                _lookDirection = moveDirection;
            }
        }
        else
        {
            targetVelocity = 0;

            moveDirection = _lookDirection;
        }

        float acceleration = GetAccelerationRate(_currentVelocity, targetVelocity, _accelerationRate, _decelerationRate);
        _currentVelocity = Mathf.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveDirection.x * _currentVelocity;
        velocity.z = moveDirection.z * _currentVelocity;

        rb.linearVelocity = velocity + _knockbackForce;

        _knockbackForce = Vector3.MoveTowards(_knockbackForce, Vector3.zero, _globalUnitSettings.UnitKnockbackDelta * Time.fixedDeltaTime);

        // Set animation variables
        SetAnimBool("Moving", targetVelocity != 0);

        if (targetVelocity != 0)
        {
            SetAnimFloat("Speed", targetVelocity);
        }
    }

    public override void Hurt(float damage, Entity from)
    {
        base.Hurt(damage, from);

        // Attack the unit that attacks this unit
        _entityTarget = from;
    }

    public override void Knockback(float force, Vector3 from)
    {
        if (knockbackResitance)
        {
            return;
        }

        Vector2 direction = rb.position - from;
        direction.y = 0;
        direction.Normalize();

        _knockbackForce = direction * force;
    }

    #region Animation parameters
    public void SetAnimTrigger(string name)
    {
        if (Animator != null)
            Animator.SetTrigger(name);
    }

    public void SetAnimBool(string name, bool b)
    {
        if (Animator != null)
            Animator.SetBool(name, b);
    }

    public void SetAnimFloat(string name, float f)
    {
        if (Animator != null)
            Animator.SetFloat(name, f);
    }
    #endregion

    public void TriggerAnimEvent(string name)
    {
        if (name == "Walk" && walkParticles != null)
        {
            walkParticles.Play();
        }

        OnAnimEvent?.Invoke(name);
    }

    private void SetAccelerationRate(ref float value, float timeToAccelerate, float distance = 1)
    {
        if (timeToAccelerate <= 0)
        {
            value = distance;
        }
        else
        {
            value = distance / timeToAccelerate;
        }

        // 60 fps
        value /= 1f / 60f;
    }

    private float GetAccelerationRate(float velocity, float targetVelocity, float acceleration, float deceleration)
    {
        if (Mathf.Abs(targetVelocity) < Mathf.Abs(velocity))
        {
            return deceleration == 1 ? 1 : deceleration * Time.deltaTime;
        }

        return acceleration == 1 ? 1 : acceleration * Time.deltaTime;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 startMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 0, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector3.zero, chaseRange);

        Gizmos.matrix = startMatrix;
    }

    private Vector3 _TEMP_REMOVEmoveTo;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3? previousPos = null;

        foreach (Vector3 point in _path)
        {
            if (previousPos.HasValue)
            {
                Gizmos.DrawLine(VisualsPlane.TransformPoint(previousPos.Value), VisualsPlane.TransformPoint(point));
            }

            previousPos = point;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(_TEMP_REMOVEmoveTo, 0.2f);
    }
#endif
}
