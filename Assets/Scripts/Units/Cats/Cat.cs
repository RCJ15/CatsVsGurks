using UnityEngine;

public class Cat : Unit
{
    public override Team Team => Team.Player;

    private LaserPointer _laserPointer;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        _laserPointer = LaserPointer.Instance;
    }

    protected override Vector3 DetermineTarget()
    {
        return LaserPointer.Position;
    }
}
