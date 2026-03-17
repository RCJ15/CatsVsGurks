using UnityEngine;
using System.Collections;

public class OrbitalCannon : MonoBehaviour
{
    [Header("Cannon Settings")]
    public float cooldown = 30f;
    private float lastFireTime = -999f;

    [Header("Targeting")]
    public LayerMask unitLayer;             // Make sure your enemies are on this layer
    public float radius = 100f;             // Effective radius of the strike

    [Header("Effects")]
    public GameObject beamEffect;           // Optional visual effect
    public float delayBeforeImpact = 1.5f;  // Delay before cannon fires

    [Header("CameraTexts")]
    public Transform cameraTransform;
    public float offset = 2f;
    public int currentTextIndex = 1;
    [SerializeField] private GameObject T1;
    [SerializeField] private GameObject T2;
    [SerializeField] private GameObject T3;

    /// <summary>
    /// Call this to attempt firing the orbital cannon.
    /// Will respect cooldown.
    /// </summary>
    public bool TryFire()
    {
        if (Time.time < lastFireTime + cooldown)
        {

            Debug.Log("Cannon on cooldown");
            Vector3 forward = cameraTransform.forward.normalized;
            Vector3 targetPosition = cameraTransform.position + forward * offset + new Vector3(0, 0, 0);
            Quaternion targetRotation = Quaternion.LookRotation(forward);

            StartCoroutine(AnimateText(targetPosition, targetRotation));
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
    IEnumerator AnimateText(Vector3 targetPosition, Quaternion targetRotation)
    {
        float duration = 0.25f;
        float time = 0;

        Vector3 startPos = cameraTransform.position + cameraTransform.forward * (offset * 0.6f);
        Quaternion startRot = targetRotation;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}