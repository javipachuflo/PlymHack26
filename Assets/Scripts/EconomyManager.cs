using UnityEngine;
using System.Collections;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Settings")]
    public float currentMoney = 500f;
    public float taxInterval = 5f; // Get paid every 5 seconds
    public float taxMultiplier = 1f; // Adjust this to balance the game

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Start the infinite tax loop
        StartCoroutine(CollectTaxesRoutine());
        Debug.Log("Started Coroutine");
    }

    private IEnumerator CollectTaxesRoutine()
    {
        Debug.Log("Started IEnumerator");
        
        while (true)
        {
            yield return new WaitForSeconds(taxInterval);
            CollectTaxes();
            Debug.Log("CollectTaxes() function fired");
        }
    }

    private void CollectTaxes()
    {
        Debug.Log("CollectTaxes() started");
        
        if (CityScoreManager.Instance != null)
        {
            Debug.Log("CityScoreManager is not null!");

            // Income = Sum of all house scores * multiplier
            float income = CityScoreManager.Instance.GetTotalTaxScore() * taxMultiplier;

            Debug.Log($"Calculating Tax... Income is: {income}");

            if (income > 0)
            {
                AddMoney(income);
                Debug.Log($"Payday! Earned ${income}. Balance: ${currentMoney}");
            }
        }
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
    }

    public bool CanAfford(float cost)
    {
        return currentMoney >= cost;
    }

    public void SpendMoney(float cost)
    {
        currentMoney -= cost;
        Debug.Log($"Spent ${cost}. Balance: ${currentMoney}");
    }
}