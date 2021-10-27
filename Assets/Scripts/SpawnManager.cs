using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    private readonly float spawnRangeX = 6.0f; //Радиус появления

    private readonly int enemyCount = 5;  //Количество врагов

    public static int waveNumber { get; private set; } = 0; //Номер волны

    void Start()
    {
        waveNumber = 0;
    }
    void Update()
    {
        SpawnEnemyWave(enemyCount);
    }
    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        float spawnPosZ = Random.Range(2f, 16f);
        Vector3 randomPos = new Vector3(spawnPosX, 0.5f, spawnPosZ);
        return randomPos;
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        if(!PlayerController.isDeath)
        {
            int activeEnemy = FindObjectsOfType<Goblin>().Length;
            if (activeEnemy == 0)
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
                }
                waveNumber++;
            }
        } 
    }
}
