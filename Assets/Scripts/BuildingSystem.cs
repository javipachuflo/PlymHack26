using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Camera mainCamera;

    [Header("Building Palette")]
    // This is the Array! It allows you to drag multiple prefabs here.
    public GameObject[] buildingPrefabs;

    // We keep track of which one is currently selected (starts at 0)
    private int selectedIndex = 0;

    private void Update()
    {
        // KEYBOARD INPUT FOR SELECTION
        // 1 -> Police (Index 0)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedIndex = 0;
            Debug.Log("Selected: Police Station");
        }
        // 2 -> Hospital (Index 1)
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedIndex = 1;
            Debug.Log("Selected: Hospital");
        }
        // 0 -> House (Index 2)
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            selectedIndex = 2;
            Debug.Log("Selected: House");
        }

        // MOUSE INPUT FOR ACTIONS
        if (Input.GetMouseButtonDown(0)) HandleInput(false); // Place
        if (Input.GetMouseButtonDown(1)) HandleInput(true);  // Remove
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
                // 1. Check for Special Buildings
                BuildingAffector affector = hit.collider.GetComponent<BuildingAffector>();
                if (affector != null)
                {
                    if (EconomyManager.Instance != null) EconomyManager.Instance.AddMoney(affector.cost);
                    Destroy(affector.gameObject);
                }

                // 2. Check for Houses
                BuildingAffectable affectable = hit.collider.GetComponent<BuildingAffectable>();
                if (affectable != null)
                {
                    // Refund logic for houses could go here if they had a cost
                    Destroy(affectable.gameObject);
                }
            }
            // PLACEMENT LOGIC
            else
            {
                // Safety Check: Make sure our array isn't empty!
                if (buildingPrefabs.Length > selectedIndex)
                {
                    GameObject prefabToPlace = buildingPrefabs[selectedIndex];

                    // Get Cost
                    BuildingAffector affector = prefabToPlace.GetComponent<BuildingAffector>();
                    int buildingCost = affector != null ? affector.cost : 0;
                    // Note: If houses don't have BuildingAffector, cost is 0, which is fine.

                    // Check Money
                    if (EconomyManager.Instance != null && !EconomyManager.Instance.CanAfford(buildingCost))
                    {
                        Debug.Log("Not enough money!");
                        return;
                    }

                    Vector2Int gridPos = gridManager.WorldToGridCoordinates(hit.point);
                    TileData tile = gridManager.GetTileAt(gridPos);

                    if (tile != null)
                    {
                        PlaceBuilding(gridPos, prefabToPlace, buildingCost);
                    }
                }
            }
        }
    }

    private void PlaceBuilding(Vector2Int gridPos, GameObject prefab, int cost)
    {
        // Simple overlap check could go here

        Vector3 spawnPos = new Vector3(
            gridPos.x * gridManager.tileSize,
            0,
            gridPos.y * gridManager.tileSize
        );

        Instantiate(prefab, spawnPos, Quaternion.identity);

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.SpendMoney(cost);
        }
    }
}