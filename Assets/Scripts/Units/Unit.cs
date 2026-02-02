using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public abstract Team Team { get; }

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
    [Tooltip("All damage this unit takes is subtracted by this number to a minimum of 1")]
    [SerializeField] private float defense = 0;

    [Space]
    [Tooltip("How many tiles per second this unit moves")]
    [SerializeField] private float speed = 1;

    [Space]
    [Tooltip("Damage, self explanatory")]
    [SerializeField] private float damage = 25;
    [Tooltip("Seconds of delay between each attack")]
    [SerializeField] private float attackDelay = 0.5f;
    [Tooltip("How close/far a unit needs to be before it'll attempt to attack")]
    [SerializeField] private float range = 1;

    [SerializeField] private UnitComponent[] components;

    private float _currentHp;

    protected virtual void Awake()
    {
        _currentHp = hp;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
