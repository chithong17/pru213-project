using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardChoiceManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject cardChoicePanel;
    [SerializeField] private CardUI[] cardSlots; // kéo 3 card vào đây

    [Header("Cards")]
    [SerializeField] private List<CardData> allCards = new List<CardData>();

    private List<CardData> currentChoices = new List<CardData>();

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            ShowRandomCards();
        }
    }

    public void ShowRandomCards()
    {
        if (cardChoicePanel == null || cardSlots == null || cardSlots.Length == 0)
        {
            Debug.LogWarning("Missing card panel or card slots.");
            return;
        }

        cardChoicePanel.SetActive(true);
        Time.timeScale = 0f;

        currentChoices.Clear();

        // tạo pool tạm để random không trùng
        List<CardData> tempPool = new List<CardData>(allCards);

        int cardCount = Mathf.Min(cardSlots.Length, tempPool.Count);

        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].gameObject.SetActive(i < cardCount);
        }

        for (int i = 0; i < cardCount; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);

            CardData chosenCard = tempPool[randomIndex];
            tempPool.RemoveAt(randomIndex); // QUAN TRỌNG: bỏ ra để không bị trùng

            currentChoices.Add(chosenCard);
            cardSlots[i].Setup(chosenCard, this);
        }

        Debug.Log("Show random cards without duplicates.");
    }

    public void ChooseCard(CardData chosenCard)
    {
        ApplyCardEffect(chosenCard);

        cardChoicePanel.SetActive(false);
        Time.timeScale = 1f;

        Debug.Log("Chosen card: " + chosenCard.cardName);
    }

    private void ApplyCardEffect(CardData card)
    {
        switch (card.effectType)
        {
            case CardEffectType.AddRangeAll:
                ApplyRangeToAllTowers(card.value);
                break;

            case CardEffectType.AddDamageAll:
                ApplyDamageToAllTowers(card.value);
                break;

            case CardEffectType.AddGold:
                AddGold(card.value);
                break;
        }
    }

    private void ApplyRangeToAllTowers(float amount)
    {
        TowerAttack[] towers = FindObjectsByType<TowerAttack>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (TowerAttack tower in towers)
        {
            tower.attackRange += amount;
        }

        Debug.Log("All towers +" + amount + " range");
    }

    private void ApplyDamageToAllTowers(float amount)
    {
        TowerAttack[] towers = FindObjectsByType<TowerAttack>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (TowerAttack tower in towers)
        {
            tower.attackDamage += amount;
        }

        Debug.Log("All towers +" + amount + " damage");
    }

    private void AddGold(float amount)
    {
        CurrencyManager currencyManager = FindAnyObjectByType<CurrencyManager>();

        if (currencyManager != null)
        {
            currencyManager.AddGold(Mathf.RoundToInt(amount));
            Debug.Log("Gain +" + amount + " gold");
        }
    }
}