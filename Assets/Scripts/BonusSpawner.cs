using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;       // Place your empty GameObjects here

    [Header("Bonus Prefabs")]
    public GameObject[] bonusPrefabs;     // 0..4 : Heart, Star, Arrows, Bullets, QuestionMark

    [Header("Timing")]
    public float minDelay = 5f;
    public float maxDelay = 10f;

    private GameObject currentBonus;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void ScheduleNextSpawn()
    {
        if (currentBonus == null)
        {
            float delay = Random.Range(minDelay, maxDelay);
            Invoke(nameof(SpawnBonus), delay);
        }
    }

    void SpawnBonus()
    {
        if (currentBonus != null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (bonusPrefabs == null || bonusPrefabs.Length == 0) return;

        Transform spot = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];

        currentBonus = Instantiate(prefab, spot.position, spot.rotation);

        var pickup = currentBonus.GetComponentInChildren<BonusPickup>();
        if (pickup != null)
        {
            pickup.ownerSpawner = this;
        }
    }

    public void OnBonusCollected(BonusPickup pickup)
    {
        if (pickup != null)
        {
            GameObject root = pickup.transform.root.gameObject;
            if (root == currentBonus)
            {
                currentBonus = null;
            }
        }
        ScheduleNextSpawn();
    }

}
