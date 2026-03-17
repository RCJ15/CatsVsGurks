using UnityEngine;

public class HeadPosition : MonoBehaviour
{
    public static Vector3 Pos { get; private set; }

    private void Update()
    {
        Pos = transform.position;
    }
}
