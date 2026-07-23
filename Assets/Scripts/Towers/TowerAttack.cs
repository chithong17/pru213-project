using UnityEngine;
using System.Collections;

public class TowerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public float attackDamage = 1f;
    public float attackCooldown = 1f;

    [Header("References")]
    [SerializeField] private TowerArcherVisual archerVisual;
    [SerializeField] private GameObject rangeCircle;
    [SerializeField] private LineRenderer rangeLine;
    [SerializeField] private RangeArcDrawer rangeSweep;
    [SerializeField] private RadarSweepDrawer radarSweep;

    [Header("Level Prefabs")]
    [SerializeField] private GameObject[] levelPrefabs = new GameObject[3];

    [Header("Upgrade Settings")]
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private int upgradeCost = 20;

    [SerializeField] private float damageIncreasePerLevel = 1f;
    [SerializeField] private float rangeIncreasePerLevel = 0.5f;
    [SerializeField] private float cooldownDecreasePerLevel = 0.1f;

    private float cooldownTimer;

    public int Level => level;
    public int UpgradeCost => upgradeCost;
    public bool CanUpgrade => level < maxLevel;

    public float AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    private void Start()
    {
        ApplyLevelPrefabVisual();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer > 0f)
        {
            return;
        }

        EnemyHealth targetEnemy = FindFirstEnemyInRange();

        if (targetEnemy == null)
        {
            return;
        }

        if (archerVisual != null)
        {
            archerVisual.PlayShoot(targetEnemy, attackDamage);
        }

        cooldownTimer = attackCooldown;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade)
        {
            Debug.Log("Tower is already max level.");
            return false;
        }

        CurrencyManager currencyManager = FindAnyObjectByType<CurrencyManager>();

        if (currencyManager == null)
        {
            Debug.LogWarning("CurrencyManager not found.");
            return false;
        }

        if (!currencyManager.SpendGold(upgradeCost))
        {
            GameAudio.PlaySFX(GameAudio.NotEnoughGold, 0.8f);
            Debug.Log("Not enough gold to upgrade.");
            return false;
        }

        level++;

        attackDamage += damageIncreasePerLevel;
        attackRange += rangeIncreasePerLevel;
        attackCooldown = Mathf.Max(0.2f, attackCooldown - cooldownDecreasePerLevel);

        upgradeCost += 15;

        ApplyLevelPrefabVisual();
        if (rangeCircle != null && rangeCircle.activeSelf)
        {
            ShowRangeCircle();
        }

        GameAudio.PlaySFX(GameAudio.TowerUpgrade, 0.85f);
        StartCoroutine(UpgradePopEffect());
        Debug.Log("Tower upgraded to level " + level);
        return true;
    }

    private IEnumerator UpgradePopEffect()
    {
        Vector3 originalScale = transform.localScale;

        transform.localScale = originalScale * 1.08f;
        yield return new WaitForSeconds(0.1f);

        transform.localScale = originalScale;
    }

    private void ApplyLevelPrefabVisual()
    {
        GameObject levelPrefab = GetLevelPrefab();
        if (levelPrefab == null)
        {
            return;
        }

        CopyChildVisual(levelPrefab, "TowerBase");
        CopyChildVisual(levelPrefab, "ArcherVisual");
        CopyFirePoint(levelPrefab);
        CopyCollider(levelPrefab);
        ApplyLevelDepthSorting();
    }

    private GameObject GetLevelPrefab()
    {
        int index = level - 1;

        if (levelPrefabs == null || index < 0 || index >= levelPrefabs.Length)
        {
            return null;
        }

        return levelPrefabs[index];
    }

    private void CopyChildVisual(GameObject sourcePrefab, string childName)
    {
        Transform sourceChild = sourcePrefab.transform.Find(childName);
        Transform targetChild = transform.Find(childName);

        if (sourceChild == null || targetChild == null)
        {
            return;
        }

        targetChild.localPosition = sourceChild.localPosition;
        targetChild.localRotation = sourceChild.localRotation;
        targetChild.localScale = sourceChild.localScale;

        SpriteRenderer sourceRenderer = sourceChild.GetComponent<SpriteRenderer>();
        SpriteRenderer targetRenderer = targetChild.GetComponent<SpriteRenderer>();

        if (sourceRenderer == null || targetRenderer == null)
        {
            return;
        }

        targetRenderer.sprite = sourceRenderer.sprite;
        targetRenderer.color = sourceRenderer.color;
        targetRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
    }

    private void ApplyLevelDepthSorting()
    {
        const int towerSortingBase = 3500;
        const int sortingUnitsPerWorldUnit = 100;

        int baseOrder = towerSortingBase -
            Mathf.RoundToInt(transform.position.y * sortingUnitsPerWorldUnit);

        SetChildSortingOrder("TowerBase", baseOrder);
        SetChildSortingOrder("ArcherVisual", baseOrder + 1);
    }

    private void SetChildSortingOrder(string childName, int sortingOrder)
    {
        Transform child = transform.Find(childName);

        if (child == null)
        {
            return;
        }

        SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            return;
        }

        renderer.sortingOrder = sortingOrder;
    }

    private void CopyFirePoint(GameObject sourcePrefab)
    {
        Transform sourceFirePoint = sourcePrefab.transform.Find("ArcherVisual/FirePoint");
        Transform targetFirePoint = transform.Find("ArcherVisual/FirePoint");

        if (sourceFirePoint == null || targetFirePoint == null)
        {
            return;
        }

        targetFirePoint.localPosition = sourceFirePoint.localPosition;
        targetFirePoint.localRotation = sourceFirePoint.localRotation;
        targetFirePoint.localScale = sourceFirePoint.localScale;
    }

    private void CopyCollider(GameObject sourcePrefab)
    {
        BoxCollider2D sourceCollider = sourcePrefab.GetComponent<BoxCollider2D>();
        BoxCollider2D targetCollider = GetComponent<BoxCollider2D>();

        if (sourceCollider == null || targetCollider == null)
        {
            return;
        }

        targetCollider.offset = sourceCollider.offset;
        targetCollider.size = sourceCollider.size;
    }

    private EnemyHealth FindFirstEnemyInRange()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (EnemyHealth enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= attackRange)
            {
                return enemy;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ShowRangeCircle()
    {
        Debug.Log("ShowRangeCircle called");

        if (rangeCircle == null)
        {
            Debug.LogWarning("Range Circle is missing.");
            return;
        }

        if (rangeLine == null)
        {
            Debug.LogWarning("Range Line is missing.");
            return;
        }

        rangeCircle.SetActive(true);

        rangeCircle.transform.localPosition = Vector3.zero;
        rangeCircle.transform.localRotation = Quaternion.identity;
        rangeCircle.transform.localScale = Vector3.one;

        int pointCount = 100;
        rangeLine.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pointCount;

            float x = Mathf.Cos(angle) * attackRange;
            float y = Mathf.Sin(angle) * attackRange;

            rangeLine.SetPosition(i, new Vector3(x, y, 0f));
        }

        EnsureRangeSweep();

        if (rangeSweep != null)
        {
            rangeSweep.SetRadius(attackRange);
        }

        if (radarSweep != null)
        {
            radarSweep.SetRadius(attackRange);
        }
    }

    private void EnsureRangeSweep()
    {
        if (rangeCircle == null)
        {
            return;
        }

        if (rangeSweep == null)
        {
            rangeSweep = rangeCircle.GetComponentInChildren<RangeArcDrawer>(true);
        }

        if (rangeSweep != null)
        {
            CopyRangeLineStyle(rangeSweep.GetComponent<LineRenderer>());
            EnsureRadarSweep(rangeSweep.gameObject);
            return;
        }

        GameObject sweepObject = new GameObject("RangeSweep");
        sweepObject.transform.SetParent(rangeCircle.transform, false);

        LineRenderer sweepLine = sweepObject.AddComponent<LineRenderer>();
        CopyRangeLineStyle(sweepLine);

        sweepObject.AddComponent<RotateRangeSweep>();
        rangeSweep = sweepObject.AddComponent<RangeArcDrawer>();
        EnsureRadarSweep(sweepObject);
    }

    private void EnsureRadarSweep(GameObject sweepObject)
    {
        if (sweepObject == null)
        {
            return;
        }

        if (radarSweep == null)
        {
            radarSweep = sweepObject.GetComponent<RadarSweepDrawer>();
        }

        if (radarSweep == null)
        {
            radarSweep = sweepObject.AddComponent<RadarSweepDrawer>();
        }

        if (rangeLine != null)
        {
            radarSweep.SetSorting(rangeLine.sortingLayerID, rangeLine.sortingOrder);
        }
    }
    private void CopyRangeLineStyle(LineRenderer sweepLine)
    {
        if (sweepLine == null || rangeLine == null)
        {
            return;
        }

        sweepLine.useWorldSpace = false;
        sweepLine.loop = false;
        sweepLine.alignment = rangeLine.alignment;
        sweepLine.material = rangeLine.material;
        sweepLine.widthMultiplier = rangeLine.widthMultiplier * 1.25f;
        sweepLine.colorGradient = rangeLine.colorGradient;
        sweepLine.sortingLayerID = rangeLine.sortingLayerID;
        sweepLine.sortingOrder = rangeLine.sortingOrder + 1;
        sweepLine.numCapVertices = rangeLine.numCapVertices;
        sweepLine.numCornerVertices = rangeLine.numCornerVertices;
    }

    public void HideRangeCircle()
    {
        if (rangeCircle == null)
        {
            return;
        }

        rangeCircle.SetActive(false);
    }

    public void AddDamage(float amount)
    {
        attackDamage += amount;
    }

    public void AddRange(float amount)
    {
        attackRange += amount;

        if (rangeCircle != null && rangeCircle.activeSelf)
        {
            ShowRangeCircle();
        }
    }

    public void MultiplyCooldown(float multiplier)
    {
        attackCooldown *= multiplier;
        attackCooldown = Mathf.Max(0.2f, attackCooldown);
    }
}




