using UnityEngine;
using TMPro; // Required to talk to TextMeshPro!

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text moneyText;

    private void Update()
    {
        // 1. Get Score
        if (CityScoreManager.Instance != null)
        {
            // "F0" formats it to 0 decimal places (whole number)
            scoreText.text = $"City Score: {CityScoreManager.Instance.currentCityScore:F0}";
        }

        // 2. Get Money
        if (EconomyManager.Instance != null)
        {
            moneyText.text = $"Money: ${EconomyManager.Instance.currentMoney}";
        }
    }
}