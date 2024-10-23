using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker,
        target;
    private GridManager grid;

    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        int maxIterations = 10000; // Limit iterations to prevent infinite loop
        int iterations = 0;

        while (openSet.Count > 0)
        {
            iterations++;
            if (iterations > maxIterations)
            {
                Debug.LogWarning("Pathfinding loop exceeded maximum iterations.");
                return;
            }

            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (
                    openSet[i].FCost < currentNode.FCost
                    || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost
                )
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // Path should go from start to end

        // New: Smooth out the path by removing redundant nodes
        path = SmoothPath(path);

        grid.path = path;

        // Send the path to the NPC movement
        NPCMovement npcMovement = seeker.GetComponent<NPCMovement>();
        if (npcMovement != null)
        {
            npcMovement.SetPath(path);
        }
    }

    List<Node> SmoothPath(List<Node> path)
    {
        if (path == null || path.Count < 2)
            return path;

        List<Node> smoothedPath = new List<Node>();

        smoothedPath.Add(path[0]); // Add start node
        Node previousDirectionNode = path[0];

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2 currentDirection = (
                path[i + 1].worldPosition - previousDirectionNode.worldPosition
            ).normalized;
            Vector2 nextDirection = (
                path[i].worldPosition - previousDirectionNode.worldPosition
            ).normalized;

            // If the direction changes, add the current node to the smoothed path
            if (currentDirection != nextDirection)
            {
                smoothedPath.Add(path[i]);
                previousDirectionNode = path[i];
            }
        }

        smoothedPath.Add(path[path.Count - 1]); // Add end node
        return smoothedPath;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void OnDrawGizmos()
    {
        if (grid != null && grid.path != null)
        {
            foreach (Node node in grid.path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (grid.nodeRadius * 2));
            }
        }
    }
}
