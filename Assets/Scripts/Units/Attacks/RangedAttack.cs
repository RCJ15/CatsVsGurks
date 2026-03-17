using System.Collections;
using UnityEngine;

public class RangedAttack : UnitAttack
{
    [SerializeField] private Vector3 startOffset;
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform visuals;
    [SerializeField] private float colliderActiveTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float minimumTravelTime;
    [SerializeField] private float heightMultiplier = 0.5f;
    [SerializeField] private float minimumHeight;
    [SerializeField] private GameObject spawnOnDeath;

    private float _distance;
    private float _t;
    private float _speed;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    private bool _enableHitbox;
    private float _enableTime;
    private bool _projectileActive;
    private Vector3 _oldPosition;

    private float _travelTime;

    private void Start()
    {
        if (User == null || Target == null)
        {
            Die();
            return;
        }

        _distance = Vector3.Distance(User.transform.position, Target.transform.position);

        _travelTime = Mathf.Max(minimumTravelTime, _distance / projectileSpeed);

        _speed = 1f / _travelTime;

        User.SetAnimTrigger("Attack");
        User.OnAnimEvent += OnAnimEvent;

        projectile.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        User.OnAnimEvent -= OnAnimEvent;
    }

    private void Update()
    {
        if (!_projectileActive)
        {
            return;
        }

        if (_enableHitbox)
        {
            _enableTime -= Time.deltaTime;

            if (_enableTime <= 0)
            {
                Die();
            }

            return;
        }

        float x = Mathf.Lerp(_startPos.x, _targetPos.x, _t);
        float z = Mathf.Lerp(_startPos.z, _targetPos.z, _t);

        float y = 
            Mathf.Lerp(_startPos.y, _targetPos.y, _t) 
            + 
            (Mathf.Sin(Mathf.Lerp(0, 180 * Mathf.Deg2Rad, _t)) * Mathf.Max(minimumHeight, _distance * heightMultiplier));

        Vector3 newPos = new(x, y, z);

        visuals.transform.forward = newPos - _oldPosition;
        _oldPosition = newPos;

        projectile.position = newPos;

        _t += Time.deltaTime * _speed;

        if (_t >= 1)
        {
            _enableHitbox = true;
            _enableTime = colliderActiveTime;
            EnableCollider();

            if (spawnOnDeath != null)
            {
                GameObject obj = Instantiate(spawnOnDeath, projectile.position, Quaternion.identity);
                Destroy(obj, 10);
            }

            visuals.gameObject.SetActive(false);
            return;
        }
    }

    private void OnAnimEvent(string name)
    {
        if (name == "Attack")
        {
            User.OnAnimEvent -= OnAnimEvent;

            if (Target == null)
            {
                Die();
                return;
            }

            UnsubscribeToDeath();

            _projectileActive = true;
            projectile.gameObject.SetActive(true);

            // Predict future position
            Rigidbody rb = Target.GetComponentInChildren<Rigidbody>(true);
            _startPos = User.transform.position + Vector3.Scale(startOffset, User.transform.forward) + Vector3.Scale(startOffset, User.transform.up);
            _targetPos = Target.transform.position;

            _oldPosition = _startPos;

            if (rb != null)
            {
                _targetPos += rb.linearVelocity * _travelTime;
            }
        }
    }

    protected override Vector3 KnockbackPoint()
    {
        return projectile.position;
    }
}
