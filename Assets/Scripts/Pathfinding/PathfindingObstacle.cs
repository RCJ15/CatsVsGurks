using UnityEngine;

public class PathfindingObstacle : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    private Vector2 _position;

    private PathfindingManager _manager;

    private void Start()
    {
        _manager = PathfindingManager.Instance;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (_position.x == pos.x && _position.y == pos.z)
        {
            return;
        }

        _position.x = pos.x;
        _position.y = pos.z;


    }
}
