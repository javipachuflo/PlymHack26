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
    private int selectedIndex = -1;

    private void Start()
    {
        // CHANGE 2: Update UI to show nothing selected at start
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSelectionUI(-1);
        }
    }

    private void Update()
    {
        // KEYBOARD INPUT
        // 1 -> Police (Index 0)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Toggle Logic: If already 0, switch to -1. Otherwise, make it 0.
            if (selectedIndex == 0) selectedIndex = -1;
            else selectedIndex = 0;

            if (UIManager.Instance != null) UIManager.Instance.UpdateSelectionUI(selectedIndex);
        }
        // 2 -> Hospital (Index 1)
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (selectedIndex == 1) selectedIndex = -1;
            else selectedIndex = 1;

            if (UIManager.Instance != null) UIManager.Instance.UpdateSelectionUI(selectedIndex);
        }
        // 0 -> House (Index 2)
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (selectedIndex == 2) selectedIndex = -1;
            else selectedIndex = 2;

            if (UIManager.Instance != null) UIManager.Instance.UpdateSelectionUI(selectedIndex);
        }

        // MOUSE INPUT
        // We add a check: "selectedIndex >= 0" to ensure we only build if something is selected!
        if (selectedIndex >= 0 && Input.GetMouseButtonDown(0))
        {
            HandleInput(false);
        }

        // Removal (Right Click) works regardless of selection
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
                if (selectedIndex >= 0 && buildingPrefabs.Length > selectedIndex)
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