using UnityEngine;
using UnityEngine.Rendering;

public class TransformVisuals : MonoBehaviour
{
    private Transform _parent;
    private Transform _transform;

    private Vector3 _offset;
    private Vector3 _startScale;

    private ParticleSystem[] _particles;

    private void Start()
    {
        _transform = transform;
        _offset = _transform.localPosition;
        _parent = _transform.parent;
        _startScale = _transform.localScale;

        _transform.SetParent(null, true);

        _particles = GetComponentsInChildren<ParticleSystem>(true);
    }

    private void LateUpdate()
    {
        if (VisualsPlane.Instance == null) return;
        if (SimulationPlane.Instance == null) return;

        if (_parent == null)
        {
            Destroy(gameObject);
            return;
        }

        _transform.position = VisualsPlane.TransformPoint(_parent.position + _offset);
        _transform.rotation = VisualsPlane.TransformRotation(_parent.forward, _parent.up);
        Vector3 scale = VisualsPlane.TransformScale(Vector3.Scale(_parent.lossyScale, _startScale));
        _transform.localScale = scale;

        foreach (ParticleSystem particles in _particles)
        {
            particles.transform.localScale = scale;
        }
    }
}
