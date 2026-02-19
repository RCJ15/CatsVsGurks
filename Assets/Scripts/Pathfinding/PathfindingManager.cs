using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    public const int RIGHT = 0;
    public const int DOWN = 1;
    public const int LEFT = 2;
    public const int UP = 3;

    public const int UP_RIGHT = 4;
    public const int UP_LEFT = 5;
    public const int DOWN_RIGHT = 6;
    public const int DOWN_LEFT = 7;

    public static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new(1, 0),
        new(0, -1),
        new(-1, 0),
        new(0, 1),

        new(1, 1),
        new(-1, 1),
        new(1, -1),
        new(-1, -1),
    };

    public float TileSize => tileSize;
    public float TileCollisionSize => tileSize * tileCollisionSize;
    public LayerMask ObstacleLayer => obstacleLayer;

    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float tileCollisionSize = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    [Space]
    [SerializeField] private int gridSize = 100;
    [SerializeField] private Vector2 gridOffset;
    private Vector3 _gridOrigin;
    private Vector2 _gridOrigin2D;

    private Tile[,] _tiles;

    private List<Tile> _path;

    private void Awake()
    {
        Instance = this;

        UpdateGridOrigin();

        // Create tiles
        _tiles = new Tile[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Tile tile = new Tile(x, y);
                _tiles[x, y] = tile;

                Vector2Int pos = new(x, y);

                bool xOver = x > 0;
                bool yOver = y > 0;

                Tile leftNeighbor = null;
                if (xOver)
                {
                    leftNeighbor = GetTile(pos + Directions[LEFT]);
                    tile.Neighbors[LEFT] = leftNeighbor;
                    leftNeighbor.Neighbors[RIGHT] = tile;
                }

                Tile downNeighbor = null;
                if (yOver)
                {
                    downNeighbor = GetTile(pos + Directions[DOWN]);
                    tile.Neighbors[DOWN] = downNeighbor;
                    downNeighbor.Neighbors[UP] = tile;
                }

                /* 8 way movement (not needed)
                if (xOver && yOver)
                {
                    Tile neighbor = GetTile(pos + Directions[DOWN_LEFT]);
                    tile.Neighbors[DOWN_LEFT] = neighbor;
                    neighbor.Neighbors[UP_RIGHT] = tile;

                    leftNeighbor.Neighbors[DOWN_RIGHT] = downNeighbor;
                    downNeighbor.Neighbors[UP_LEFT] = leftNeighbor;
                }
                */

                // 2D
                Vector2 worldPos2D = GridToWorld2D(pos);
                Vector2 halfSize2D = Vector2.one * tileSize / 2f;

                tile.WorldPoint2D = worldPos2D;

                tile.WorldPoints2D[0] = worldPos2D - halfSize2D;
                tile.WorldPoints2D[1] = worldPos2D + halfSize2D;
                tile.WorldPoints2D[2] = worldPos2D + new Vector2(-halfSize2D.x, halfSize2D.y);
                tile.WorldPoints2D[3] = worldPos2D + new Vector2(halfSize2D.x, -halfSize2D.y);

                Vector2 halfCollisionSize2D = Vector2.one * tileSize * tileCollisionSize / 2f;

                tile.WorldCollisionPoints2D[0] = worldPos2D - halfCollisionSize2D;
                tile.WorldCollisionPoints2D[1] = worldPos2D + halfCollisionSize2D;
                tile.WorldCollisionPoints2D[2] = worldPos2D + new Vector2(-halfCollisionSize2D.x, halfCollisionSize2D.y);
                tile.WorldCollisionPoints2D[3] = worldPos2D + new Vector2(halfCollisionSize2D.x, -halfCollisionSize2D.y);

                // 3D
                Vector3 worldPos = GridToWorld(pos);
                Vector3 halfSize = new(halfSize2D.x, 0, halfSize2D.y);

                tile.WorldPoint = worldPos;

                tile.WorldPoints[0] = worldPos - halfSize;
                tile.WorldPoints[1] = worldPos + halfSize;
                tile.WorldPoints[2] = worldPos + new Vector3(-halfSize.x, 0, halfSize.y);
                tile.WorldPoints[3] = worldPos + new Vector3(halfSize.x, 0, -halfSize.y);

                Vector3 halfCollisionSize = new(halfCollisionSize2D.x, 0, halfCollisionSize2D.y);

                tile.WorldCollisionPoints[0] = worldPos - halfCollisionSize;
                tile.WorldCollisionPoints[1] = worldPos + halfCollisionSize;
                tile.WorldCollisionPoints[2] = worldPos + new Vector3(-halfCollisionSize.x, 0, halfCollisionSize.y);
                tile.WorldCollisionPoints[3] = worldPos + new Vector3(halfCollisionSize.x, 0, -halfCollisionSize.y);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateGridOrigin();
    }
#endif

    private void UpdateGridOrigin()
    {
        _gridOrigin2D = new Vector2(-1, -1) * (gridSize / 2) * tileSize;
        _gridOrigin2D.x += gridOffset.x;
        _gridOrigin2D.y += gridOffset.y;

        _gridOrigin = new Vector3(_gridOrigin2D.x, 0, _gridOrigin2D.y);
    }

    public async Task<List<Vector3>> FindPath(Vector3 start, Vector3 end, CancellationToken cancellationToken)
    {
        await Awaitable.BackgroundThreadAsync();

        Dictionary<Tile, TileScores> scores = new();

        Tile startTile = GetTile(WorldToGrid(start));
        Tile endTile = GetTile(WorldToGrid(end));

        // Impossible path, TODO: Aproximate path if end is blocked
        if (startTile == null || endTile == null || endTile.Blocked)
        {
            return null;
        }

        List<Tile> open = new() { startTile };
        int count = 1;

        scores.Add(startTile, new(startTile, 0, Distance(startTile, endTile)));

        HashSet<Vector2Int> closed = new();

        Dictionary<Vector2Int, Tile> parentMap = new();

        while (count > 0)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            // Get the tile with the lowest F cost
            Tile current = null;
            int index = -1;
            TileScores currentScores = new TileScores(null, Mathf.Infinity, Mathf.Infinity);

            for (int i = 0; i < count; i++)
            {
                Tile tile = open[i];
                TileScores score = scores[tile];

                if (score.TotalCost < currentScores.TotalCost)
                {
                    current = tile;
                    currentScores = score;
                    index = i;
                }
            }

            if (current == endTile)
            {
                return await ConstructPath(parentMap, current, cancellationToken);
            }

            open.RemoveAt(index);
            count--;

            closed.Add(current.Pos);

            for (int i = 0; i < 4; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                Tile neighbor = current.Neighbors[i];

                if (neighbor == null || closed.Contains(neighbor.Pos) || neighbor.Blocked) continue;

                float tentativeG = currentScores.G + 1;

                if (scores.TryGetValue(neighbor, out TileScores score) && tentativeG >= score.G)
                {
                    continue;
                }

                score.Tile = neighbor;
                score.G = tentativeG;
                score.H = Distance(neighbor, endTile);

                scores[neighbor] = score;

                // Set the current node as the parent of the neighbor
                parentMap[neighbor.Pos] = current;

                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor);
                    count++;
                }
            }
        }

        // Couldn't find path :(
        return null;
    }

    private async Task<List<Vector3>> ConstructPath(Dictionary<Vector2Int, Tile> parentMap, Tile current, CancellationToken cancellationToken)
    {
        await Awaitable.BackgroundThreadAsync();

        var tiles = new List<Tile> { current };

        while (parentMap.ContainsKey(current.Pos))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            current = parentMap[current.Pos];

            tiles.Add(current);
        }

        tiles.Reverse();

        // Optimize path
        List<Vector3> path = new()
        {
            tiles[0].WorldPoint
        };

        //QueryParameters parameters = new(obstacleLayer);

        int count = tiles.Count;
        for (int i = 0; i < count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            Tile tile = tiles[i];

            // Shoot rays that will detect blocked tiles
            int e = 0;
            int lastIndex = 0;

            for (int j = i + 1; j < count; j++)
            {
                Tile nextTile = tiles[j];

                Vector2Int tilePos = tile.Pos;
                Vector2Int nextTilePos = nextTile.Pos;

                Vector2Int min = new(Mathf.Min(tilePos.x, nextTilePos.x), Mathf.Min(tilePos.y, nextTilePos.y));
                Vector2Int max = new(Mathf.Max(tilePos.x, nextTilePos.x), Mathf.Max(tilePos.y, nextTilePos.y));

                Vector2 from = tile.WorldPoint2D;
                Vector2 to = nextTile.WorldPoint2D;

                bool hit = false;
                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        Tile lineTile = GetTile(x, y);

                        if (!lineTile.Blocked)
                        {
                            continue;
                        }

                        if (PathfindingUtility.LineIntersectsBox(from, to, lineTile.WorldPoint2D, Vector2.one * tileSize * tileCollisionSize))
                        {
                            hit = true;
                            break;
                        }
                    }

                    if (hit)
                    {
                        break;
                    }
                }

                if (!hit)
                {
                    lastIndex = e;
                }

                e++;
            }

            i += lastIndex;

            path.Add(tiles[i].WorldPoint);
        }

        return path;
    }

    public class Tile
    {
        public Vector2Int Pos { get; private set; }
        public int X => Pos.x;
        public int Y => Pos.y;

        public Vector3 WorldPoint { get; set; }
        public Vector3[] WorldPoints { get; private set; } = new Vector3[4];
        public Vector3[] WorldCollisionPoints { get; private set; } = new Vector3[4];

        public Vector2 WorldPoint2D { get; set; }
        public Vector2[] WorldPoints2D { get; private set; } = new Vector2[4];
        public Vector2[] WorldCollisionPoints2D { get; private set; } = new Vector2[4];

        public float Cost { get; set; }

        public bool Blocked => BlockCount > 0;
        public int BlockCount { get; set; } = 0;

        public Tile[] Neighbors { get; private set; } = new Tile[4];

        public Tile(int x, int y)
        {
            Pos = new(x, y);
        }
    }

    public struct TileScores
    {
        public Tile Tile;

        public float G, H;
        public float TotalCost => G + H + (Tile == null ? 0 : Tile.Cost);

        public TileScores(Tile tile, float g, float h)
        {
            Tile = tile;
            G = g;
            H = h;
        }
    }

    #region WorldToGrid (Vector2)
    public Vector2Int WorldToGrid2D(Vector2 pos)
    {
        pos -= _gridOrigin2D;
        pos /= tileSize;

        return new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public Vector2Int WorldToGridFloor2D(Vector2 pos)
    {
        pos -= _gridOrigin2D;
        pos /= tileSize;

        return new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    }

    public Vector2Int WorldToGridCeil2D(Vector2 pos)
    {
        pos -= _gridOrigin2D;
        pos /= tileSize;

        return new(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y));
    }
    #endregion

    #region WorldToGrid (Vector3)
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        pos -= _gridOrigin;
        pos /= tileSize;

        return new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public Vector2Int WorldToGridFloor(Vector3 pos)
    {
        pos -= _gridOrigin;
        pos /= tileSize;

        return new(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
    }

    public Vector2Int WorldToGridCeil(Vector3 pos)
    {
        pos -= _gridOrigin;
        pos /= tileSize;

        return new(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.z));
    }
    #endregion

    public Vector3 GridToWorld2D(Vector2Int pos)
    {
        return GridToWorld2D(pos.x, pos.y);
    }

    public Vector2 GridToWorld2D(int x, int y)
    {
        return (new Vector2(x, y) * tileSize) + _gridOrigin2D;
    }

    public Vector3 GridToWorld(Vector2Int pos)
    {
        return GridToWorld(pos.x, pos.y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return (new Vector3(x, 0, y) * tileSize) + _gridOrigin;
    }

    public Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0) return null;
        if (x > gridSize || y > gridSize) return null;

        return _tiles[x, y];
    }

    public static float Distance(Tile t1, Tile t2)
    {
        return Distance(t1.Pos, t2.Pos);
    }
    public static float Distance(Vector2Int p1, Vector2Int p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector2Int camPos = WorldToGrid(Camera.current.transform.position);

        Vector3 size = new Vector3(tileSize, 0, tileSize);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (Distance(camPos, new Vector2Int(x, y)) <= 15)
                {
                    Vector3 pos = GridToWorld(x, y);

                    if (Application.isPlaying)
                    {
                        Tile tile = GetTile(x, y);
                        Gizmos.color = tile.Blocked ? Color.red : Color.green;

                        pos.y += tile.Blocked ? 0.05f : 0;
                    }

                    if (tileCollisionSize > 0 && tileCollisionSize < 1)
                    {
                        Color startCol = Gizmos.color;
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(pos, size * tileCollisionSize);

                        Gizmos.color = startCol;
                    }

                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }
#endif
}
