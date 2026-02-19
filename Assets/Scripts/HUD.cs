using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.PlayerLoop;
using System;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    [SerializeField] private TMP_Text moneyText;
    private string _moneyFormat;
    private float _moneyTextStartFontSize;
    [SerializeField] private float moneyCountDuration;
    private float _moneySpeed;
    private float _moneyPosition;
    [Space]
    [SerializeField] private float moneyTweenSize;
    [SerializeField] private float moneyTweenDuration;
    [SerializeField] private Ease moneyEase;

    [SerializeField] private TMP_Text waveText;
    private string _waveFormat;

    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    private string _healthFormat;

    [Space]
    [SerializeField] private Slider bosshealthSlider;
    [SerializeField] private TMP_Text bosshealthText;

    private int _moneyMax;
    private int _moneyMin;
    private int _currentMoney;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _moneyFormat = moneyText.text;
        _moneyTextStartFontSize = moneyText.fontSize;

        _currentMoney = Player.Money;
        _moneyMax = _currentMoney;
        _moneyMin = _moneyMax;

        _moneyPosition = 1;

        _moneySpeed = 1 / moneyCountDuration;

        UpdateMoneyText();

        Player.OnChangeMoney += OnChangeMoney;

        _waveFormat = waveText.text;
        UpdateWaveText();
        Player.OnChangeWave += OnChangeWave;

        _healthFormat = healthText.text;
        UpdateHP();

        Player.OnChangeHP += OnChangeHP;
        Player.OnChangeMaxHP += OnChangeMaxHP;
    }

    private void Update()
    {
        int money;

        if (_moneyPosition <= 0)
        {
            money = _moneyMin;
        }
        else if (_moneyPosition >= 1)
        {
            money = _moneyMax;
        }
        else
        {
            money = Mathf.RoundToInt(Mathf.Lerp(_moneyMin, _moneyMax, _moneyPosition));
        }

        if (_moneyPosition < 1)
        {
            _moneyPosition += _moneySpeed * Time.deltaTime;
        }

        if (_currentMoney != money)
        {
            _currentMoney = money;
            UpdateMoneyText();
        }
    }

    private void UpdateMoneyText()
    {
        moneyText.text = string.Format(_moneyFormat, _currentMoney.ToString("N0"));
    }

    private void OnChangeMoney(int oldMoney, int newMoney)
    {
        _moneyMin = _currentMoney;
        _moneyMax = newMoney;

        _moneyPosition = 0;

        if (newMoney > oldMoney)
        {
            moneyText.DOKill();
            moneyText.color = Color.yellow;
            moneyText.DOColor(Color.white, moneyTweenDuration);

            moneyText.fontSize = _moneyTextStartFontSize * moneyTweenSize;
            DOTween.To(() => moneyText.fontSize, (v) => moneyText.fontSize = v, _moneyTextStartFontSize, moneyTweenDuration).SetEase(moneyEase).SetTarget(moneyText);
        }
    }

    private void UpdateWaveText()
    {
        waveText.text = string.Format(_waveFormat, Player.Wave);
    }

    private void OnChangeWave(int oldWave, int newWave)
    {
        UpdateWaveText();
    }

    private void UpdateHP()
    {
        healthSlider.maxValue = Player.MaxHP;
        healthSlider.value = Player.HP;

        healthText.text = string.Format(_healthFormat, Mathf.Ceil(Player.HP), Mathf.Ceil(Player.MaxHP));
    }

    private void OnChangeMaxHP(float oldHP, float newHP)
    {
        UpdateHP();
    }

    private void OnChangeHP(float oldHealth, float newHealth)
    {
        UpdateHP();
    }
}
