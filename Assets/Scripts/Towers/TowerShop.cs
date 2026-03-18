using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class TowerShop : MonoBehaviour
{
    public static TowerShop Instance { get; private set; }

    public static bool Open => Instance == null ? false : Instance._open;

    private bool _open;
    [SerializeField] private float rotationDelta;

    [SerializeField] private TMP_Text moneyText;
    private string _moneyTextFormat;
    [SerializeField] private OVRInput.RawButton toggleShopInput;

    private BuyTowerButton[] _buttons;
    private Vector3 _position;
    private Quaternion _rotation;
    private LaserPointer _laserPointer;

    private TutorialText _text;

    private void Awake()
    {
        Instance = this;

        _text = FindAnyObjectByType<TutorialText>();
    }

    private void OnDestroy()
    {
        Player.OnChangeMoney -= OnChangeMoney;
    }

    private void OnChangeMoney(int oldValue, int newValue)
    {
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = string.Format(_moneyTextFormat, Player.Money);
    }

    private void Start()
    {
        _laserPointer = LaserPointer.Instance;
        _buttons = GetComponentsInChildren<BuyTowerButton>(true);

        foreach (var button in _buttons)
        {
            button.Toggle(false, true);
        }

        _moneyTextFormat = moneyText.text;
        UpdateMoneyText();

        Player.OnChangeMoney += OnChangeMoney;

        moneyText.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (OVRInput.GetDown(toggleShopInput) && _laserPointer.TowerPreview == null && _text.currentTextIndex >= 4

#if UNITY_EDITOR
            || UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame
#endif

            )
        {
            SetOpen(!_open);
        }

        // Follow
        transform.position = _position;
        transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, rotationDelta * Time.deltaTime);
    }

    public void SetOpen(bool open)
    {
        _open = open;

        if (_open)
        {
            _position = HeadPosition.Pos;
            _rotation = Quaternion.Euler(0, HeadPosition.EulerAngles.y, 0);
        }

        // On Open
        foreach (var button in _buttons)
        {
            button.Toggle(_open);
        }

        moneyText.transform.DOKill();
        moneyText.transform.DOScale(_open ? 1 : 0, 0.5f).SetEase(Ease.InOutSine);
    }
}
