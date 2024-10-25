using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed = 2f; // Base movement speed
    public float wanderRadius = 5f; // Radius for wandering behavior
    public float baseWaitTimeAtTarget = 1f; // Base wait time at each target
    public float collisionAvoidanceDistance = 0.5f; // Distance to adjust when colliding with walls
    public float raycastDistance = 1f; // Distance for raycasting to detect walls
    public float stuckThreshold = 2f; // Increased time before recalculating path if stuck
    public float jitterThreshold = 0.05f; // Slightly increase minimum movement to reset the stuck timer
    public float recalculateOnCollisionTime = 1f; // Increased time to wait before recalculating path after a collision
    public float tightSpaceAvoidanceDistance = 1f; // Increased distance for avoiding tight spaces like doorways

    private int targetIndex;
    private List<Node> path;
    private GridManager gridManager;
    private Pathfinding pathfinding;
    private bool isWandering = false;
    private float waitTimer;
    private float randomSpeed;
    private float randomWaitTime;

    private bool isPlayerControlled = false; // Flag to check if the NPC is currently controlled by player
    private BoxCollider2D npcCollider;

    // Variables for stuck detection
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    // Variables for collision handling
    private float collisionTimer = 0f; // Timer for delaying path recalculation after a collision
    private bool isCollidingWithWall = false; // Flag to check if the NPC is colliding with a wall

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();

        // Initialize with randomized speed and wait time
        randomSpeed = speed * Random.Range(0.75f, 1.25f); // Randomize speed between 75% to 125% of base speed
        randomWaitTime = baseWaitTimeAtTarget * Random.Range(0.75f, 1.25f); // Randomize wait time similarly

        npcCollider = GetComponent<BoxCollider2D>(); // Get the collider component
        waitTimer = randomWaitTime;
        StartWandering();

        lastPosition = transform.position; // Initialize last position
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            if (path != null)
            {
                MoveAlongPath();
            }
        }
        else
        {
            if (path != null)
            {
                MoveAlongPath();
            }
            else if (!isWandering)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    randomWaitTime = baseWaitTimeAtTarget * Random.Range(0.75f, 1.25f);
                    randomSpeed = speed * Random.Range(0.75f, 1.25f);

                    StartWandering();
                    waitTimer = randomWaitTime;
                }
            }
        }

        // Handle path recalculation on collision after a delay
        if (isCollidingWithWall)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer > recalculateOnCollisionTime)
            {
                //Debug.Log("NPC recalculating path after collision.");
                RecalculatePath();
                isCollidingWithWall = false; // Reset collision state
            }
        }
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        isPlayerControlled = true;
        path = pathfinding.FindPath(transform.position, targetPosition);
        targetIndex = 0;

        //Debug.Log($"NPC is moving to player-assigned target at {targetPosition}");
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

        // Perform raycasting to detect walls in front of the NPC
        if (IsWallAhead())
        {
            // Adjust NPC's movement direction to avoid the wall or tight space
            AvoidWallOrTightSpace();
        }

        // Move towards the target position (modified if avoiding walls)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition, // Move towards the target
            isPlayerControlled ? speed * Time.deltaTime : randomSpeed * Time.deltaTime
        );

        // Check if the NPC is close to the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            targetIndex++;
            if (targetIndex >= path.Count)
            {
                path = null;

                if (isPlayerControlled)
                {
                    isPlayerControlled = false;
                    //Debug.Log("NPC has reached the player-assigned target.");
                }
                else
                {
                    isWandering = false;
                    waitTimer = randomWaitTime;
                }
            }
        }

        // Recalculate path if the NPC is stuck for too long (in case wall avoidance fails)
        if (IsStuck())
        {
            //Debug.Log("NPC appears to be stuck. Recalculating path.");
            RecalculatePath();
        }
    }

    // Check if there is a wall or tight space in front of the NPC using raycasting
    bool IsWallAhead()
    {
        Vector2 direction = ((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, gridManager.unwalkableMask);

        return hit.collider != null;
    }

    // Avoid wall or tight space by adjusting movement direction slightly
    void AvoidWallOrTightSpace()
    {
        // If the NPC is in a tight space (e.g., doorway), perform a wider avoidance arc
        if (IsInTightSpace())
        {
            Vector2 avoidanceDirection = Vector2.Perpendicular(((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized);
            transform.position += (Vector3)avoidanceDirection * tightSpaceAvoidanceDistance;
        }
        else
        {
            // Regular wall avoidance
            Vector2 avoidanceDirection = Vector2.Perpendicular(((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized);
            transform.position += (Vector3)avoidanceDirection * collisionAvoidanceDistance;
        }
    }

    // Check if the NPC is in a tight space (like a doorway)
    bool IsInTightSpace()
    {
        // Perform additional raycasts to the left and right of the NPC to detect tight spaces
        Vector2 leftCheck = transform.position + Vector3.left * raycastDistance;
        Vector2 rightCheck = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftCheck, Vector2.left, raycastDistance, gridManager.unwalkableMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rightCheck, Vector2.right, raycastDistance, gridManager.unwalkableMask);

        // If there are obstacles on both sides, it's a tight space
        return leftHit.collider != null && rightHit.collider != null;
    }

    // Check if the NPC is stuck by comparing its position over time
    bool IsStuck()
    {
        // Check if the NPC has barely moved in the last second
        if (Vector3.Distance(transform.position, lastPosition) < jitterThreshold)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f; // Reset the timer if the NPC has moved
        }

        lastPosition = transform.position;

        // If the NPC hasn't moved for the stuckThreshold time, consider it stuck
        return stuckTimer > stuckThreshold;
    }

    // Recalculate the path if the NPC is stuck or colliding
    void RecalculatePath()
    {
        if (isPlayerControlled)
        {
            // Recalculate path to the player-assigned target if stuck
            path = pathfinding.FindPath(transform.position, path[targetIndex].worldPosition);
            targetIndex = 0;
        }
        else
        {
            // Restart wandering if stuck while wandering
            StartWandering();
        }
    }

    // Handle collision events
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("NPC collided with a wall, preparing to recalculate path.");
            isCollidingWithWall = true; // Mark NPC as colliding with a wall
            collisionTimer = 0f; // Reset the collision timer
        }

        if (collision.gameObject.CompareTag("NPC"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isCollidingWithWall = false; // Stop collision recalculation when leaving the wall
        }
    }
}
