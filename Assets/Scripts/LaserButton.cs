using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserButton : MonoBehaviour, IClickable
{

    [SerializeField] private bool isReset;
    [SerializeField] private GameObject menuManager;
    public void OnClickDown()
    {
        Debug.Log("LaserButton clicked down");
    }

    public void OnClickUp()
    {
        if (isReset)
        {
            ResetGame();
        } else
        {
            ResumeGame();
        }
        Debug.Log("LaserButton clicked up");
    }

    public void OnDeselect()
    {
        Debug.Log("LaserButton deselected");
    }

    private void Update()
    {
        Debug.Log("Position of left controller" + OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch));
    }

    public void OnSelect()
    {

        Debug.Log("LaserButton selected");
    }

    private void ResetGame()
    {
        Debug.ClearDeveloperConsole();
        Debug.Log("Resetting game...");
        // Ensure time scale is normal (in case the game was paused)
        Time.timeScale = 1f;

        // Reload the currently active scene to reset game state
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResumeGame()
    {
        
        Debug.Log("Resuming game...");
        menuManager.GetComponent<Menu>().OpenCloseMenu();
    }

}
