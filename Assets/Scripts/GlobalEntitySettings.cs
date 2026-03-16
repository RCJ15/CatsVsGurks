using UnityEngine;

public class GlobalEntitySettings : MonoBehaviour
{
    public static GlobalEntitySettings Instance { get; private set; }

    public Material HurtMaterial => hurtMaterial;
    public float HurtDuration => hurtDuration;

    [SerializeField] private Material hurtMaterial;
    [SerializeField] private float hurtDuration;

    private void Awake()
    {
        Instance = this;
    }
}
