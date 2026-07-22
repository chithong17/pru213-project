using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public BaseHealth baseHealth;
    public WaveManager waveManager;

    // ĐỔI TÊN HỆ THỐNG MỚI VÀO ĐÂY
    public TowerPlacementController towerPlacementController;

    public Text goldText;
    public Text baseHealthText;
    public Text waveText;
    public Text buildStatusText;
    public Button buildTowerButton;

    private GUIStyle fallbackHudStyle;

    private void Start()
    {
        FindReferences();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void FindReferences()
    {
        if (currencyManager == null) currencyManager = FindAnyObjectByType<CurrencyManager>();
        if (baseHealth == null) baseHealth = FindAnyObjectByType<BaseHealth>();
        if (waveManager == null) waveManager = FindAnyObjectByType<WaveManager>();

        // TÌM HỆ THỐNG XÂY DỰNG MỚI
        if (towerPlacementController == null)
        {
            towerPlacementController = FindAnyObjectByType<TowerPlacementController>();
        }
    }

    private void CreateSimpleUIIfNeeded()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (canvas.transform is RectTransform canvasRectTransform)
        {
            canvasRectTransform.localScale = Vector3.one;
        }

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        if (goldText == null) goldText = CreateText(canvas.transform, "GoldText", new Vector2(100f, -20f));
        if (baseHealthText == null) baseHealthText = CreateText(canvas.transform, "BaseHealthText", new Vector2(100f, -50f));
        if (waveText == null) waveText = CreateText(canvas.transform, "WaveText", new Vector2(100f, -80f));

        if (buildStatusText == null)
        {
            buildStatusText = CreateText(canvas.transform, "BuildStatusText", new Vector2(100f, -110f));
            buildStatusText.rectTransform.sizeDelta = new Vector2(420f, 30f);
        }

        if (buildTowerButton == null)
        {
            buildTowerButton = CreateButton(
                canvas.transform,
                "BuildTowerButton",
                new Vector2(120f, -150f),
                new Vector2(180f, 40f),
                "Build Tower"
            );
        }

        buildTowerButton.onClick.RemoveAllListeners();
        buildTowerButton.onClick.AddListener(OnBuildTowerButtonClicked);
    }

    private Text CreateText(Transform parent, string objectName, Vector2 position)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(250f, 30f);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;

        return text;
    }

    private Button CreateButton(Transform parent, string objectName, Vector2 position, Vector2 size, string label)
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.12f, 0.24f, 0.16f, 0.9f);

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = image.color;
        buttonColors.highlightedColor = new Color(0.18f, 0.34f, 0.22f, 0.95f);
        buttonColors.pressedColor = new Color(0.08f, 0.18f, 0.12f, 1f);
        buttonColors.selectedColor = buttonColors.highlightedColor;
        buttonColors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        button.colors = buttonColors;

        Text buttonText = CreateText(buttonObject.transform, objectName + "Text", Vector2.zero);
        buttonText.text = label;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.rectTransform.anchorMin = Vector2.zero;
        buttonText.rectTransform.anchorMax = Vector2.one;
        buttonText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonText.rectTransform.anchoredPosition = Vector2.zero;
        buttonText.rectTransform.sizeDelta = Vector2.zero;

        return button;
    }

    private void OnBuildTowerButtonClicked()
    {
        // GỌI HÀM TOGGLE CỦA HỆ THỐNG MỚI
        if (towerPlacementController != null)
        {
            towerPlacementController.ToggleBuildMode();
        }
    }

    private void UpdateUI()
    {
        if (goldText != null && currencyManager != null)
            goldText.text = "Gold: " + currencyManager.currentGold;

        if (baseHealthText != null && baseHealth != null)
            baseHealthText.text = "Base HP: " + baseHealth.currentHealth + " / " + baseHealth.maxHealth;

        if (waveText != null && waveManager != null)
            waveText.text = "Wave: " + waveManager.currentWave;

        if (buildTowerButton != null)
        {
            Text buttonText = buildTowerButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = towerPlacementController != null && towerPlacementController.IsBuildModeActive
                    ? "Cancel Build"
                    : "Build Tower";
            }
        }

        if (buildStatusText != null)
        {
            if (towerPlacementController == null || !towerPlacementController.IsBuildModeActive)
            {
                buildStatusText.text = "Build: press Q or Build Tower to preview";
            }
            else
            {
                bool canAfford = currencyManager != null && currencyManager.currentGold >= towerPlacementController.TowerCost;

                if (!canAfford)
                {
                    buildStatusText.text = "Build: not enough gold (" + towerPlacementController.TowerCost + ")";
                }
                else
                {
                    buildStatusText.text = "Build: move mouse and click to place (" + towerPlacementController.TowerCost + " gold)";
                }
            }
        }
    }

    private void CreateFallbackHudStyle()
    {
        fallbackHudStyle = new GUIStyle();
        fallbackHudStyle.alignment = TextAnchor.UpperLeft;
        fallbackHudStyle.fontSize = 18;
        fallbackHudStyle.normal.textColor = Color.white;
        fallbackHudStyle.padding = new RectOffset(10, 10, 10, 10);
    }

    //private void OnGUI()
    //{
    //    if (fallbackHudStyle == null) CreateFallbackHudStyle();

    //    string goldValue = currencyManager == null ? "?" : currencyManager.currentGold.ToString();
    //    string hpValue = baseHealth == null ? "?" : (baseHealth.currentHealth + " / " + baseHealth.maxHealth);
    //    string waveValue = waveManager == null ? "?" : waveManager.currentWave.ToString();
    //    string buildValue = towerPlacementController == null || !towerPlacementController.IsBuildModeActive ? "OFF" : "ON";

    //    string hudText =
    //        "Gold: " + goldValue + "\n" +
    //        "Base HP: " + hpValue + "\n" +
    //        "Wave: " + waveValue + "\n" +
    //        "Build: " + buildValue;

    //    GUI.Box(new Rect(12f, 160f, 220f, 110f), hudText, fallbackHudStyle);
    //}
}