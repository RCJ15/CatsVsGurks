using System.Collections.Generic;
using UnityEngine;
using Tile = PathfindingManager.Tile;

public class PathfindingObstacle : MonoBehaviour
{
    public Type ShapeType { get => type; set => type = value; }
    public Vector2 Size { get => size; set => size = value; }
    public float Radius { get => radius; set => radius = value; }

    public bool Block { get => block; set => block = value; }
    public float Cost { get => cost; set => cost = value; }

    [SerializeField] private Type type;
    [SerializeField] private Vector2 size = Vector2.one;
    private Vector2 _halfSize;
    [SerializeField] private float radius = 0.5f;

    private Vector2 _position;
    private Vector2 _scale;
    private float _rot;

    [SerializeField] private bool block = true;
    [SerializeField] private float cost = 0;

    private Type _oldType;
    private Vector2 _oldSize;
    private float _oldRadius;

    private bool _oldBlock;
    private float _oldCost;

    private bool _appliedBlock;
    private float _appliedCost;

    private bool _applyThisFrame;

    private bool _applied;

    private PathfindingManager _manager;

    private List<Tile> _tiles = new();

    private void Start()
    {
        _manager = PathfindingManager.Instance;

        _position.x = transform.position.x;
        _position.y = transform.position.z;

        _rot = transform.eulerAngles.y;

        _scale = new(transform.localScale.x, transform.localScale.z);

        _oldType = type;
        _oldSize = size;
        _halfSize = size / 2f;
        _oldRadius = radius;

        _oldBlock = block;
        _oldCost = cost;

        StoreTiles();
        Apply();
    }

    private void OnEnable()
    {
        if (_manager == null)
        {
            return;
        }

        StoreTiles();
        Apply();
    }

    private void OnDisable()
    {
        TryRemove();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector2 scale = new(transform.localScale.x, transform.localScale.z);
        float rot = transform.eulerAngles.y;

        if (
            _position.x != pos.x || _position.y != pos.z || _rot != rot || _scale != scale
            || 
            _oldType != type || _oldSize != size || _oldRadius != radius
            )
        {
            _position.x = pos.x;
            _position.y = pos.z;

            _rot = rot;

            _scale = scale;

            _oldType = type;
            _oldSize = size;
            _halfSize = size / 2f;
            _oldRadius = radius;

            StoreTiles();
            _applyThisFrame = true;
        }

        if (_oldBlock != block || _oldCost != cost)
        {
            _oldBlock = block;
            _oldCost = cost;

            _applyThisFrame = true;
        }

        if (_applyThisFrame)
        {
            Apply();
            _applyThisFrame = false;
        }
    }

    private void StoreTiles()
    {
        TryRemove();

        _tiles.Clear();

        Vector2Int min;
        Vector2Int max;

        switch (type)
        {
            case Type.Box:
                Vector2 size = this.size * _scale;
                Vector2 halfSize = _halfSize * _scale;
                Vector2 bottomLeft;
                Vector2 topRight;

                // Regular
                if (_rot == 0 || _rot == 360)
                {
                    bottomLeft = _position - halfSize;
                    topRight = _position + halfSize;
                }
                // Inverted
                else if (_rot == 180)
                {
                    bottomLeft = _position + halfSize;
                    topRight = _position - halfSize;
                }
                // Any rotation
                else
                {
                    bottomLeft = new(Mathf.Infinity, Mathf.Infinity);
                    topRight = new(Mathf.NegativeInfinity, Mathf.NegativeInfinity);

                    Vector2[] points = new Vector2[]
                    {
                        _position - halfSize,
                        _position + halfSize,
                        _position + new Vector2(-halfSize.x, halfSize.y),
                        _position + new Vector2(halfSize.x, -halfSize.y),
                    };

                    for (int i = 0; i < 4; i++)
                    {
                        points[i] = PathfindingUtility.RotatePoint(points[i], _position, _rot);
                    }

                    foreach (var p in points)
                    {
                        if (p.x < bottomLeft.x) bottomLeft.x = p.x;
                        if (p.y < bottomLeft.y) bottomLeft.y = p.y;

                        if (p.x > topRight.x) topRight.x = p.x;
                        if (p.y > topRight.y) topRight.y = p.y;
                    }
                }

                min = _manager.WorldToGridFloor2D(bottomLeft);
                max = _manager.WorldToGridCeil2D(topRight);

                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        Tile tile = _manager.GetTile(x, y);

                        // Check if tile is in box
                        foreach (Vector2 p in tile.WorldCollisionPoints2D)
                        {
                            if (PathfindingUtility.PointInRotatedBox(p, _position, size, _rot))
                            {
                                _tiles.Add(tile);
                                break;
                            }
                        }
                    }
                }
                break;

            case Type.Circle:
                float scale = Mathf.Max(_scale.x, _scale.y);

                float radius = this.radius * scale;

                Vector2 offset = Vector2.one * radius;
                min = _manager.WorldToGridFloor2D(_position - offset);
                max = _manager.WorldToGridCeil2D(_position + offset);

                Vector2 tileSize = _manager.TileCollisionSize * Vector2.one;

                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        Tile tile = _manager.GetTile(x, y);

                        // Check if tile collides with circle
                        if (PathfindingUtility.CircleIntersectsBox(_position, radius, tile.WorldPoint2D, tileSize))
                        {
                            _tiles.Add(tile);
                        }
                    }
                }
                break;
        }
    }

    private void Apply()
    {
        TryRemove();

        _appliedBlock = block;
        _appliedCost = cost;

        foreach (Tile tile in _tiles)
        {
            if (_appliedBlock)
            {
                tile.BlockCount++;
            }
            tile.Cost += _appliedCost;
        }

        _applied = true;
    }

    private void TryRemove()
    {
        if (!_applied)
        {
            return;
        }

        Remove();
        _applied = false;
    }

    private void Remove()
    {
        foreach (Tile tile in _tiles)
        {
            if (_appliedBlock)
            {
                tile.BlockCount--;
            }
            tile.Cost -= _appliedCost;
        }
    }

    public enum Type
    {
        Box,
        Circle,
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Matrix4x4 startMatrix = Gizmos.matrix;

        switch (type)
        {
            case Type.Box:
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0, size.y));
                break;

            case Type.Circle:
                float matrixSize = Mathf.Max(transform.localScale.x, transform.localScale.z);

                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new(matrixSize, 0, matrixSize));

                Gizmos.DrawWireSphere(Vector3.zero, radius);
                break;
        }

        Gizmos.matrix = startMatrix;
    }
#endif
}
