using UnityEngine;
using Meta.XR;
using UnityEngine.UIElements;

public class FollowPointer : MonoBehaviour
{
    public Transform rightControllerAnchor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = rightControllerAnchor.position;
        Vector3 rotation = rightControllerAnchor.forward;
       if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            Debug.Log("Trigger Pressed");
        }
        Debug.Log("Position: " + position);
        Debug.Log("Rotation: " + rotation);
    }
}
