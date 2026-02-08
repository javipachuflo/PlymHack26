using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton reference

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text moneyText;

    [Header("Selection UI")]
    // Drag your 3 Green Ring images here!
    // Order: 0 = Police, 1 = Hospital, 2 = House
    public GameObject[] selectionRings;

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
            moneyText.text = $"Money: £{EconomyManager.Instance.currentMoney}";
    }

    public void UpdateSelectionUI(int index)
    {
        // Loop through all rings
        for (int i = 0; i < selectionRings.Length; i++)
        {
            if (selectionRings[i] != null)
            {
                // Turn ON if it matches the index, OFF if it doesn't
                selectionRings[i].SetActive(i == index);
            }
        }
    }
}