using UnityEngine;

public class Entity : MonoBehaviour
{
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

    public virtual void Hurt(float damage, Unit from)
    {
        HP -= Mathf.Max(damage - defense, 1);
    }
}
