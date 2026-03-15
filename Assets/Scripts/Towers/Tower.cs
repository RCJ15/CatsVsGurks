using UnityEngine;

public class Tower : Entity
{
    public int Cost => cost;

    // Only the player can place towers
    public override Team Team => Team.Player;

    [Header("Tower")]
    [SerializeField] protected int cost;

    public override void Knockback(float force, Vector3 from)
    {
        // Towers can't take knockback
    }
}
