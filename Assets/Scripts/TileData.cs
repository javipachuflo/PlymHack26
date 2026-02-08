using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class TileData : MonoBehaviour
{
    // Two separate "Heatmaps"
    private Dictionary<Vector2Int, int> safetyInfluences = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, int> healthInfluences = new Dictionary<Vector2Int, int>();

    [SerializeField] private Renderer tileRenderer;

    // THE SIGNAL: Other scripts can listen to this!
    public event Action OnStateChanged;

    public GameObject occupiedObject;

    // We updated this to take a 'category' parameter
    public void AddInfluence(BuildingCategory category, Vector2Int buildingPos, int score)
    {
        if (category == BuildingCategory.Safety)
        {
            if (safetyInfluences.ContainsKey(buildingPos)) safetyInfluences[buildingPos] = score;
            else safetyInfluences.Add(buildingPos, score);
        }
        else if (category == BuildingCategory.Health)
        {
            if (healthInfluences.ContainsKey(buildingPos)) healthInfluences[buildingPos] = score;
            else healthInfluences.Add(buildingPos, score);
        }

        UpdateVisuals();
    }

    public void RemoveInfluence(BuildingCategory category, Vector2Int buildingPos)
    {
        if (category == BuildingCategory.Safety)
        {
            if (safetyInfluences.ContainsKey(buildingPos)) safetyInfluences.Remove(buildingPos);
        }
        else if (category == BuildingCategory.Health)
        {
            if (healthInfluences.ContainsKey(buildingPos)) healthInfluences.Remove(buildingPos);
        }

        UpdateVisuals();
    }

    public int GetMaxSafety()
    {
        if (safetyInfluences.Count == 0) return 0;
        return safetyInfluences.Values.Max();
    }

    public int GetMaxHealth()
    {
        if (healthInfluences.Count == 0) return 0;
        return healthInfluences.Values.Max();
    }

    // Helper for the House to get the color easily
    public Color GetCurrentColor()
    {
        return tileRenderer.material.color;
    }

    private void UpdateVisuals()
    {
        // Calculate Average
        float safety = GetMaxSafety();
        float health = GetMaxHealth();

        // If you add more metrics later, increase this divisor
        float averageScore = (safety + health) / 2f;

        // 0 is Red, 100 is Green
        tileRenderer.material.color = Color.Lerp(Color.red, Color.green, averageScore / 100f);

        // FIRE THE SIGNAL!
        // "Hey everyone listening, my data just changed!"
        OnStateChanged?.Invoke();
    }
}