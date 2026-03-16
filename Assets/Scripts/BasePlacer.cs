using UnityEngine;

public class BasePlacer : MonoBehaviour
{

    [SerializeField] private GameObject targetObject;

    private bool justStarted;

    private void placeBace()
    {

        Vector3 newPos = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 0.47f*gameObject.transform.localScale.x, targetObject.transform.position.z);

        gameObject.transform.position = newPos;
        Debug.Log("Has changed base to new position" + gameObject.transform.position);
        //gameObject.transform.rotation = targetObject.transform.rotation;
    }

    private void Start()
    {

        justStarted = true;
    }


    private void Update()
    {
        Debug.Log(targetObject.transform.position);

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            justStarted = false;
            placeBace();
        }
    }
}
