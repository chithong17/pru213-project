using UnityEngine;

public enum CardEffectType
{
    AddRangeAll,
    AddDamageAll,
    AddGold
}

[System.Serializable]
public class CardData
{
    public string cardName;
    [TextArea]
    public string description;
    public CardEffectType effectType;
    public float value;
}