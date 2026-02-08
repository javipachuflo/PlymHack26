using UnityEngine;

public class BuildingAffectable : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer buildingRenderer;

    private GridManager gridManager;
    private TileData currentTile;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager == null) return;

        Vector2Int gridPos = gridManager.WorldToGridCoordinates(transform.position);
        currentTile = gridManager.GetTileAt(gridPos);

        if (currentTile != null)
        {
            currentTile.OnStateChanged += HandleTileChange;
            HandleTileChange(); // Update immediately
        }

        // NEW: Register ourselves with the City Manager
        if (CityScoreManager.Instance != null)
        {
            CityScoreManager.Instance.RegisterHouse(this);
        }
    }

    private void OnDestroy()
    {
        if (currentTile != null)
        {
            currentTile.OnStateChanged -= HandleTileChange;
        }

        // NEW: Remove ourselves from the list
        if (CityScoreManager.Instance != null)
        {
            CityScoreManager.Instance.UnregisterHouse(this);
        }
    }

    private void HandleTileChange()
    {
        UpdateColor();

        // NEW: Tell the manager to recalculate because my score changed!
        if (CityScoreManager.Instance != null)
        {
            CityScoreManager.Instance.RecalculateScore();
        }
    }

    private void UpdateColor()
    {
        if (buildingRenderer != null && currentTile != null)
        {
            buildingRenderer.material.color = currentTile.GetCurrentColor();
        }
    }

    // NEW: Helper method for the Manager to read
    public float GetScore()
    {
        if (currentTile == null) return 0;

        // We need to calculate the average score (0-100)
        float safety = currentTile.GetMaxSafety();
        float health = currentTile.GetMaxHealth();
        return (safety + health) / 2f;
    }
}