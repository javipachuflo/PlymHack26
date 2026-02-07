using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed to easily find the Max value

public class TileData : MonoBehaviour
{
    // The "Heatmap": Stores which building (Vector2Int coordinates) provides what score (int)
    // We use Vector2Int as the key because it's a unique ID for grid positions.
    private Dictionary<Vector2Int, int> safetyInfluences = new Dictionary<Vector2Int, int>();

    // Visuals
    [SerializeField] private Renderer tileRenderer;

    // Call this when a building is placed near this tile
    public void AddInfluence(Vector2Int buildingPos, int score)
    {
        if (safetyInfluences.ContainsKey(buildingPos))
        {
            safetyInfluences[buildingPos] = score; // Update existing
        }
        else
        {
            safetyInfluences.Add(buildingPos, score); // Add new
        }

        UpdateVisuals();
    }

    // Call this when a building is removed
    public void RemoveInfluence(Vector2Int buildingPos)
    {
        if (safetyInfluences.ContainsKey(buildingPos))
        {
            safetyInfluences.Remove(buildingPos);
        }

        UpdateVisuals();
    }

    // The "Pull" logic: The tile calculates its own current max score
    public int GetCurrentSafetyScore()
    {
        if (safetyInfluences.Count == 0) return 0;

        // Linq magic: looks at all values and returns the highest one
        return safetyInfluences.Values.Max();
    }

    private void UpdateVisuals()
    {
        float score = GetCurrentSafetyScore();

        // Simple visual debugging: 0 is Red, 100 is Green
        // We divide by 100f to get a 0-1 value for Lerp
        tileRenderer.material.color = Color.Lerp(Color.red, Color.green, score / 100f);
    }
}