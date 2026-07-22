using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerSelection : MonoBehaviour
{
    public static TowerSelection Instance { get; private set; }

    [SerializeField] private TowerInfoPanel towerInfoPanel;

    private TowerAttack selectedTower;
    private Camera mainCamera;

    public TowerAttack SelectedTower => selectedTower;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        // Nếu đang click vào UI như panel, button Upgrade thì không ẩn
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 mouseWorldPosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPosition2D);

        foreach (Collider2D hit in hits)
        {
            TowerClick towerClick = hit.GetComponent<TowerClick>();

            if (towerClick != null)
            {
                // Click trúng tower thì không ẩn panel.
                // TowerClick.OnMouseDown sẽ tự gọi SelectTower.
                return;
            }
        }

        ClearSelection();
    }

    public void SelectTower(TowerAttack tower)
    {
        if (selectedTower != null && selectedTower != tower)
        {
            selectedTower.HideRangeCircle();
        }

        selectedTower = tower;

        selectedTower.ShowRangeCircle();

        Debug.Log("Selected tower: " + tower.name + " Lv" + tower.Level);

        if (towerInfoPanel != null)
        {
            towerInfoPanel.Show(tower);
        }
    }

    public void ClearSelection()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRangeCircle();
        }

        selectedTower = null;

        if (towerInfoPanel != null)
        {
            towerInfoPanel.Hide();
        }
    }
}