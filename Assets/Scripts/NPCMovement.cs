using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed = 2f; // Base movement speed
    public float wanderRadius = 5f; // Radius for wandering behavior
    public float baseWaitTimeAtTarget = 1f; // Base wait time at each target
    public float collisionAvoidanceDistance = 0.5f; // Distance to adjust when colliding with walls

    private int targetIndex;
    private List<Node> path;
    private GridManager gridManager;
    private Pathfinding pathfinding;
    private bool isWandering = false;
    private float waitTimer;
    private float randomSpeed;
    private float randomWaitTime;

    private bool isPlayerControlled = false; // Flag to check if the NPC is currently controlled by player

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();

        // Initialize with randomized speed and wait time
        randomSpeed = speed * Random.Range(0.75f, 1.25f); // Randomize speed between 75% to 125% of base speed
        randomWaitTime = baseWaitTimeAtTarget * Random.Range(0.75f, 1.25f); // Randomize wait time similarly

        waitTimer = randomWaitTime;
        StartWandering();
    }

    void Update()
    {
        // Check if the NPC is in "player-controlled" mode (i.e., moving to a player-assigned destination)
        if (isPlayerControlled)
        {
            // Move along the assigned path if set by the player
            if (path != null)
            {
                MoveAlongPath();
            }
        }
        else
        {
            // Continue normal wandering if not under player control
            if (path != null)
            {
                MoveAlongPath();
            }
            else if (!isWandering)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    // Restart wandering with randomized values
                    randomWaitTime = baseWaitTimeAtTarget * Random.Range(0.75f, 1.25f);
                    randomSpeed = speed * Random.Range(0.75f, 1.25f);

                    StartWandering();
                    waitTimer = randomWaitTime; // Reset the wait timer with the new randomized wait time
                }
            }
        }
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        isPlayerControlled = true;
        path = pathfinding.FindPath(transform.position, targetPosition);
        targetIndex = 0;

        Debug.Log($"NPC is moving to player-assigned target at {targetPosition}");
    }

    void StartWandering()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
        Vector2 wanderTarget = (Vector2)transform.position + randomDirection;

        Node wanderNode = gridManager.NodeFromWorldPoint(wanderTarget);
        if (wanderNode != null && wanderNode.isWalkable)
        {
            path = pathfinding.FindPath(transform.position, wanderTarget);
            targetIndex = 0;
            isWandering = true;
        }
    }

    void MoveAlongPath()
    {
        if (targetIndex >= path.Count)
            return;

        Vector3 targetPosition = path[targetIndex].worldPosition;

        AvoidWalls();

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            isPlayerControlled ? speed * Time.deltaTime : randomSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            targetIndex++;
            if (targetIndex >= path.Count)
            {
                path = null;

                if (isPlayerControlled)
                {
                    isPlayerControlled = false;
                    Debug.Log("NPC has reached the player-assigned target.");
                }
                else
                {
                    isWandering = false;
                    waitTimer = randomWaitTime;
                }
            }
        }
    }

    void AvoidWalls()
    {
        float detectionDistance = gridManager.nodeRadius + 0.5f; 

        RaycastHit2D hit = Physics2D.Raycast(
            (Vector2)transform.position,
            (Vector2)path[targetIndex].worldPosition - (Vector2)transform.position,
            detectionDistance,
            gridManager.unwalkableMask
        );

        if (hit.collider != null)
        {
            Vector2 avoidanceDirection = Vector2.Perpendicular(hit.normal).normalized;

            transform.position += (Vector3)(avoidanceDirection * collisionAvoidanceDistance);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
