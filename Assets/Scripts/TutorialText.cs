using UnityEngine;
using System.Collections;

public class TutorialText : MonoBehaviour
{
    public Transform cameraTransform;
    public float offset = 2f;

    IEnumerator Start()
    {
        // Vänta tills VR-kameran hunnit initieras
        yield return new WaitForSeconds(1f);

        Vector3 forward = cameraTransform.forward.normalized;
        Vector3 targetPosition = cameraTransform.position + forward * offset + new Vector3(0, -0.2f, 0);
        Quaternion targetRotation = Quaternion.LookRotation(forward);

        StartCoroutine(AnimateMenu(targetPosition, targetRotation));
    }

    IEnumerator AnimateMenu(Vector3 targetPosition, Quaternion targetRotation)
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
