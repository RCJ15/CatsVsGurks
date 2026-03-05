using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour, IClickable
{
    public static Dictionary<Collider, ClickableObject> ColliderToClickable { get; private set; } = new();

    [SerializeField] private Collider col;
    [SerializeField] private Component[] clickables;
    private IClickable[] _clickables;

    private void OnEnable()
    {
        if (col == null) return;
        ColliderToClickable.Add(col, this);
    }

    private void OnDisable()
    {
        if (col == null) return;
        ColliderToClickable.Remove(col);
    }

    private void Start()
    {
        int length = clickables.Length;
        _clickables = new IClickable[length];

        for (int i = 0; i < length; i++)
        {
            _clickables[i] = (IClickable)clickables[i];
        }
    }

    public void OnSelect()
    {
        foreach (IClickable clickable in _clickables)
        {
            clickable.OnSelect();
        }
    }

    public void OnDeselect()
    {
        foreach (IClickable clickable in _clickables)
        {
            clickable.OnDeselect();
        }
    }

    public void OnClickDown()
    {
        foreach (IClickable clickable in _clickables)
        {
            clickable.OnClickDown();
        }
    }

    public void OnClickUp()
    {
        foreach (IClickable clickable in _clickables)
        {
            clickable.OnClickUp();
        }
    }
}
