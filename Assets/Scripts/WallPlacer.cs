using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject wallPreviewPrefab;

    private GameObject previewInstance;
    private bool isPlacing = false;

    // Update is called once per frame
    void Update()
    {
        if (isPlacing)
        {
            UpdatePreviewPosition();

            // Place the wall on left mouse click
            if (Input.GetMouseButtonDown(0))
            {
                PlaceWall();
            }
        }
    }

    public void TogglePlacingWall()
    {
        isPlacing = !isPlacing;

        if (isPlacing)
        {
            // Create the preview instance when starting placement
            previewInstance = Instantiate(wallPreviewPrefab);
        }
        else
        {
            // Destroy the preview instance when stopping placement
            if (previewInstance != null)
            {
                Destroy(previewInstance);
            }
        }

        Debug.Log("Wall placement " + (isPlacing ? "started" : "stopped"));
    }

    private void UpdatePreviewPosition()
    {
        if (previewInstance == null) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));

        // Update the position of the preview wall
        previewInstance.transform.position = new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0);
    }

    private void PlaceWall()
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(previewInstance.transform.position.x), Mathf.FloorToInt(previewInstance.transform.position.y));

        if (IsValidPlacement(gridPosition))
        {
            Instantiate(wallPrefab, previewInstance.transform.position, Quaternion.identity);
        }
    }

    private bool IsValidPlacement(Vector2Int position)
    {
        // Add logic to check if a wall can be placed here (e.g., no other wall or object present)
        return true; // Adjust this as needed
    }
}

