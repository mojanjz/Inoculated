using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnBehaviour : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float respawnTime = 4.0f;
    public Vector2[] spawnPoints;

    public int InitialCount = 3;
    public int Count = 0;
    [SerializeField] int MaxCount = 6;
    

    // Start is called before the first frame update
    void Start()
    {
        while(Count < InitialCount)
        {
            SpawnEnemy();
        }

        StartCoroutine(enemyWave());
    }

    public void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab) as GameObject;
        int positionSelector = Random.Range(0, spawnPoints.Length);
        enemy.transform.position = spawnPoints[positionSelector];

        enemy.GetComponent<Attackable>().EnemySpawner = this;
        Count++;
    }

    IEnumerator enemyWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);

            if( Count < MaxCount )
            {
                SpawnEnemy();
            }
        }

    }
}
