using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    public static CameraInstance Instance { get; private set; }
    public static Camera Cam => Instance._camera;

    private Camera _camera;

    private void Awake()
    {
        Instance = this;

        _camera = GetComponent<Camera>();
    }
}
