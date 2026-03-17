using UnityEngine;
using System.Collections;

public class OrbitalCannon : MonoBehaviour
{
    [Header("Cannon Settings")]
    public float cooldown = 10f;
    private float lastFireTime = -999f;

    [Header("Targeting")]
    public LayerMask unitLayer;             // Make sure your enemies are on this layer
    public float radius = 100f;             // Effective radius of the strike

    [Header("Effects")]
    public GameObject beamEffect;           // Optional visual effect
    public float delayBeforeImpact = 1.5f;  // Delay before cannon fires

    /// <summary>
    /// Call this to attempt firing the orbital cannon.
    /// Will respect cooldown.
    /// </summary>
    public bool TryFire()
    {
        if (Time.time < lastFireTime + cooldown)
        {
            Debug.Log("Cannon on cooldown");
            return false;
        }

        lastFireTime = Time.time;
        return true;
    }

    /// <summary>
    /// Handles the warning/delay before the cannon actually fires.
    /// </summary>
    public void FireSequence()
    {
        Debug.Log("Orbital strike incoming...");

        // Optional: spawn warning indicator here if you want

        

        Fire();
    }

    /// <summary>
    /// Executes the strike: destroys enemies and spawns visual effects.
    /// </summary>
    private void Fire()
    {
        //FIRE BEAM
    }

#if UNITY_EDITOR
    // Optional: visualize the strike radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}