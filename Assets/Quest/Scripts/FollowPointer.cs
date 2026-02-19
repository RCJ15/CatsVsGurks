using System.Collections;
using UnityEngine;
using Meta.XR;
using System;

public class FollowPointer : MonoBehaviour
{
    public Transform rightControllerAnchor;

    [Header("Ray settings")]
    public float maxDistance = 10f;
    public float showDuration = 0.5f;
    public Color lineColor = Color.green;
    public float lineWidth = 0.01f;
    public Material lineMaterial; // assign a URP-friendly unlit material in the inspector

    private LineRenderer lineRenderer;
    private LineRenderer innerLineRenderer;
    private Coroutine hideRoutine;

    private GlobalUnitSettings _unitSettings;

    void Start()
    {
        _unitSettings = GlobalUnitSettings.Instance;

        // If no material assigned, try to pick a URP-friendly fallback shader.
        // Prefer an editor-created material assigned in the inspector to avoid shader compile issues at build time.
        Material baseMat = GetSafeMaterial();
        if (baseMat == null)
        {
            Debug.LogWarning("FollowPointer: No safe material available. Please assign a simple Unlit material (URP: 'Universal Render Pipeline/Unlit') to 'lineMaterial' in the inspector.");
            return;
        }

        // Create the outer LineRenderer
        if (lineRenderer == null)
        {
            var go = new GameObject("PointerRay_Outer");
            go.transform.SetParent(null);

            lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            // Use a copy of the base material so each renderer has its own instance
            lineRenderer.material = new Material(baseMat);
            ApplyColorToMaterial(lineRenderer.material, lineColor);

            lineRenderer.numCapVertices = 2;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.enabled = false;
        }

        // Create the inner LineRenderer (visual only)
        if (innerLineRenderer == null)
        {
            var inner = new GameObject("PointerRay_Inner");
            inner.transform.SetParent(null);

            innerLineRenderer = inner.AddComponent<LineRenderer>();
            innerLineRenderer.positionCount = 2;
            innerLineRenderer.useWorldSpace = true;

            float innerWidth = Mathf.Max(0f, lineWidth * 0.5f);
            innerLineRenderer.startWidth = innerWidth;
            innerLineRenderer.endWidth = innerWidth;

            innerLineRenderer.material = new Material(baseMat);
            ApplyColorToMaterial(innerLineRenderer.material, lineColor);

            innerLineRenderer.numCapVertices = 2;
            innerLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            innerLineRenderer.receiveShadows = false;
            innerLineRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (rightControllerAnchor == null)
            return;

        Vector3 position = rightControllerAnchor.position;
        Vector3 forward = rightControllerAnchor.forward;

        position = SimulationPlane.TransformPoint(position);
        forward = SimulationPlane.TransformDirection(forward);

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            Debug.Log("Trigger Pressed");
            ShowRay(position, forward);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.B))
        {
            tempDeleteLaterPls(position, forward);
        }
    }

    private void tempDeleteLaterPls(Vector3 origin, Vector3 direction)
    {
        if (lineRenderer == null || innerLineRenderer == null)
            return;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        Vector3 endPoint = origin + direction * maxDistance;

        if (Physics.Raycast(ray, out hit, maxDistance, _unitSettings.ObstacleLayer))
        {
            endPoint = hit.point;

            FindFirstObjectByType<TowerPlaceButton>().tempPlaceTower(endPoint);
        }
    }

    private void ShowRay(Vector3 origin, Vector3 direction)
    {
        if (lineRenderer == null || innerLineRenderer == null)
            return;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        Vector3 endPoint = origin + direction * maxDistance;
            
        if (Physics.Raycast(ray, out hit, maxDistance, _unitSettings.ObstacleLayer))
        {
            endPoint = hit.point;
            LaserPointer.Position = endPoint;
            
            Debug.Log($"Ray hit: {hit.collider.name} at {endPoint}");

            /*
            // Outer line
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, endPoint);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.enabled = true;

            // Inner line (visual only)
            float innerWidth = Mathf.Max(0f, lineWidth * 0.5f);
            innerLineRenderer.SetPosition(0, origin);
            innerLineRenderer.SetPosition(1, endPoint);
            innerLineRenderer.startWidth = innerWidth;
            innerLineRenderer.endWidth = innerWidth;
            innerLineRenderer.enabled = true;

            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            hideRoutine = StartCoroutine(HideAfter(showDuration));
            */
        }
        else
        {
            /*
            Debug.Log("Raycast did not hit anything");
            if (lineRenderer.enabled)
                lineRenderer.enabled = false;
            if (innerLineRenderer.enabled)
                innerLineRenderer.enabled = false;

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            */
        }
    }

    private IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (lineRenderer != null)
            lineRenderer.enabled = false;
        if (innerLineRenderer != null)
            innerLineRenderer.enabled = false;
        hideRoutine = null;
    }

    // Attempts to provide a safe material for URP builds. Prefer inspector-assigned material.
    private Material GetSafeMaterial()
    {
        if (lineMaterial != null)
            return lineMaterial;

        // Try URP Unlit first (recommended for reduced shader variants/compilation)
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
        if (urpUnlit != null)
        {
            var m = new Material(urpUnlit);
            ApplyColorToMaterial(m, lineColor);
            return m;
        }

        // Fallback to Sprites/Default (may not exist in URP projects)
        Shader sprites = Shader.Find("Sprites/Default");
        if (sprites != null)
        {
            var m = new Material(sprites);
            ApplyColorToMaterial(m, lineColor);
            return m;
        }

        // Last resort: try a simple internal colored shader (may not be present under URP either)
        Shader internalColored = Shader.Find("Hidden/Internal-Colored");
        if (internalColored != null)
        {
            var m = new Material(internalColored);
            ApplyColorToMaterial(m, lineColor);
            return m;
        }

        // If nothing workable found, return null so caller can warn and avoid creating renderers that may trigger build issues.
        return null;
    }

    // Apply color to material using common property names
    private void ApplyColorToMaterial(Material mat, Color color)
    {
        if (mat == null)
            return;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color); // URP
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);     // Legacy
        else if (mat.HasProperty("_TintColor"))
            mat.SetColor("_TintColor", color);
        else
            mat.color = color;
    }
}
