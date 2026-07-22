using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private TextMeshProUGUI cardText;
    [SerializeField] private Button button;
    [SerializeField] private float hoverVolume = 0.65f;
    [SerializeField] private float selectVolume = 0.85f;

    private CardData cardData;
    private CardChoiceManager cardChoiceManager;

    public void Setup(CardData data, CardChoiceManager manager)
    {
        cardData = data;
        cardChoiceManager = manager;

        cardText.text = data.cardName + "\n\n" + data.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickCard);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameAudio.PlaySFX(GameAudio.UiHover, hoverVolume);
    }

    private void OnClickCard()
    {
        GameAudio.PlaySFX(GameAudio.CardSelect, selectVolume);

        if (cardChoiceManager != null && cardData != null)
        {
            cardChoiceManager.ChooseCard(cardData);
        }
    }
}
