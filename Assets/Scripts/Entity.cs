using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public abstract class Entity : MonoBehaviour
{
    public abstract Team Team { get; }
    public bool Targettable { get; set; } = true;

    public static readonly Dictionary<Collider, Entity> EntityColliders = new();
    public Collider Collider { get; private set; }

    public Action OnDie { get; set; }

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

    protected GlobalEntitySettings _globalEntitySettings;

    private Renderer[] _renderers;
    private int _meshRendersLength;
    private Material[][] _originalMaterials;
    private Material[][] _hurtMaterials;
    private Coroutine[] _hurtCoroutines;

    private float _currentHp;

    protected virtual void Awake()
    {
        _currentHp = hp;

        List<Renderer> renderers = new();
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            System.Type type = renderer.GetType();
            if (type != typeof(MeshRenderer) && type != typeof(SkinnedMeshRenderer))
            {
                continue;
            }

            renderers.Add(renderer);
        }

        _renderers = renderers.ToArray();
    }

    protected virtual void Start()
    {
        _globalEntitySettings = GlobalEntitySettings.Instance;

        _meshRendersLength = _renderers.Length;
        _originalMaterials = new Material[_meshRendersLength][];
        _hurtMaterials = new Material[_meshRendersLength][];

        for (int i = 0; i < _meshRendersLength; i++)
        {
            Renderer renderers = _renderers[i];

            if (renderers == null) continue;

            int materialsLength = renderers.materials.Length;
            _originalMaterials[i] = new Material[materialsLength];

            for (int j = 0; j < materialsLength; j++)
            {
                _originalMaterials[i][j] = renderers.materials[j];
            }

            _hurtMaterials[i] = new Material[materialsLength];

            for (int j = 0; j < materialsLength; j++)
            {
                _hurtMaterials[i][j] = _globalEntitySettings.HurtMaterial;
            }
        }

        _hurtCoroutines = new Coroutine[_meshRendersLength];
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
        else
        {
            foreach (Coroutine coroutine in _hurtCoroutines)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }

            _meshRendersLength = _renderers.Length;
            for (int i = 0; i < _meshRendersLength; i++)
            {
                _hurtCoroutines[i] = StartCoroutine(DoHurtAnim(i));
            }
        }
    }

    private IEnumerator DoHurtAnim(int i)
    {
        Renderer mr = _renderers[i];

        if (mr == null) yield break;

        mr.materials = _hurtMaterials[i];

        yield return new WaitForSeconds(_globalEntitySettings.HurtDuration);

        mr.materials = _originalMaterials[i];
    }

    public virtual void Die()
    {
        Targettable = false;
        OnDie?.Invoke();
        Destroy(gameObject);
    }

    public abstract void Knockback(float force, Vector3 from);
}
