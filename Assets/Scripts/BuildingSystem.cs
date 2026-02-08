using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;

    [Header("Building Palette")]
    public GameObject[] buildingPrefabs;

    // -1 means nothing is selected (Inspection Mode)
    private int selectedIndex = -1;

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSelectionUI(-1);
        }
    }

    private void Update()
    {
        // KEYBOARD INPUT (Toggle Logic)
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleSelection(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleSelection(1);
        else if (Input.GetKeyDown(KeyCode.Alpha0)) ToggleSelection(2);

        // MOUSE INPUT
        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(false);
        }

        // Right Click (Removal)
        if (Input.GetMouseButtonDown(1))
        {
            HandleInput(true);
        }

        // ... inside Update() ...

        // ESCAPE KEY (Cancel / Close)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedIndex = -1; // Deselect everything

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateSelectionUI(-1); // Turn off green rings
                UIManager.Instance.HideInspector();       // Close the panel
            }
        }
    }

    private void ToggleSelection(int index)
    {
        if (selectedIndex == index) selectedIndex = -1; // Toggle Off
        else selectedIndex = index; // Toggle On

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSelectionUI(selectedIndex);
            // Always hide the inspector when we switch tools
            UIManager.Instance.HideInspector();
        }
    }

    private void HandleInput(bool isRemoving)
    {
        if (gridManager == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // --- REMOVAL LOGIC (Right Click) ---
            if (isRemoving)
            {
                // 1. Clear the Tile
                Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
                TileData tile = gridManager.GetTileAt(gridPos);
                if (tile != null) tile.occupiedObject = null;

                // 2. Destroy Object & Refund
                BuildingAffector affector = hit.collider.GetComponent<BuildingAffector>();
                if (affector != null)
                {
                    if (EconomyManager.Instance != null) EconomyManager.Instance.AddMoney(affector.cost);
                    Destroy(affector.gameObject);
                }

                BuildingAffectable affectable = hit.collider.GetComponent<BuildingAffectable>();
                if (affectable != null)
                {
                    Destroy(affectable.gameObject);
                }

                // Hide inspector if we destroy something
                if (UIManager.Instance != null) UIManager.Instance.HideInspector();
            }
            // --- LEFT CLICK LOGIC ---
            else
            {
                // A. BUILDING MODE (Something selected)
                if (selectedIndex >= 0)
                {
                    if (buildingPrefabs.Length > selectedIndex)
                    {
                        GameObject prefabToPlace = buildingPrefabs[selectedIndex];

                        BuildingAffector affector = prefabToPlace.GetComponent<BuildingAffector>();
                        int buildingCost = affector != null ? affector.cost : 0;

                        if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(buildingCost))
                        {
                            Debug.Log("Not enough money!");
                            return;
                        }

                        Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
                        TileData tile = gridManager.GetTileAt(gridPos);

                        // Check if tile is empty
                        if (tile != null && tile.occupiedObject == null)
                        {
                            PlaceBuilding(gridPos, prefabToPlace, buildingCost);
                        }
                    }
                }
                // B. INSPECTOR MODE (Nothing selected)
                else
                {
                    // Check if we clicked a House
                    BuildingAffectable house = hit.collider.GetComponent<BuildingAffectable>();

                    if (house != null)
                    {
                        // Find the tile UNDER the house
                        Vector2Int gridPos = gridManager.WorldToGridCoordinates(house.transform.position);
                        TileData tile = gridManager.GetTileAt(gridPos);

                        if (tile != null && UIManager.Instance != null)
                        {
                            // Get the data from the tile!
                            int health = tile.GetMaxHealth();
                            int safety = tile.GetMaxSafety();

                            // Show it on UI
                            UIManager.Instance.ShowInspector(health, safety);
                        }
                    }
                    else
                    {
                        // If we clicked something else (like empty ground), hide the panel
                        if (UIManager.Instance != null) UIManager.Instance.HideInspector();
                    }
                }
            }
        }
    }

    private void PlaceBuilding(Vector2Int gridPos, GameObject prefab, int cost)
    {
        Vector3 spawnPos = new Vector3(
            gridPos.x * gridManager.tileSize,
            0,
            gridPos.y * gridManager.tileSize
        );

        GameObject newBuilding = Instantiate(prefab, spawnPos, Quaternion.identity);

        TileData tile = gridManager.GetTileAt(gridPos);
        if (tile != null)
        {
            tile.occupiedObject = newBuilding;
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.SpendMoney(cost);
        }
    }
}