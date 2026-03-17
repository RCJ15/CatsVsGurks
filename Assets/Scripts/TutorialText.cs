using UnityEngine;
using System.Collections;

public class TutorialText : MonoBehaviour
{
    public Transform cameraTransform;
    public float offset = 2f;
    private int currentTextIndex = 1;
    [SerializeField] private GameObject T1;
    [SerializeField] private GameObject T2;
    [SerializeField] private GameObject T3;

    private void Start()
    {
        Debug.Log("hej start");
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        Debug.Log("hej spawn");

        if (currentTextIndex == 1)
        {
            yield return new WaitForSeconds(1.5f);
            Debug.Log("hej t1");
            T1.SetActive(true);
            currentTextIndex++;
        }
        else if(currentTextIndex == 2)
        {
            T1.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Debug.Log("hej t2");
            T2.SetActive(true);
            currentTextIndex++;
        }
        else
        {
            T2.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            T3.SetActive(true);
        }

        Vector3 forward = cameraTransform.forward.normalized;
        Vector3 targetPosition = cameraTransform.position + forward * offset + new Vector3(0, 0, 0);
        Quaternion targetRotation = Quaternion.LookRotation(forward);

        StartCoroutine(AnimateMenu(targetPosition, targetRotation));
        Debug.Log("hej slut");
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
