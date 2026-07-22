using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [Header("Currency Settings")]
    public int startingGold = 50;
    public int currentGold;

    [Header("HUD")]
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        currentGold = startingGold;
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();

        Debug.Log("Gold: " + currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
        {
            return false;
        }

        currentGold -= amount;
        UpdateGoldUI();

        Debug.Log("Gold: " + currentGold);
        return true;
    }

    private void UpdateGoldUI()
    {
        if (goldText == null)
        {
            Debug.LogWarning(
                "Chưa gắn GoldText cho CurrencyManager! Object: "
                + gameObject.name
            );

            return;
        }

        goldText.text = currentGold.ToString();
    }
}