using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 20;
    public int height = 20;
    public float tileSize = 1f;

    [Header("References")]
    public GameObject tilePrefab; // Drag your Tile prefab here (must have TileData script!)

    private TileData[,] grid; // 2D Array to store the actual scripts

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grid = new TileData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate world position
                Vector3 worldPos = new Vector3(x * tileSize, 0, y * tileSize);

                // Spawn the tile
                GameObject newTileObj = Instantiate(tilePrefab, worldPos, Quaternion.identity);
                newTileObj.transform.parent = this.transform;
                newTileObj.name = $"Tile_{x}_{y}";

                // Store the script in our 2D array for fast lookup later
                TileData tileData = newTileObj.GetComponent<TileData>();
                grid[x, y] = tileData;
            }
        }
    }

    // Helper: Converts a World Position (e.g. 5.5, 0, 3.2) to Grid Coordinates (5, 3)
    public Vector2Int WorldToGridCoordinates(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int z = Mathf.RoundToInt(worldPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    // Helper: Returns the TileData script at specific coordinates (Safe to call even if out of bounds)
    public TileData GetTileAt(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
        {
            return grid[pos.x, pos.y];
        }
        return null;
    }
}