using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private GridManager grid;

    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // Adjust this condition to prefer nodes that make wider turns
                if (openSet[i].FCost < currentNode.FCost || 
                   (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // Apply an extra cost if turning sharply to avoid sharp turns
                newCostToNeighbor += GetTurnPenalty(currentNode, neighbor);

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

        return null;
    }

    // New method to penalize sharp turns
    int GetTurnPenalty(Node currentNode, Node nextNode)
    {
        // Define the current direction of movement
        Vector2 currentDirection = new Vector2(currentNode.gridX - (currentNode.parent?.gridX ?? currentNode.gridX),
                                               currentNode.gridY - (currentNode.parent?.gridY ?? currentNode.gridY));

        // Define the direction to the next node
        Vector2 nextDirection = new Vector2(nextNode.gridX - currentNode.gridX,
                                            nextNode.gridY - currentNode.gridY);

        // If the direction has changed (indicating a turn), add a penalty
        if (currentDirection != nextDirection)
        {
            // Increase penalty for sharp turns (e.g., 90-degree turns)
            return 10; // Adjust this value to control how "wide" the turns should be
        }

        // No turn, so no penalty
        return 0;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
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
