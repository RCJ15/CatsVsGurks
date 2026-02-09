using UnityEngine;

public class TransformVisuals : MonoBehaviour
{
    private Transform _root;
    private Transform _transform;

    private Vector3 _offset;

    private void Start()
    {
        _transform = transform;
        _offset = _transform.localPosition;
        _root = _transform.root;
    }

    private void LateUpdate()
    {
        if (VisualsPlane.Instance == null) return;
        if (SimulationPlane.Instance == null) return;

        _transform.position = VisualsPlane.TransformPoint(_root.position + _offset);
        _transform.rotation = VisualsPlane.TransformRotation(_root.forward, _root.up);
    }
}
