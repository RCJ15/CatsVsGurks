using UnityEngine;

public class Menu : MonoBehaviour
{
    private bool isOpen;
    [SerializeField] GameObject panel;

    private void Start()
    {
        isOpen = false;
        panel.SetActive(false);
    }
    public void OpenCloseMenu()
    {
        if (isOpen)
        {
            isOpen = false;
            panel.SetActive(false);
            Debug.Log("Closing MENU");

            Time.timeScale = 1.0f;
        }
        else
        {
            isOpen = true;
            panel.SetActive(true);
            Debug.Log("Opening MENU");

            Time.timeScale = 0.0f;
        }
    }
}
