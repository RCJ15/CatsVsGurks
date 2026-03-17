using UnityEngine;

public class BuyTowerButton : MonoBehaviour, IClickable
{
    [SerializeField] private Tower tower;
    [SerializeField] private TowerPreview towerPreview;

    private LaserPointer _laserPointer;

    public void OnClickDown()
    {
        if (Player.Money < tower.Cost)
        {
            Debug.Log("NOT ENOUGH MONEY!!!");
            return;
        }

        // Buy tower
        Player.Money -= tower.Cost;

        _laserPointer.TowerToPlace = tower;
        _laserPointer.TowerPreview = Instantiate(towerPreview);
    }

    public void OnClickUp()
    {

    }

    public void OnDeselect()
    {

    }

    public void OnSelect()
    {

    }

    private void Start()
    {
        _laserPointer = LaserPointer.Instance;
    }
}
