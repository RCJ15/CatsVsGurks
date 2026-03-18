using System;
using UnityEngine;

public class VisualsPlane : MonoBehaviour
{
    public static VisualsPlane Instance { get; private set; }

    public static Transform Transform { get; private set; }

    [SerializeField] private Vector2 scaleRange;

    [SerializeField] private GameObject visualizePlane;
    [SerializeField] private GameObject tutorial;

    public bool FieldPlaced => _fieldPlaced;
    private bool _fieldPlaced = false;

    private void Awake()
    {
        Instance = this;

        Transform = transform;
    }

    private void Update()
    {
        visualizePlane.gameObject.SetActive(!_fieldPlaced);

        if (_fieldPlaced)
            return;

        float scale;
        if (HiveMQSubscriber.Instance == null)
        {
            //Debug.LogWarning("VisualsPlane: HiveMQSubscriber.Instance is null. Ensure a HiveMQSubscriber exists in the scene and its Awake() ran.");
            scale = Mathf.Lerp(scaleRange.x, scaleRange.y, 0.6f); // Default to mid-range if HiveMQSubscriber is not available
            //Debug.Log("Hej Vi scalea nyss");

        }
        else
        {
            //Debug.Log("Hej VARFÖR ÄR VI HÄR STOP STOP STOP");

            float potValue = Mathf.Clamp01(HiveMQSubscriber.Instance.Connected ? HiveMQSubscriber.Instance.PotValue : 0.6f);
            scale = Mathf.Lerp(scaleRange.x, scaleRange.y, potValue);

        }

        transform.localScale = scale * Vector3.one;

        //Debug.Log("Hej Checking right trigger now");

        if (tutorial.GetComponent<TutorialText>().currentTextIndex == 3 && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            //Debug.Log("HEJ!");
            _fieldPlaced = true;

            if (tutorial == null)
            {
                Debug.LogError("VisualsPlane: 'tutorial' GameObject reference is not assigned in the Inspector.");
                return;
            }

            var tutComp = tutorial.GetComponent<TutorialText>();
            if (tutComp == null)
            {
                Debug.LogError("VisualsPlane: TutorialText component not found on 'tutorial' GameObject.");
                return;
            }

            StartCoroutine(tutComp.Spawn());
        }
    }

    public static Vector3 TransformPoint(Vector3 point)
    {
        return Transform.TransformPoint(SimulationPlane.Transform.InverseTransformPoint(point));
    }

    public static Vector3 TransformDirection(Vector3 direction)
    {
        return Transform.TransformDirection(SimulationPlane.Transform.InverseTransformDirection(direction));
    }

    public static Vector3 TransformScale(Vector3 scale)
    {
        return Vector3.Scale(Transform.localScale, scale);
    }

    public static Quaternion TransformRotation(Vector3 forward, Vector3 up)
    {
        forward = TransformDirection(forward);
        up = TransformDirection(up);

        return Quaternion.LookRotation(forward, up);
    }
}
