using UnityEngine;

public class BuildingAffector : MonoBehaviour
{
    [Header("Settings")]
    public int radius = 3; // How far the effect reaches (in tiles)
    public int scoreValue = 100; // The score this building provides (e.g., Safety)

    private GridManager gridManager;
    private Vector2Int gridPosition;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        
        // Snap to grid coordinates
        gridPosition = gridManager.WorldToGridCoordinates(transform.position);
        
        // Apply influence immediately upon placement
        UpdateInfluence(true);
    }

    private void OnDestroy()
    {
        // Remove influence when deleted
        UpdateInfluence(false);
    }

    private void UpdateInfluence(bool isAdding)
    {
        if (gridManager == null) return;

        // Loop through a square area around the building
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int targetPos = new Vector2Int(gridPosition.x + x, gridPosition.y + y);

                // Ask the GridManager for the tile at these coordinates
                TileData tile = gridManager.GetTileAt(targetPos);

                if (tile != null)
                {
                    if (isAdding)
                    {
                        // Pass our own GridPosition as the ID so the tile knows who sent it
                        tile.AddInfluence(gridPosition, scoreValue);
                    }
                    else
                    {
                        tile.RemoveInfluence(gridPosition);
                    }
                }
            }
        }
    }
}