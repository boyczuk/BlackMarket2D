using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed = 2f;
    public float wanderRadius = 5f;
    public float baseWaitTimeAtTarget = 1f;
    public float stuckThreshold = 0.1f;
    public float recalculateOnCollisionTime = 0.5f;

    public bool isInPlayerGang = false; // Indicates if NPC is part of the player's gang
    public bool isPlayerControlled = false; // Tracks if the NPC is currently following a player command

    private List<Node> path;
    private GridManager gridManager;
    private Pathfinding pathfinding;
    private bool isWandering = false;
    private bool reachedWanderTarget = true; 
    private bool pathInProgress = false; 
    private float waitTimer;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();
        StartWandering();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (path != null && path.Count > 0)
        {
            MoveAlongPath();
        }
        else if (!isPlayerControlled && !pathInProgress && reachedWanderTarget)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                StartWandering();
                waitTimer = baseWaitTimeAtTarget;
            }
        }
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        StopAllCoroutines();
        isPlayerControlled = true;
        isWandering = false;
        reachedWanderTarget = false;
        pathInProgress = true; 
        path = pathfinding.FindPath(transform.position, targetPosition);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No valid path found to target position.");
            return;
        }
        StartCoroutine(ResumeWanderingAfterDelay());
    }

    private IEnumerator ResumeWanderingAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        isPlayerControlled = false;
        reachedWanderTarget = true;
        StartWandering();
    }

    void StartWandering()
    {
        if (pathInProgress) return; // Prevents new wandering path from interrupting
        
        Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
        Vector2 wanderTarget = (Vector2)transform.position + randomDirection;

        Node wanderNode = gridManager.NodeFromWorldPoint(wanderTarget);
        if (wanderNode != null && wanderNode.isWalkable)
        {
            path = pathfinding.FindPath(transform.position, wanderTarget);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("No valid path found for wandering.");
                return;
            }
            isWandering = true;
            reachedWanderTarget = false;
            pathInProgress = true;
        }
    }

    void MoveAlongPath()
    {
        if (path == null || path.Count == 0) return;

        Vector3 targetPosition = path[0].worldPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            path.RemoveAt(0);
            if (path.Count == 0)
            {
                path = null;
                pathInProgress = false; 
                if (!isPlayerControlled)
                {
                    isWandering = true;
                    reachedWanderTarget = true;
                }
            }
        }

        if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > recalculateOnCollisionTime)
            {
                RecalculatePath();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void RecalculatePath()
    {
        if (isPlayerControlled)
        {
            path = pathfinding.FindPath(transform.position, path != null && path.Count > 0 ? path[path.Count - 1].worldPosition : transform.position);
        }
        else
        {
            StartWandering();
        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
            }
        }
    }
}
