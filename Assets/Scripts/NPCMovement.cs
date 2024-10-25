using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float speed = 2f; 
    public float wanderRadius = 5f; 
    public float baseWaitTimeAtTarget = 1f; 
    public float collisionAvoidanceDistance = 0.5f;
    public float raycastDistance = 1f;
    public float stuckThreshold = 2f;
    public float jitterThreshold = 0.05f; 
    public float recalculateOnCollisionTime = 1f; 
    public float tightSpaceAvoidanceDistance = 1f; 

    private int targetIndex;
    private List<Node> path;
    private GridManager gridManager;
    private Pathfinding pathfinding;
    private bool isWandering = false;
    private float waitTimer;
    private float randomSpeed;
    private float randomWaitTime;

    private bool isPlayerControlled = false; 
    private BoxCollider2D npcCollider;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    private float collisionTimer = 0f;
    private bool isCollidingWithWall = false; 

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinding = FindObjectOfType<Pathfinding>();

        randomSpeed = speed * Random.Range(0.75f, 1.25f);
        randomWaitTime = baseWaitTimeAtTarget * Random.Range(0.75f, 1.25f);

        npcCollider = GetComponent<BoxCollider2D>(); 
        waitTimer = randomWaitTime;
        StartWandering();

        lastPosition = transform.position;
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

        if (isCollidingWithWall)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer > recalculateOnCollisionTime)
            {
                RecalculatePath();
                isCollidingWithWall = false;
            }
        }
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        isPlayerControlled = true;
        path = pathfinding.FindPath(transform.position, targetPosition);
        targetIndex = 0;
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

        if (IsWallAhead())
        {
            AvoidWallOrTightSpace();
        }

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
                }
                else
                {
                    isWandering = false;
                    waitTimer = randomWaitTime;
                }
            }
        }

        if (IsStuck())
        {
            RecalculatePath();
        }
    }

    bool IsWallAhead()
    {
        Vector2 direction = ((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, gridManager.unwalkableMask);

        return hit.collider != null;
    }

    void AvoidWallOrTightSpace()
    {
        if (IsInTightSpace())
        {
            Vector2 avoidanceDirection = Vector2.Perpendicular(((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized);
            transform.position += (Vector3)avoidanceDirection * tightSpaceAvoidanceDistance;
        }
        else
        {
            Vector2 avoidanceDirection = Vector2.Perpendicular(((Vector2)path[targetIndex].worldPosition - (Vector2)transform.position).normalized);
            transform.position += (Vector3)avoidanceDirection * collisionAvoidanceDistance;
        }
    }

    bool IsInTightSpace()
    {
        Vector2 leftCheck = transform.position + Vector3.left * raycastDistance;
        Vector2 rightCheck = transform.position + Vector3.right * raycastDistance;

        RaycastHit2D leftHit = Physics2D.Raycast(leftCheck, Vector2.left, raycastDistance, gridManager.unwalkableMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rightCheck, Vector2.right, raycastDistance, gridManager.unwalkableMask);

        return leftHit.collider != null && rightHit.collider != null;
    }

    bool IsStuck()
    {
        if (Vector3.Distance(transform.position, lastPosition) < jitterThreshold)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        return stuckTimer > stuckThreshold;
    }

    void RecalculatePath()
    {
        if (isPlayerControlled)
        {
            path = pathfinding.FindPath(transform.position, path[targetIndex].worldPosition);
            targetIndex = 0;
        }
        else
        {
            StartWandering();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isCollidingWithWall = true;
            collisionTimer = 0f;
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
            isCollidingWithWall = false;
        }
    }
}
