using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnBehaviour : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float respawnTime = 2.0f;
    public Vector2[] spawnPoints;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enemyWave());
    }

    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab) as GameObject;
        int positionSelector = Random.Range(0, spawnPoints.Length);
        enemy.transform.position = spawnPoints[positionSelector];
    }

    IEnumerator enemyWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            SpawnEnemy();
        }

    }
}
