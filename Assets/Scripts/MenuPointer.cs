using UnityEngine;

public class MenuPointer : MonoBehaviour
{
    public float maxDistance = 100f;
    public LayerMask layerMask = ~0; // All layers by default
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;


    [SerializeField] private OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;
    [SerializeField] private GameObject menu;


    private void Update()
    {
        //if (menu.GetComponent<Menu>().isOpen && OVRInput.GetDown(triggerButton))

        if (OVRInput.GetDown(triggerButton))
        {
            ShootRay();
        }
    }

    private void ShootRay()
    {
        Debug.Log("Shooting ray from " + transform.position + " in direction " + transform.forward);


        Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.green, 0.01f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask.value, triggerInteraction))
        {
            var go = hit.collider.gameObject;

            // Prefer CompareTag for performance and clarity if you add tags to the objects.
            if (go.name == "RESUME_BLOCK")
            {
                for (int i = 0; i < 100; i++)
                {
                    Debug.Log("Hit resume button " + i);
                }
            }
            else if (go.name == "RESET_BLOCK")
            {
                for (int i = 0; i < 100; i++)
                {
                    Debug.Log("Hit resume button " + i);
                }
            }
        }
    }
}
