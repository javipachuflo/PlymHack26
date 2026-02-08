using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;

    [Header("Building to Place")]
    public GameObject currentBuildingPrefab;

    private void Update()
    {
        // Left Click to Place
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(false);
        }

        // Right Click to Remove
        if (Input.GetMouseButtonDown(1))
        {
            HandleInput(true);
        }
    }

    private void HandleInput(bool isRemoving)
    {
        if (gridManager == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // REMOVAL LOGIC
            if (isRemoving)
            {
                // Check if we hit a building directly
                BuildingAffector affector = hit.collider.GetComponent<BuildingAffector>();
                if (affector != null)
                {
                    Destroy(affector.gameObject);
                }
                // Optional: Check if we hit a tile, then find the building on it (requires tracking)
                // For now, clicking the building directly is easiest.
            }
            // PLACEMENT LOGIC
            else if (currentBuildingPrefab != null)
            {
                Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
                TileData tile = gridManager.GetTileAt(gridPos);

                if (tile != null)
                {
                    PlaceBuilding(gridPos);
                }
            }
        }
    }

    private void PlaceBuilding(Vector2Int gridPos)
    {
        // Check if there is already a building here? 
        // For a hackathon, we can skip complex overlap checks if you want speed.

        Vector3 spawnPos = new Vector3(
            gridPos.x * gridManager.tileSize,
            0,
            gridPos.y * gridManager.tileSize
        );

        Instantiate(currentBuildingPrefab, spawnPos, Quaternion.identity);
    }
}