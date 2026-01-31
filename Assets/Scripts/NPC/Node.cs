using UnityEngine;

public class Node
{
    public Vector2Int gridPos;
    public bool walkable;

    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;
}

