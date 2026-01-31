using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCErraticMovement : NPCMovement
{
    [Header("Erratic Settings")]
    [SerializeField] float wanderRadius = 3f;
    [SerializeField] float minWait = 0.5f;
    [SerializeField] float maxWait = 2f;

    List<Vector3> path;
    int pathIndex;
    Coroutine routine;

        void OnEnable()
    {
        base.OnEnable();    
        routine = StartCoroutine(ErraticRoutine());
    }

    void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);

        path = null;
    }

    Vector3 GetRandomNearbyPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = transform.position + (Vector3)offset;

            Vector2Int gridPos = grid.WorldToGrid(candidate);
            if (grid.IsWalkable(gridPos))
                return candidate;
        }

        return transform.position;
    }

    IEnumerator ErraticRoutine()
    {
        while (true)
        {
            Vector3 target = GetRandomNearbyPoint();
            BuildPathTo(target);

            yield return new WaitForSeconds(
                Random.Range(minWait, maxWait)
            );
        }
    }

    void BuildPathTo(Vector3 worldTarget)
    {
        Vector2Int start = grid.WorldToGrid(transform.position);
        Vector2Int end   = grid.WorldToGrid(worldTarget);

        if (!grid.IsWalkable(end))
            return;

        List<Vector2Int> gridPath =
            pathfinder.FindPath(start, end);

        if (gridPath == null || gridPath.Count == 0)
            return;

        path = new List<Vector3>();
        foreach (var p in gridPath)
            path.Add(grid.GridToWorld(p));

        pathIndex = 0;
    }

    void Update()
    {
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
