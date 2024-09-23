using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject wallPrefab;
    private bool isPlacing = false;

    // Update is called once per frame
    void Update()
{
    if (isPlacing && Input.GetMouseButtonDown(0)) // Left mouse click
    {
        // Convert the mouse position to world coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Round the world position to the nearest whole number to align with your grid
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

        // Check if it's valid to place a wall at this position
        if (IsValidPlacement(gridPosition))
        {
            Instantiate(wallPrefab, new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0), Quaternion.identity);
        }
    }
}

    public void TogglePlacingWall() {
        isPlacing = !isPlacing;
    }

    private bool IsValidPlacement(Vector2Int position) {
        return true;
    }
}
