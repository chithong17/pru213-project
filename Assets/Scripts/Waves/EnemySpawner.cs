using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform enemiesParent;
    public Transform waypointsParent;
    public Transform endPoint;

    public void SpawnEnemy(GameObject enemyPrefabToSpawn)
    {
        if (enemyPrefabToSpawn == null || spawnPoint == null)
        {
            return;
        }

        GameObject newEnemy = Instantiate(
            enemyPrefabToSpawn, 
            spawnPoint.position,
            Quaternion.identity,
            enemiesParent
        );

        newEnemy.SetActive(true);

        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement != null)
        {
            // SỬA Ở ĐÂY: Gọi cái hàm SetupWaypoints ở bên EnemyMovement để truyền bản đồ vào
            enemyMovement.SetupWaypoints(GetWaypoints(), endPoint);
        }
    }

    private Transform[] GetWaypoints()
    {
        if (waypointsParent == null) return new Transform[0];

        Transform[] waypoints = new Transform[waypointsParent.childCount];
        for (int i = 0; i < waypointsParent.childCount; i++)
        {
            waypoints[i] = waypointsParent.GetChild(i);
        }
        return waypoints;
    }
}