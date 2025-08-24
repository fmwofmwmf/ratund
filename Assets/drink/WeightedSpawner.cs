using UnityEngine;

using UnityEngine;

[System.Serializable]
public class WeightedPrefab
{
    public GameObject prefab;
    public float weight = 1f; // higher = more likely
}

public class WeightedSpawner : MonoBehaviour
{
    [Header("Prefabs with Weights")]
    public WeightedPrefab[] prefabs;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    /// <summary>
    /// Spawns one random prefab based on weights.
    /// </summary>
    public void SpawnRandom()
    {
        if (prefabs == null || prefabs.Length == 0) return;

        float totalWeight = 0f;
        foreach (var wp in prefabs)
            totalWeight += wp.weight;

        float randomValue = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var wp in prefabs)
        {
            cumulative += wp.weight;
            if (randomValue <= cumulative)
            {
                Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
                Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
                Instantiate(wp.prefab, pos, rot);
                return;
            }
        }

        return; // fallback
    }
}

