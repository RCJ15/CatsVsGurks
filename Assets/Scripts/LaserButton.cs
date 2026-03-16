using UnityEngine;

public class LaserButton : MonoBehaviour, IClickable
{
    public void OnClickDown()
    {
        Debug.Log("LaserButton.OnClickDown");
    }

    public void OnClickUp()
    {
        Debug.Log("LaserButton.OnClickUp");
    }

    public void OnDeselect()
    {
        Debug.Log("LaserButton.OnDeselect");
    }

    public void OnSelect()
    {
        Debug.Log("LaserButton.OnSelect");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
