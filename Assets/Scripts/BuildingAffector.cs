using UnityEngine;

public class BuildingAffector : MonoBehaviour
{
    [Header("Settings")]
    public BuildingCategory category;
    public int radius = 3;
    public int maxScore = 100; // The score at the very center
    public int minScore = 20;
    public int cost = 100;

    private GridManager gridManager;
    private Vector2Int gridPosition;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        gridPosition = gridManager.WorldToGridCoordinates(transform.position);
        UpdateInfluence(true);
    }

    private void OnDestroy()
    {
        UpdateInfluence(false);
    }

    private void UpdateInfluence(bool isAdding)
    {
        if (gridManager == null) return;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int targetPos = new Vector2Int(gridPosition.x + x, gridPosition.y + y);

                // Calculate distance (Euclidean is good for circular falloff)
                float distance = Vector2Int.Distance(gridPosition, targetPos);

                if (distance <= radius)
                {
                    TileData tile = gridManager.GetTileAt(targetPos);

                    if (tile != null)
                    {
                        if (isAdding)
                        {
                            // Calculate the falloff score
                            int distanceScore = CalculateScore(distance);
                            tile.AddInfluence(category, gridPosition, distanceScore);
                        }
                        else
                        {
                            tile.RemoveInfluence(category, gridPosition);
                        }
                    }
                }
            }
        }
    }

    // New Helper Function
    private int CalculateScore(float distance)
    {
        // 1. Get a value from 0 to 1 representing "how far are we?"
        // 0 = center, 1 = edge
        float normalizedDist = distance / radius;

        // 2. Invert it so 1 is center and 0 is edge
        float strength = 1f - normalizedDist;

        // 3. Lerp between Min and Max based on strength
        // If strength is 0, we get minScore. If strength is 1, we get maxScore.
        float blendedScore = Mathf.Lerp(minScore, maxScore, strength);

        return Mathf.RoundToInt(blendedScore);
    }
}