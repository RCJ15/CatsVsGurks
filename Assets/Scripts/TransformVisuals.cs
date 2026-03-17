using UnityEngine;
using UnityEngine.Rendering;

public class TransformVisuals : MonoBehaviour
{
    private Transform _parent;
    private Transform _transform;

    private Vector3 _offset;
    private Vector3 _startScale;

    private ParticleSystem[] _particles;
    private TrailRenderer[] _trailRenderers;
    private int _trailRenderersLength;
    private float[] _trailRenderersSizes;

    private void Start()
    {
        _transform = transform;
        _offset = _transform.localPosition;
        _parent = _transform.parent;
        _startScale = _transform.localScale;

        _transform.SetParent(null, true);

        _particles = GetComponentsInChildren<ParticleSystem>(true);

        _trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
        _trailRenderersLength = _trailRenderers.Length;
        _trailRenderersSizes = new float[_trailRenderersLength];

        for (int i = 0; i < _trailRenderersLength; i++)
        {
            _trailRenderersSizes[i] = _trailRenderers[i].widthMultiplier;
        }
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

        float scale1D = Mathf.Max(scale.x, scale.y, scale.z);

        for (int i = 0; i < _trailRenderersLength; i++)
        {
            _trailRenderers[i].widthMultiplier = _trailRenderersSizes[i] * scale1D;
        }
    }
}
