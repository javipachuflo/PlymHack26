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
            else if (currentBuildingPrefab != null)
            {
                // 1. Get the cost from the prefab we are trying to place
                BuildingAffector affector = currentBuildingPrefab.GetComponent<BuildingAffector>();
                int buildingCost = affector != null ? affector.cost : 0;

                // 2. Check if we have enough money
                if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(buildingCost))
                {
                    Debug.Log("Not enough money!");
                    return; // Stop here, don't place it
                }

                Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
                TileData tile = gridManager.GetTileAt(gridPos);

                if (tile != null)
                {
                    PlaceBuilding(gridPos, buildingCost); // Pass the cost to the next function
                }
            }
        }
        // PLACEMENT LOGIC
        else if (currentBuildingPrefab != null)
        {
            // 1. Get the cost from the prefab we are trying to place
            BuildingAffector affector = currentBuildingPrefab.GetComponent<BuildingAffector>();
            int buildingCost = affector != null ? affector.cost : 0;

            // 2. Check if we have enough money
            if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(buildingCost))
            {
                Debug.Log("Not enough money!");
                return; // Stop here, don't place it
            }

            Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
            TileData tile = gridManager.GetTileAt(gridPos);

            if (tile != null)
            {
                PlaceBuilding(gridPos, buildingCost); // Pass the cost to the next function
            }
        }
    }

    private void PlaceBuilding(Vector2Int gridPos, int cost)
    {
        // Check if there is already a building here? 
        // For a hackathon, we can skip complex overlap checks if you want speed.

        Vector3 spawnPos = new Vector3(
            gridPos.x * gridManager.tileSize,
            0,
            gridPos.y * gridManager.tileSize
        );

        Instantiate(currentBuildingPrefab, spawnPos, Quaternion.identity);

        // 3. Deduct the money
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.SpendMoney(cost);
        }
    }
}