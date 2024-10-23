using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBounds : MonoBehaviour
{
    public Tilemap tilemap;  // Reference to the tilemap

    void Start()
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;
        
        // Calculate the size of the map
        Vector3Int mapSize = bounds.size;
        Debug.Log("Tilemap Size: " + mapSize.x + " x " + mapSize.y);
    }
}
