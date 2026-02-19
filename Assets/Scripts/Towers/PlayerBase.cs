using UnityEngine;

public class PlayerBase : Tower
{
    public static PlayerBase Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
    }
}
