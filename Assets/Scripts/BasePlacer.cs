using UnityEngine;

public class BasePlacer : MonoBehaviour
{

    [SerializeField] private GameObject targetObject;

    private bool justStarted;
    [SerializeField] private GameObject tutorial;

    [SerializeField] private GameObject catPrettay;

    [SerializeField] private GameObject camera;

    private void placeBace()
    {
        Debug.Log("hej bas start");
        Vector3 newPos = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 0.47f * gameObject.transform.localScale.x, targetObject.transform.position.z);

        gameObject.transform.position = newPos;
        Debug.Log("Has changed base to new position" + gameObject.transform.position);
        //gameObject.transform.rotation = targetObject.transform.rotation;

        StartCoroutine(tutorial.GetComponent<TutorialText>().Spawn());


        Vector3 spawnPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.11f, gameObject.transform.position.z);

        Quaternion spawnRot = Quaternion.Euler(0, targetObject.transform.rotation.eulerAngles.y, 0);

        // Compute rotation so the spawned cat looks at the player (camera), only yaw (no tilt)
        Quaternion quaternion = Quaternion.identity;

        Vector3 dirToPlayer = camera.transform.position - spawnPos;
        dirToPlayer.y = 0f; // keep only horizontal direction
        if (dirToPlayer.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
            quaternion = Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f);
        }
        else
        {
            quaternion = Quaternion.Euler(0, targetObject.transform.rotation.eulerAngles.y + 180, 0);
        }

        Vector3 forwardDir = spawnRot * Vector3.forward;
        spawnPos += forwardDir.normalized * 0.01f;

        if (catPrettay != null)
        {
            Instantiate(catPrettay, spawnPos, quaternion);
            targetObject.SetActive(false);
        }
        Debug.Log("hej bas slut");
    }

    private void Start()
    {

        justStarted = true;
    }


    private void Update()
    {
        //Debug.Log(targetObject.transform.position);

        if (justStarted && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {

            justStarted = false;
            placeBace();
        }
    }
}
