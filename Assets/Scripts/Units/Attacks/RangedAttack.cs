using System.Collections;
using UnityEngine;

public class RangedAttack : UnitAttack
{
    [SerializeField] private Transform projectile;
    [SerializeField] private float colliderActiveTime;
    [SerializeField] private float travelTime;
    [SerializeField] private float heightMultiplier = 0.5f;
    private float _distance;
    private float _t;
    private float _speed;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    private bool _enableHitbox;
    private float _enableTime;

    private void Start()
    {
        _speed = 1f / travelTime;

        User.CanMove = false;
        User.SetAnimTrigger("Attack");

        _distance = Vector3.Distance(User.transform.position, Target.transform.position);

        // Predict future position
        Rigidbody rb = Target.GetComponentInChildren<Rigidbody>(true);
        _startPos = User.transform.position;
        _targetPos = Target.transform.position;

        if (rb != null)
        {
            _targetPos += rb.linearVelocity * travelTime;
        }
    }

    private void Update()
    {
        if (_enableHitbox)
        {
            _enableTime -= Time.deltaTime;

            if (_enableTime <= 0)
            {
                Destroy(gameObject);
            }

            return;
        }

        float x = Mathf.Lerp(_startPos.x, _targetPos.x, _t);
        float z = Mathf.Lerp(_startPos.z, _targetPos.z, _t);

        float y = 
            Mathf.Lerp(_startPos.z, _targetPos.y, _t) 
            + 
            (Mathf.Sin(Mathf.Lerp(0, 180 * Mathf.Deg2Rad, _t)) * _distance * heightMultiplier);

        projectile.position = new(x, y, z);

        _t += Time.deltaTime * _speed;

        if (_t >= 1)
        {
            _enableHitbox = true;
            _enableTime = colliderActiveTime;
            EnableCollider();

            projectile.gameObject.SetActive(false);
            return;
        }
    }

    /*
    private ThrowData GetPredictedPositionThrowData(ThrowData DirectThrowData)
    {
        Vector3 throwVelocity = DirectThrowData.ThrowVelocity;
        throwVelocity.y = 0;
        float time = DirectThrowData.DeltaXZ / throwVelocity.magnitude;
        Vector3 playerMovement;

        if (MovementPredictionMode == PredictionMode.CurrentVelocity)
        {
            playerMovement = PlayerCharacterController.velocity * time;
        }
        else
        {
            Vector3[] positions = HistoricalPositions.ToArray();
            Vector3 averageVelocity = Vector3.zero;
            for (int i = 1; i < positions.Length; i++)
            {
                averageVelocity += (positions[i] - positions[i - 1]) / HistoricalPositionInterval;
            }
            averageVelocity /= HistoricalTime * HistoricalResolution;
            playerMovement = averageVelocity;

        }

        Vector3 newTargetPosition = new Vector3(
            Target.position.x + PlayerCharacterController.center.x + playerMovement.x,
            Target.position.y + PlayerCharacterController.center.y + playerMovement.y,
            Target.position.z + PlayerCharacterController.center.x + playerMovement.z
        );

        // Option Calculate again the trajectory based on target position
        ThrowData predictiveThrowData = CalculateThrowData(
            newTargetPosition, 
            AttackProjectile.position
        );

        predictiveThrowData.ThrowVelocity = Vector3.ClampMagnitude(
            predictiveThrowData.ThrowVelocity, 
            MaxThrowForce
        );

        return predictiveThrowData;
    }

    */
}
