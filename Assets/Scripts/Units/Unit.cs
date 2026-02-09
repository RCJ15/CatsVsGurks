using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public abstract class Unit : MonoBehaviour
{
    private readonly Vector3[] _corners = new Vector3[100];
    private int _cornersLength;
    private int _cornerIndex = 0;

    public abstract Team Team { get; }

    [SerializeField] private Transform targetTemp;
    private Vector3 _targetPos;
    [Space]

    [SerializeField] private string unitName;

    #region Statistics
    public float MaxHP
    {
        get => hp;
        set
        {
            float difference = value - hp;
            hp = value;

            if (difference > 0)
            {
                _currentHp += difference;
            }

            if (_currentHp > hp)
            {
                hp = value;
            }
        }
    }

    public float HP
    {
        get => _currentHp;
        set
        {
            if (_currentHp == value)
            {
                return;
            }

            float difference = value - _currentHp;

            _currentHp = value;
        }
    }

    public float Defense
    {
        get => defense;
        set => defense = value;
    }

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
    [Tooltip("Hitpoints, self explanatory")]
    [SerializeField] private float hp = 100;
    [Tooltip("All damage this unit takes is subtracted by this number")]
    [SerializeField] private float defense = 0;

    [Space]
    [Tooltip("How many tiles per second this unit moves")]
    [SerializeField] private float speed = 1;
    [Tooltip("How many seconds it takes for this unit to accelerate to max speed")]
    [SerializeField] private float timeToAccelerate = 0.1f;
    private float _accelerationRate;
    [Tooltip("How many seconds it takes for this unit to decelerate to zero speed")]
    [SerializeField] private float timeToDecelerate = 0.1f;
    private float _decelerationRate;
    private float _currentVelocity;

    [Space]
    [Tooltip("Damage, self explanatory")]
    [SerializeField] private float damage = 25;
    [Tooltip("Seconds of delay between each attack")]
    [SerializeField] private float attackDelay = 0.5f;
    [Tooltip("How close/far a unit needs to be before it'll attempt to attack")]
    [SerializeField] private float range = 1;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent agent;

    private float _currentHp;
    protected NavMeshPath path => agent.path;

    private Coroutine _pathCoroutine;

    private GlobalUnitSettings _globalUnitSettings;

    protected virtual void Awake()
    {
        _currentHp = hp;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.isStopped = true;

        UpdateAccelerationRates();

        _corners[0] = rb.position;
    }

    protected virtual void Start()
    {
        _globalUnitSettings = GlobalUnitSettings.Instance;
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

    protected virtual void Update()
    {
        // TEMPORARY METHOD OF TARGET POSITION
        if (targetTemp.position != _targetPos)
        {
            _targetPos = targetTemp.position;
            agent.Warp(rb.position);

            Debug.Log("Set destination");

            Vector3 target = _targetPos;

            if (NavMesh.SamplePosition(_targetPos, out NavMeshHit hit, _globalUnitSettings.SampleSearchDistance, NavMesh.AllAreas))
            {
                target = hit.position;
            }

            agent.SetDestination(target);

            if (_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
            }

            _pathCoroutine = StartCoroutine(AwaitPath());

            agent.isStopped = true;
        }

        DrawPath();

        /*
        if (targetTemp.position != _targetPos)
        {
            _targetPos = targetTemp.position;

            if (NavMesh.CalculatePath(transform.position, _targetPos, NavMesh.AllAreas, _path))
            {
                _cornerIndex = 0;

                _cornersLength = _path.GetCornersNonAlloc(_corners);
            }
        }

        if (_path.status == NavMeshPathStatus.PathInvalid)
        {
            return;
        }

        if (_cornerIndex >= _cornersLength)
        {
            return;
        }

        Vector3 target = _corners[_cornerIndex];

        transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        float sqrDist = (transform.position - target).sqrMagnitude;

        if (sqrDist <= _globalUnitSettings.CornerDistSqr)
        {
            _cornerIndex++;
        }
        */
    }

    private void DrawPath()
    {
        Vector3 previousCorner = _corners[0];

        for (int i = 1; i < _cornersLength; i++)
        {
            Vector3 corner = _corners[i];

            Debug.DrawLine(VisualsPlane.TransformPoint(previousCorner), VisualsPlane.TransformPoint(corner), Color.green);

            previousCorner = corner;
        }
    }

    private IEnumerator AwaitPath()
    {
        yield return new WaitUntil(() => !agent.pathPending);

        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            _cornerIndex = 0;
            _cornersLength = 0;
        }

        _cornerIndex = 0;
        _cornersLength = path.GetCornersNonAlloc(_corners);

        Debug.Log("Got path");
    }

    protected virtual void FixedUpdate()
    {
        float targetVelocity = _cornerIndex >= _cornersLength ? 0 : speed;

        float acceleration = GetAccelerationRate(_currentVelocity, targetVelocity, _accelerationRate, _decelerationRate);
        _currentVelocity = Mathf.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 targetPos = _corners[Mathf.Min(_cornerIndex, _cornersLength)];

        rb.linearVelocity = (targetPos - rb.position).normalized * _currentVelocity;

        float sqrDist = (rb.position - targetPos).sqrMagnitude;

        if (sqrDist <= _globalUnitSettings.DestinationDistSqr)
        {
            _cornerIndex++;
        }
    }

    public virtual void Hurt(float damage)
    {
        HP -= Mathf.Max(damage - defense, 1);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_corners[_cornerIndex], 0.1f);
    }
}
