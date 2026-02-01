using UnityEngine;
using System.Collections.Generic;

public class ChasePlayerMovement : NPCMovement
{
    [Header("Chase Settings")]
    [SerializeField] private float pathUpdateInterval = 0.5f;
    [SerializeField] private float chaseDuration = 3f;

    private Transform targetPlayer;
    private List<Vector3> path;
    private int pathIndex;
    private float pathUpdateTimer;
    private float chaseTimer;
    private Guard guard;

    void Awake()
    {
        base.Awake();
        guard = GetComponentInParent<Guard>();
    }

    public void SetTarget(Transform playerToChase)
    {
        targetPlayer = playerToChase;
        pathUpdateTimer = 0f;
        chaseTimer = 0f;
    }

    private void ChasePlayer()
    {
        pathUpdateTimer -= Time.deltaTime;
        chaseTimer += Time.deltaTime;
        
        if (chaseTimer >= chaseDuration)
        {
            guard?.ChangeToErratic();
            return;
        }
        
        if (pathUpdateTimer <= 0f)
        {
            BuildPathToPlayer();
            pathUpdateTimer = pathUpdateInterval;
        }
    }

    private void BuildPathToPlayer()
    {
        if (targetPlayer == null || grid == null || pathfinder == null)
            return;

        Vector2Int start = grid.WorldToGrid(transform.position);
        Vector2Int end = grid.WorldToGrid(targetPlayer.position);

        if (!grid.IsWalkable(end))
            return;

        List<Vector2Int> gridPath = pathfinder.FindPath(start, end);

        if (gridPath == null || gridPath.Count == 0)
            return;

        path = new List<Vector3>();
        foreach (var p in gridPath)
            path.Add(grid.GridToWorld(p));

        pathIndex = 0;
    }

    void Update()
    {
        if (targetPlayer == null) return;

        ChasePlayer();
        FollowPath();
    }

    void FollowPath()
    {
        if (path == null || pathIndex >= path.Count)
            return;

        Vector3 target = path[pathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
            pathIndex++;
    }
}
