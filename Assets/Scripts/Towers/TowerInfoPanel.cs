using UnityEngine;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    private TowerAttack currentTower;

    private void Start()
    {
        Hide();
    }

    public void Show(TowerAttack tower)
    {
        currentTower = tower;

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        Refresh();
    }

    public void Hide()
    {
        currentTower = null;

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (currentTower == null)
        {
            return;
        }

        titleText.text = "Tower Info";
        levelText.text = "Level: " + currentTower.Level;
        damageText.text = "Damage: " + currentTower.AttackDamage;
        rangeText.text = "Range: " + currentTower.AttackRange;
        cooldownText.text = "Cooldown: " + currentTower.AttackCooldown.ToString("0.00");

        if (currentTower.CanUpgrade)
        {
            upgradeCostText.text = "Upgrade Cost: " + currentTower.UpgradeCost;
        }
        else
        {
            upgradeCostText.text = "Max Level";
        }
    }

    public void UpgradeCurrentTower()
    {
        if (currentTower == null)
        {
            return;
        }

        bool upgraded = currentTower.TryUpgrade();

        if (upgraded)
        {
            Refresh();
        }
    }
}