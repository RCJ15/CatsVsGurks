using UnityEngine;

public class ClickableTest : MonoBehaviour, IClickable
{
    public void OnClickDown()
    {
        Debug.Log("CLICK DOWN");
    }

    public void OnClickUp()
    {
        Debug.Log("CLICK UP");
    }

    public void OnDeselect()
    {
        Debug.Log("DESELECTED");
    }

    public void OnSelect()
    {
        Debug.Log("SELECTED");
    }
}
