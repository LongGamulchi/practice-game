using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;
    

    void Start()
    {
        NextWave();
    }//첫 웨이브 시작


    void Update()
    {
        if(enemiesRemainingToSpawn> 0 && Time.time> nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, transform.position, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }//스폰 시간에 맞춰 적을 스폰하고 적이 생선 될 때 마다 OnDeath 이벤트에 OnEnemyDeath메소드를 추가한다.
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }//적이 죽을때마다 추가된 메소드가 작동될것이고 살아있는 적의 수가 0 이되면 다음 웨이브를 시작한다.

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        { 
        currentWave = waves[currentWaveNumber - 1];

        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
    }//웨이브가 남아있다면 조건에 맞춰 웨이브를 실행한다.

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
