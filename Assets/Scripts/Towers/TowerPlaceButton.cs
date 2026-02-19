using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TowerPlaceButton : MonoBehaviour
{
    public static bool PlacingTower { get; private set; }

    [SerializeField] private Tower tower;

    private Button _button;
    private bool _thisPlacingTower;

    private GlobalTowerSettings _globalTowerSettings;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Player.Money -= tower.Cost;

        _thisPlacingTower = true;
        PlacingTower = true;
    }

    private void Start()
    {
        _globalTowerSettings = GlobalTowerSettings.Instance;
    }

    private void Update()
    {
        if (_thisPlacingTower && _globalTowerSettings.PlacementInputAction.action.WasPressedThisFrame())
        {
            PlacingTower = false;
            _thisPlacingTower = false;
            Instantiate(tower, LaserPointer.Position, Quaternion.identity);
        }

        UpdateInteractable();
    }

    public void tempPlaceTower(Vector3 pos)
    {
        Instantiate(tower, pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    private void UpdateInteractable()
    {
        _button.interactable = !PlacingTower && Player.Money >= tower.Cost;
    }
}
