using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    GridManager grid;

    void Awake()
    {
        grid = FindFirstObjectByType<GridManager>();
    }

    public List<Vector2Int> FindPath(
        Vector2Int start,
        Vector2Int target
    )
    {
        Node startNode = grid.grid[start.x, start.y];
        Node targetNode = grid.grid[target.x, target.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                   (openSet[i].fCost == current.fCost &&
                    openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable ||
                    closedSet.Contains(neighbor))
                    continue;

                int newCost = current.gCost + 10;
                if (newCost < neighbor.gCost ||
                    !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost =
                        GetDistance(neighbor, targetNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector2Int p = node.gridPos;
        Vector2Int[] dirs = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var d in dirs)
        {
            Vector2Int np = p + d;
            if (grid.IsWalkable(np))
                neighbors.Add(grid.grid[np.x, np.y]);
        }

        return neighbors;
    }

    List<Vector2Int> RetracePath(Node start, Node end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = end;

        while (current != start)
        {
            path.Add(current.gridPos);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node a, Node b)
    {
        return Mathf.Abs(a.gridPos.x - b.gridPos.x) +
               Mathf.Abs(a.gridPos.y - b.gridPos.y);
    }
}

