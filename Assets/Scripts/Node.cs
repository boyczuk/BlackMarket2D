using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector2 worldPosition;  // Ensure this is a Vector2
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
    {
        this.isWalkable = isWalkable;
        this.worldPosition = worldPosition;  // Vector2 type
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
