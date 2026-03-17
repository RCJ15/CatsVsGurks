using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BuyTowerButton : MonoBehaviour, IClickable
{
    public bool CanBuy => Player.Money >= tower.Cost;

    private Collider _collider;
    [SerializeField] private Transform visuals;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private CanvasGroup text;

    [Space]
    [SerializeField] private Tower tower;
    [SerializeField] private TowerPreview towerPreview;

    private LaserPointer _laserPointer;

    public void OnClickDown()
    {

    }

    public void OnClickUp()
    {
        visuals.DOKill();
        visuals.localScale = Vector3.one * 1.2f;
        visuals.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

        if (!CanBuy)
        {
            Debug.Log("NOT ENOUGH MONEY!!!");
            return;
        }

        // Buy tower
        Player.Money -= tower.Cost;

        _laserPointer.TowerToPlace = tower;
        _laserPointer.TowerPreview = Instantiate(towerPreview);
    }

    public void OnSelect()
    {
        text.DOKill();
        text.DOFade(1, 0.5f);
    }

    public void OnDeselect()
    {
        text.DOKill();
        text.DOFade(0, 0.5f);
    }

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>(true);
    }

    private void Start()
    {
        _laserPointer = LaserPointer.Instance;

        Player.OnChangeMoney += OnChangeMoney;

        UpdateColor();
    }

    private void OnDestroy()
    {
        Player.OnChangeMoney -= OnChangeMoney;
    }

    private void OnChangeMoney(int oldValue, int newValue)
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (sprite == null) return;
        sprite.color = CanBuy ? Color.white : Color.red;
    }

    public void Toggle(bool visible, bool instant = false)
    {
        _collider.enabled = visible;

        transform.DOKill();

        Vector3 size = visible ? Vector3.one : Vector3.zero;
        Ease ease = visible ? Ease.OutSine : Ease.InSine;

        if (instant)
        {
            transform.localScale = size;
        }
        else
        {
            transform.DOScale(size, 0.5f).SetEase(ease);
        }
    }
}
