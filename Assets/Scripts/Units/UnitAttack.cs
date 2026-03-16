using UnityEngine;

public abstract class UnitAttack : MonoBehaviour
{
    public Unit User { get; set; }
    public Entity Target { get; set; }

    [SerializeField] private Collider col;
    [SerializeField] private bool friendlyFire = false;
    [SerializeField] private float knockbackForce = 3f;

    protected virtual void Awake()
    {
        DisableCollider();
    }

    public void SubscribeToDeath()
    {
        if (User == null) return;
        User.OnDie += Die;
    }

    public void UnsubscribeToDeath()
    {
        if (User == null) return;
        User.OnDie -= Die;
    }

    protected void EnableCollider() => col.enabled = true;
    protected void DisableCollider() => col.enabled = false;

    protected virtual void OnTriggerEnter(Collider other)
    {
        // Check if collider is entity
        if (!Entity.EntityColliders.TryGetValue(other, out Entity entity))
        {
            return;
        }

        // Don't hit self
        if (entity == (Entity)User)
        {
            return;
        }

        // Don't hit entities on same team unless friendly fire is true
        if (!friendlyFire && entity.Team == User.Team)
        {
            return;
        }

        // Damage
        OnHitEntity(entity);
    }

    public virtual void Die()
    {
        UnsubscribeToDeath();
        Destroy(gameObject);
    }

    protected virtual void OnHitEntity(Entity entity)
    {
        entity.Hurt(User.Damage, User);
        entity.Knockback(knockbackForce, KnockbackPoint());
    }

    protected virtual Vector3 KnockbackPoint() => transform.position;
}
