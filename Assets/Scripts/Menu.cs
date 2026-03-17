using UnityEngine;
using System.Collections;
using System;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public bool isOpen;

    //private AudioSource source;
    //public AudioClip openMenu;
    //public AudioClip closeMenu;

    public Transform cameraTransform;
    [SerializeField] private float offset = 2f;

    private void Start()
    {
        isOpen = false;
        panel.SetActive(false);
        //source = GetComponent<AudioSource>();
    }
    public void OpenCloseMenu()
    {
        if (isOpen)
        {
            isOpen = false;
            panel.SetActive(false);
            //source.PlayOneShot(closeMenu, 0.75f);
            Debug.Log("Closing MENU");

            Time.timeScale = 1.0f;
        }
        else
        {
            /*Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            panel.transform.position = cameraTransform.position + offset * forward;
            panel.transform.LookAt(cameraTransform);
            panel.transform.Rotate(0, 180, 0);
            panel.transform.position += new Vector3(0, -0.2f, 0);

            isOpen = true;
            panel.SetActive(true);
            source.PlayOneShot(openMenu, 0.4f);
            Debug.Log("Opening MENU");

            Time.timeScale = 0.0f;*/

            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 targetPosition = cameraTransform.position + forward * offset;
            targetPosition += new Vector3(0, -0.5f, 0);

            Quaternion targetRotation = Quaternion.LookRotation(forward);

            StartCoroutine(AnimateMenu(targetPosition, targetRotation));

            isOpen = true;
            //source.PlayOneShot(openMenu, 0.3f);
            Time.timeScale = 0.0f;
        }
    }
    IEnumerator AnimateMenu(Vector3 targetPosition, Quaternion targetRotation)
    {
        float duration = 0.25f;
        float time = 0;

        Vector3 startPos = cameraTransform.position + cameraTransform.forward * (offset * 0.6f);
        Quaternion startRot = targetRotation;

        panel.transform.position = startPos;
        panel.transform.rotation = startRot;

        panel.SetActive(true);

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = time / duration;

            panel.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            panel.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        panel.transform.position = targetPosition;
        panel.transform.rotation = targetRotation;
    }



    [SerializeField] private OVRInput.RawButton menuToggleButton;
    private void Update()
    {


        if (OVRInput.GetDown(menuToggleButton))
        {
            Console.WriteLine("Menu Toggle Button Pressed");
            OpenCloseMenu();
        }
    }


}