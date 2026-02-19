using UnityEngine;

public class Tower : Entity
{
    public int Cost => cost;

    [Header("Tower")]
    [SerializeField] protected int cost;
}
