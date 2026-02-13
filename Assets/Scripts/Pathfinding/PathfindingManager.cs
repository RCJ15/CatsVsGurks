using System;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float tileCollisionSize = 1f;
    [SerializeField] private int gridSize = 100;
    [SerializeField] private Vector2 gridOffset;

    private Tile[,] _tiles;
    private Vector3 _gridOrigin;

    private List<Tile> _path;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

                /*
                if (xOver && yOver)
                {
                    Tile neighbor = GetTile(pos + Directions[DOWN_LEFT]);
                    tile.Neighbors[DOWN_LEFT] = neighbor;
                    neighbor.Neighbors[UP_RIGHT] = tile;

                    leftNeighbor.Neighbors[DOWN_RIGHT] = downNeighbor;
                    downNeighbor.Neighbors[UP_LEFT] = leftNeighbor;
                }
                */
            }
        }

        GetTile(0, 0).BlockCount++;
        GetTile(1, 0).BlockCount++;
        GetTile(2, 0).BlockCount++;
        GetTile(3, 0).BlockCount++;

        GetTile(3, 3).BlockCount++;
        GetTile(3, 4).BlockCount++;
        GetTile(3, 5).BlockCount++;
        GetTile(3, 6).BlockCount++;

        GetTile(4, 3).BlockCount++;

        GetTile(4, 4).BlockCount++;
        GetTile(5, 4).BlockCount++;
        GetTile(6, 4).BlockCount++;
        GetTile(7, 4).BlockCount++;
        GetTile(8, 4).BlockCount++;
        GetTile(9, 4).BlockCount++;

        _path = FindPath(GetTile(9, 9), GetTile(9, 0));

        for (int i = 1; i < _path.Count; i++)
        {

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
        _gridOrigin = new Vector3(-1, 0, -1) * (gridSize / 2) * tileSize;
        _gridOrigin.x += gridOffset.x;
        _gridOrigin.z += gridOffset.y;
    }

    private void Update()
    {
        
    }

    public List<Tile> FindPath(Tile start, Tile end)
    {
        Dictionary<Tile, TileScores> scores = new();

        List<Tile> open = new() { start };
        int count = 1;

        scores.Add(start, new(start, 0, Distance(start, end)));

        HashSet<Tile> closed = new();

        Dictionary<Vector2Int, Tile> parentMap = new();

        while (count > 0)
        {
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

            if (current == end)
            {
                return ConstructPath(parentMap, current);
            }

            open.RemoveAt(index);
            count--;

            closed.Add(current);

            foreach (Tile neighbor in current.Neighbors)
            {
                if (neighbor == null || closed.Contains(neighbor) || neighbor.Blocked) continue;

                float tentativeG = currentScores.G + 1;

                if (scores.TryGetValue(neighbor, out TileScores score) && tentativeG >= score.G)
                {
                    continue;
                }

                score.Tile = neighbor;
                score.G = tentativeG;
                score.H = Distance(neighbor, end);

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

    private List<Tile> ConstructPath(Dictionary<Vector2Int, Tile> parentMap, Tile current)
    {
        var path = new List<Tile> { current };

        while (parentMap.ContainsKey(current.Pos))
        {
            Vector3 pos1 = GridToWorld(current.Pos);

            current = parentMap[current.Pos];

            Vector3 pos2 = GridToWorld(current.Pos);
            Debug.DrawLine(pos1, pos2, Color.black, 999);

            path.Add(current);
        }

        path.Reverse();

        

        return path;
    }

    public class Tile
    {
        public Vector2Int Pos { get; private set; }
        public int X => Pos.x;
        public int Y => Pos.y;

        public float Cost { get; set; }

        public bool Blocked => BlockCount > 0;
        public int BlockCount { get; set; } = 0;

        public Tile[] Neighbors { get; private set; } = new Tile[8];

        public Tile(int x, int y)
        {
            Pos = new(x, y);
        }

        public void PrintNeighbors()
        {
            Debug.Log($"{ToString()} | NEIGHBOR R:{Neighbors[RIGHT]}, D:{Neighbors[DOWN]}, L:{Neighbors[LEFT]}, U:{Neighbors[UP]}");
        }

        public override string ToString()
        {
            return $"TILE(X:{X}, Y:{Y})";
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

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        pos -= _gridOrigin;
        pos /= tileSize;

        return new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
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
    public Tile GetTile(int x, int y) => _tiles[x, y];

    public static float Distance(Tile t1, Tile t2)
    {
        return Distance(t1.Pos, t2.Pos);
    }
    public static float Distance(Vector2Int p1, Vector2Int p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
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

                    Gizmos.DrawWireCube(pos, size);
                }
            }
        }
    }
#endif
}
