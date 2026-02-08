using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Helps with summing up the list

public class CityScoreManager : MonoBehaviour
{
    // Singleton instance so houses can find it easily
    public static CityScoreManager Instance;

    // The Registry: A list of all active houses
    private List<BuildingAffectable> allHouses = new List<BuildingAffectable>();

    [Header("Read Only")]
    public float currentCityScore = 0f;

    private void Awake()
    {
        // Basic Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterHouse(BuildingAffectable house)
    {
        if (!allHouses.Contains(house))
        {
            allHouses.Add(house);
            RecalculateScore();
        }
    }

    public void UnregisterHouse(BuildingAffectable house)
    {
        if (allHouses.Contains(house))
        {
            allHouses.Remove(house);
            RecalculateScore();
        }
    }

    // Call this whenever a house is added/removed OR when a house changes color
    public void RecalculateScore()
    {
        if (allHouses.Count == 0)
        {
            currentCityScore = 0;
            return;
        }

        float totalScore = 0;

        // Loop through the registry
        foreach (var house in allHouses)
        {
            totalScore += house.GetScore();
        }

        // Average it out
        currentCityScore = totalScore / allHouses.Count;

        Debug.Log($"City Score Updated: {currentCityScore}");
    }
}