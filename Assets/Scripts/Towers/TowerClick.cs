using UnityEngine;

public class TowerClick : MonoBehaviour
{
    [SerializeField] private TowerAttack towerAttack;

    private void Awake()
    {
        if (towerAttack == null)
        {
            towerAttack = GetComponent<TowerAttack>();
        }
    }

    private void OnMouseDown()
    {
        if (towerAttack == null)
        {
            return;
        }

        if (TowerSelection.Instance != null)
        {
            TowerSelection.Instance.SelectTower(towerAttack);
        }
    }
}