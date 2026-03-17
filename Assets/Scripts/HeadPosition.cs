using UnityEngine;

public class HeadPosition : MonoBehaviour
{
    public static Vector3 Pos { get; private set; }
    public static Vector3 EulerAngles { get; private set; }

    private void Update()
    {
        Pos = transform.position;
        EulerAngles = transform.eulerAngles;
    }
}
