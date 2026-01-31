#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Gizmos")]
    public bool drawGridGizmos = true;
    public bool drawBlocked = true;
    public bool drawWalkable = true;

    public Color walkableColor = new Color(0, 1, 0, 0.25f);
    public Color blockedColor  = new Color(1, 0, 0, 0.25f);
    public Tilemap ground;
    public Tilemap obstacles;

    public int width;
    public int height;
    public Vector2Int origin;

    public Node[,] grid;

    void Awake()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector3Int cell = new Vector3Int(
                origin.x + x,
                origin.y + y,
                0
            );

            bool walkable =
                ground.HasTile(cell) &&
                !obstacles.HasTile(cell);

            grid[x, y] = new Node {
                gridPos = new Vector2Int(x, y),
                walkable = walkable
            };
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Calculate Grid Bounds from Tilemap")]
    void CalculateGridBounds()
    {
        if (ground == null)
        {
            Debug.LogError("Ground Tilemap is not assigned!");
            return;
        }

        ground.CompressBounds();
        BoundsInt bounds = ground.cellBounds;

        origin = new Vector2Int(bounds.xMin, bounds.yMin);
        width = bounds.size.x;
        height = bounds.size.y;

        Debug.Log($"Grid Bounds Calculated:");
        Debug.Log($"  Origin: {origin}");
        Debug.Log($"  Width: {width}");
        Debug.Log($"  Height: {height}");
        Debug.Log($"  Bounds: from ({bounds.xMin}, {bounds.yMin}) to ({bounds.xMax}, {bounds.yMax})");
    }
#endif

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = ground.WorldToCell(worldPos);

        return new Vector2Int(
            cell.x - origin.x,
            cell.y - origin.y
        );
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3Int cell = new Vector3Int(
            origin.x + gridPos.x,
            origin.y + gridPos.y,
            0
        );

        return ground.GetCellCenterWorld(cell);
    }

    public bool IsWalkable(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 ||
            pos.x >= width || pos.y >= height)
            return false;

        return grid[pos.x, pos.y].walkable;
    }

    void OnDrawGizmos()
    {
        if (!drawGridGizmos || grid == null)
            return;

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector2Int gp = new Vector2Int(x, y);
            Vector3 world = GridToWorld(gp);

            Gizmos.color = grid[x, y].walkable
                ? walkableColor
                : blockedColor;

            Gizmos.DrawCube(
                world,
                Vector3.one * 0.9f
            );
        }
    }
}