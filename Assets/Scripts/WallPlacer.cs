using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject[] buildablePrefabs;
    public GameObject currentPrefab;
    private GameObject previewInstance;
    private bool isPlacing = false;

    void Update()
    {
        if (isPlacing && currentPrefab != null)
        {
            UpdatePreviewPosition();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
        }
    }

    public void StartPlacingObject(int prefabIndex)
    {
        if (isPlacing && currentPrefab == buildablePrefabs[prefabIndex])
        {
            StopPlacingObject();
            return;
        }

        if (prefabIndex >= 0 && prefabIndex < buildablePrefabs.Length)
        {
            currentPrefab = buildablePrefabs[prefabIndex];
            isPlacing = true;

            if (previewInstance != null)
            {
                Destroy(previewInstance);
            }

            previewInstance = Instantiate(currentPrefab);
            var spriteRenderer = previewInstance.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color semiTransparent = spriteRenderer.color;
                semiTransparent.a = 0.5f;
                spriteRenderer.color = semiTransparent;
            }
        }
    }

    public void StopPlacingObject()
    {
        isPlacing = false;
        currentPrefab = null;

        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }
    }

    private void UpdatePreviewPosition()
    {
        if (previewInstance == null)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(
            Mathf.FloorToInt(mousePosition.x),
            Mathf.FloorToInt(mousePosition.y)
        );

        previewInstance.transform.position = new Vector3(
            gridPosition.x + 0.5f,
            gridPosition.y + 0.5f,
            0
        );
    }

    private void PlaceObject()
    {
        Vector2Int gridPosition = new Vector2Int(
            Mathf.FloorToInt(previewInstance.transform.position.x),
            Mathf.FloorToInt(previewInstance.transform.position.y)
        );

        if (IsValidPlacement(gridPosition))
        {
            Instantiate(currentPrefab, previewInstance.transform.position, Quaternion.identity);
        }
    }

    private bool IsValidPlacement(Vector2Int position)
    {
        return true;
    }
}
