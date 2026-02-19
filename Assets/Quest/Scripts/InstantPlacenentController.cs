using Meta.XR;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class InstantPlacementController : MonoBehaviour
{
    public Transform rightControllerAnchor;
    public GameObject prefabToPlace;
    public EnvironmentRaycastManager raycastManager;
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            Debug.Log("Trigger Pressed");
            var ray = new Ray(
                rightControllerAnchor.position,
                rightControllerAnchor.forward
            );
            Debug.Log("Hej");

            TryPlace(ray);
        }
    }
    private void TryPlace(Ray ray)
    {
        Debug.Log("Tjena");
        if (raycastManager == null)
        {
            Debug.LogError("EnvironmentRaycastManager reference (raycastManager) is null. Assign it in the Inspector.");
            return;
        }

        if (raycastManager.Raycast(ray, out var hit))
        {
            Debug.Log("tjoho");
            var objectToPlace = Instantiate(prefabToPlace);
            objectToPlace.transform.SetPositionAndRotation(
                hit.point,
                Quaternion.LookRotation(hit.normal, Vector3.up)
            );

            // If no MRUK component is present in the scene, we add an OVRSpatialAnchor component
            // to the instantiated prefab to anchor it in the physical space and prevent drift.
            if (MRUK.Instance?.IsWorldLockActive != true)
            {
                objectToPlace.AddComponent<OVRSpatialAnchor>();
                Debug.Log("Tjenare");
            }
            else
            {
                Debug.Log("Fel");
            }
        }
        else
        {
            Debug.Log($"Environment raycast returned false. Status: {hit.status}. " +
                      $"IsSupported: {EnvironmentRaycastManager.IsSupported}, " +
                      $"ManagerEnabled: {raycastManager.enabled}, ActiveInHierarchy: {raycastManager.gameObject.activeInHierarchy}. " +
                      $"Ray origin: {ray.origin}, dir: {ray.direction}");
        }
    }

}