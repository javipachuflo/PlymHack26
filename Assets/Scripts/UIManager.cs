using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton reference

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text moneyText;

    [Header("Selection UI")]
    public GameObject[] selectionRings;

    [Header("Inspector UI")]
    public GameObject inspectorPanel; // The panel background
    public TMP_Text statText;         // The text inside the panel

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (CityScoreManager.Instance != null)
            scoreText.text = $"City Score: {CityScoreManager.Instance.currentCityScore:F0}";

        if (EconomyManager.Instance != null)
            moneyText.text = $"Money: ${EconomyManager.Instance.currentMoney}";
    }

    public void UpdateSelectionUI(int index)
    {
        for (int i = 0; i < selectionRings.Length; i++)
        {
            if (selectionRings[i] != null)
            {
                selectionRings[i].SetActive(i == index);
            }
        }
    }

    // NEW: Call this to show the house stats
    public void ShowInspector(int health, int safety)
    {
        if (inspectorPanel != null) inspectorPanel.SetActive(true);

        if (statText != null)
        {
            statText.text = $"House Stats:\nHealth: {health}\nSafety: {safety}";
        }
    }

    // NEW: Call this to hide the panel
    public void HideInspector()
    {
        if (inspectorPanel != null) inspectorPanel.SetActive(false);
    }
}