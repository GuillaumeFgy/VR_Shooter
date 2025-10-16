using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;               // fixed positions in your scene

    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;            // your 5 prefabs
    public bool randomizePrefabPerPoint = true;  // off = use index mapping

    [Header("Timing")]
    public float initialDelay = 1.0f;
    public float spawnInterval = 3.5f;
    public int maxAlive = 8;

    [Header("Target")]
    public Transform player; // set to VR camera

    readonly List<GameObject> alive = new();

    void Start()
    {
        if (!player && Camera.main) player = Camera.main.transform;
        InvokeRepeating(nameof(TrySpawn), initialDelay, spawnInterval);
    }

    void TrySpawn()
    {
        alive.RemoveAll(go => go == null); // clean
        if (alive.Count >= maxAlive) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // pick a spawn point with no nearby enemy
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // Optional: ensure it’s clear
        // if (Physics.CheckSphere(sp.position, 0.5f)) return;

        // choose prefab
        GameObject prefab;
        if (randomizePrefabPerPoint)
        {
            prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }
        else
        {
            int idx = System.Array.IndexOf(spawnPoints, sp);
            if (idx < 0) idx = 0;
            idx = Mathf.Clamp(idx, 0, enemyPrefabs.Length - 1);
            prefab = enemyPrefabs[idx];
        }

        var go = Instantiate(prefab, sp.position, sp.rotation);
        alive.Add(go);

        // optional: wire the target at runtime
        var ctrl = go.GetComponent<EnemyController>();
        if (ctrl && player) ctrl.target = player;
    }
}
