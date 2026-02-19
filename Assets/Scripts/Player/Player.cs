using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    #region Events
    public delegate void ChangeValueEvent<T>(T oldValue, T newValue);
    public static ChangeValueEvent<float> OnChangeMaxHP { get; set; }
    public static ChangeValueEvent<float> OnChangeHP { get; set; }
    public static ChangeValueEvent<int> OnChangeMoney { get; set; }
    public static ChangeValueEvent<int> OnChangeWave { get; set; }
    #endregion

    #region Static Getters and Setters
    public static float MaxHP
    {
        get => Instance.hp;
        set
        {
            float oldValue = MaxHP;
            if (oldValue == value) return;

            float difference = value - Instance.hp;
            Instance.hp = value;

            if (difference > 0)
            {
                Instance._currentHp += difference;
            }

            if (Instance._currentHp > Instance.hp)
            {
                Instance.hp = value;
            }

            OnChangeMaxHP?.Invoke(oldValue, value);
        }
    }

    public static float HP
    {
        get => Instance._currentHp;
        set
        {
            float oldValue = HP;
            if (oldValue == value) return;

            float difference = value - Instance._currentHp;

            Instance._currentHp = value;

            OnChangeHP?.Invoke(oldValue, value);
        }
    }

    private float _currentHp;

    public static int Money
    {
        get => Instance.money;
        set
        {
            int oldValue = Money;
            if (oldValue == value) return;

            Instance.money = value;

            OnChangeMoney?.Invoke(oldValue, value);
        }
    }

    public static int Wave
    {
        get => Instance.wave;
        set
        {
            int oldValue = Wave;
            if (oldValue == value) return;

            Instance.wave = value;

            OnChangeWave?.Invoke(oldValue, value);
        }
    }
    #endregion

    [SerializeField] private float hp;
    [SerializeField] private int money;
    [SerializeField] private int wave;

    private void Awake()
    {
        _currentHp = hp;
        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Money += UnityEngine.Random.Range(50, 1000);
        }
        else if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            Money -= UnityEngine.Random.Range(50, 1000);
        }
        else if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            HP -= UnityEngine.Random.Range(1, 10);
        }
    }
}
