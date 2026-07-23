using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TowerPlacementController : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private Tilemap buildableMap;
    [SerializeField] private GameObject previewMarker;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int towerCost = 10;
    [SerializeField] private float minimumTowerDistance = 1.45f;
    [SerializeField] private Color validPreviewColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color invalidPreviewColor = new Color(1f, 0.25f, 0.25f, 0.5f);

    private CurrencyManager currencyManager;
    private bool isBuildModeActive;
    private GameObject ghostTower;

    private readonly Dictionary<Vector3Int, bool> occupiedGridCells =
        new Dictionary<Vector3Int, bool>();

    private readonly List<Vector3> placedTowerPositions =
        new List<Vector3>();

    private static readonly Vector3Int[] occupiedOffsets =
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    private const int towerSortingBase = 3500;
    private const int sortingUnitsPerWorldUnit = 100;

    public bool IsBuildModeActive => isBuildModeActive;
    public int TowerCost => towerCost;

    private void Start()
    {
        currencyManager = FindAnyObjectByType<CurrencyManager>();

        CreateGhostTowerPreview();
        HidePreviewMarker();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleBuildMode();
        }

        if (isBuildModeActive)
        {
            HandleTowerPlacement();
        }
    }

    public void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;
        GameAudio.PlaySFX(GameAudio.UiClick, 0.65f);

        if (!isBuildModeActive)
        {
            HidePreviewMarker();
        }
    }

    private void CreateGhostTowerPreview()
    {
        if (towerPrefab == null || previewMarker == null)
        {
            Debug.LogWarning(
                "TowerPlacementController is missing Tower Prefab or Preview Marker."
            );

            return;
        }

        ghostTower = Instantiate(towerPrefab);
        ghostTower.name = "GhostTowerPreview";

        ghostTower.transform.SetParent(previewMarker.transform);
        ghostTower.transform.localPosition = Vector3.zero;
        ghostTower.transform.localRotation = Quaternion.identity;

        MonoBehaviour[] scripts =
            ghostTower.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        Collider2D[] colliders =
            ghostTower.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        SpriteRenderer[] sprites =
            ghostTower.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = validPreviewColor;
            SetTowerSortingOrder(sprite, ghostTower.transform.position.y);
        }
    }

    private void HandleTowerPlacement()
    {
        if (Mouse.current == null ||
            Camera.main == null ||
            buildableMap == null)
        {
            return;
        }

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = 0f;

        Vector3Int targetCellPosition =
            buildableMap.WorldToCell(mouseWorldPosition);

        bool isCellBuildable =
            buildableMap.HasTile(targetCellPosition);

        Vector3 cellCenterPosition =
            buildableMap.GetCellCenterWorld(targetCellPosition);

        cellCenterPosition.z = 0f;

        bool canBuildHere =
            isCellBuildable &&
            !IsPlacementAreaOccupied(targetCellPosition) &&
            !IsTooCloseToExistingTower(cellCenterPosition);

        if (isCellBuildable)
        {
            ShowPreviewMarker(targetCellPosition, canBuildHere);

            if (Mouse.current.leftButton.wasPressedThisFrame && canBuildHere)
            {
                AttemptToBuildTower(targetCellPosition);
            }
        }
        else
        {
            HidePreviewMarker();
        }
    }

    private bool IsPlacementAreaOccupied(Vector3Int cellPosition)
    {
        return occupiedGridCells.TryGetValue(
            cellPosition,
            out bool occupied
        ) && occupied;
    }

    private bool IsTooCloseToExistingTower(Vector3 worldPosition)
    {
        foreach (Vector3 placedTowerPosition in placedTowerPositions)
        {
            if (Vector3.Distance(worldPosition, placedTowerPosition) < minimumTowerDistance)
            {
                return true;
            }
        }

        return false;
    }

    private void MarkPlacementAreaOccupied(Vector3Int centerCell)
    {
        foreach (Vector3Int offset in occupiedOffsets)
        {
            Vector3Int cellToMark = centerCell + offset;
            occupiedGridCells[cellToMark] = true;
        }
    }

    private void ShowPreviewMarker(Vector3Int cellPosition, bool canBuildHere)
    {
        if (previewMarker == null || buildableMap == null)
        {
            return;
        }

        previewMarker.SetActive(true);

        Vector3 cellCenterPosition =
            buildableMap.GetCellCenterWorld(cellPosition);

        cellCenterPosition.z = 0f;
        previewMarker.transform.position = cellCenterPosition;

        SetGhostPreviewColor(
            canBuildHere ? validPreviewColor : invalidPreviewColor
        );
        SetGhostPreviewSortingOrder(cellCenterPosition.y);
    }

    private void HidePreviewMarker()
    {
        if (previewMarker != null)
        {
            previewMarker.SetActive(false);
        }
    }

    private void SetGhostPreviewColor(Color previewColor)
    {
        if (ghostTower == null)
        {
            return;
        }

        SpriteRenderer[] sprites =
            ghostTower.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = previewColor;
        }
    }

    private void AttemptToBuildTower(Vector3Int cellPosition)
    {
        if (towerPrefab == null)
        {
            Debug.LogWarning("Tower prefab is missing.");
            return;
        }

        if (currencyManager == null)
        {
            Debug.LogWarning("CurrencyManager was not found.");
            return;
        }

        Vector3 spawnPosition =
            buildableMap.GetCellCenterWorld(cellPosition);

        spawnPosition.z = 0f;

        if (IsPlacementAreaOccupied(cellPosition) ||
            IsTooCloseToExistingTower(spawnPosition))
        {
            Debug.LogWarning("This position is too close to another tower.");
            return;
        }

        if (!currencyManager.SpendGold(towerCost))
        {
            GameAudio.PlaySFX(GameAudio.NotEnoughGold, 0.8f);
            Debug.LogWarning("Not enough gold to build tower.");
            return;
        }

        GameAudio.PlaySFX(GameAudio.BuildTower, 0.85f);

        GameObject newTower = Instantiate(
            towerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        RestoreTowerVisual(newTower);
        MarkPlacementAreaOccupied(cellPosition);
        placedTowerPositions.Add(spawnPosition);

        isBuildModeActive = false;
        HidePreviewMarker();

        Debug.Log("Tower built at cell: " + cellPosition);
    }

    private void RestoreTowerVisual(GameObject tower)
    {
        if (tower == null)
        {
            return;
        }

        SpriteRenderer[] spriteRenderers =
            tower.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = true;

            Color spriteColor = spriteRenderer.color;
            spriteColor.a = 1f;
            spriteRenderer.color = spriteColor;

            SetTowerSortingOrder(spriteRenderer, tower.transform.position.y);
        }
    }

    private void SetGhostPreviewSortingOrder(float worldY)
    {
        if (ghostTower == null)
        {
            return;
        }

        SpriteRenderer[] sprites =
            ghostTower.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in sprites)
        {
            SetTowerSortingOrder(sprite, worldY);
        }
    }

    private void SetTowerSortingOrder(SpriteRenderer spriteRenderer, float worldY)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        int baseOrder = towerSortingBase -
            Mathf.RoundToInt(worldY * sortingUnitsPerWorldUnit);

        if (spriteRenderer.gameObject.name == "TowerBase")
        {
            spriteRenderer.sortingOrder = baseOrder;
        }
        else if (spriteRenderer.gameObject.name == "ArcherVisual")
        {
            spriteRenderer.sortingOrder = baseOrder + 1;
        }
        else
        {
            spriteRenderer.sortingOrder = baseOrder + 2;
        }
    }
}


