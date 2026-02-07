using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;

    [Header("Building to Place")]
    // Drag your Building Prefab here for now. 
    // Later, you can change this variable via UI buttons to swap buildings.
    public GameObject currentBuildingPrefab;

    private void Update()
    {
        // 0 is Left Click
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (currentBuildingPrefab == null || gridManager == null) return;

        // 1. Create a Ray from the camera through the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 2. Cast the ray into the world
        // We use 'Mathf.Infinity' to check as far as needed
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 3. Convert the hit point (World Position) to Grid Coordinates
            Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);

            // 4. Check if we clicked on a valid tile (optional but good for safety)
            TileData tile = gridManager.GetTileAt(gridPos);

            if (tile != null)
            {
                PlaceBuilding(gridPos);
            }
        }
    }

    private void PlaceBuilding(Vector2Int gridPos)
    {
        // Calculate the world position for the building based on the grid coordinate
        // This ensures it snaps exactly to the center/corner of the grid cell
        Vector3 spawnPos = new Vector3(
            gridPos.x * gridManager.tileSize,
            0,
            gridPos.y * gridManager.tileSize
        );

        Instantiate(currentBuildingPrefab, spawnPos, Quaternion.identity);
    }
}