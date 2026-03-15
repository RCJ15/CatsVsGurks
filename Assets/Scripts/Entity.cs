using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract Team Team { get; }
    public bool Targettable { get; set; } = true;

    public static readonly Dictionary<Collider, Entity> EntityColliders = new();
    public Collider Collider { get; private set; }

    #region Stats
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
    #endregion

    [Header("Health")]
    [Tooltip("Hitpoints, self explanatory")]
    [SerializeField] private float hp = 100;
    [Tooltip("All damage this entity takes is subtracted by this number")]
    [SerializeField] private float defense = 0;

    private float _currentHp;

    protected virtual void Awake()
    {
        _currentHp = hp;
    }

    protected virtual void OnEnable()
    {
        Collider = GetComponentInChildren<Collider>(true);

        if (Collider == null) return;
        EntityColliders.Add(Collider, this);
    }

    protected virtual void OnDisable()
    {
        if (Collider == null) return;
        EntityColliders.Remove(Collider);
    }

    public virtual void Hurt(float damage, Unit from)
    {
        HP -= Mathf.Max(damage - defense, 1);

        // DIE
        if (HP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Targettable = false;
        Destroy(gameObject);
    }

    public abstract void Knockback(float force, Vector3 from);
}
