using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ChipSpawner : MonoBehaviour
{
    [Header("Chip Settings")]
    public List<Chip> chipTypes = new List<Chip>();  // Define your chip denominations
    public Transform spawnParent;                    // Where to spawn chips
    public float spawnDelay = 0.2f;                  // Time between each chip spawn

    /// <summary>
    /// Break a value into chips and spawn them with a delay.
    /// </summary>
    public void SpawnChips(int amount)
    {
        if (chipTypes.Count == 0) return;

        // Sort chipTypes from largest to smallest value
        chipTypes.Sort((a, b) => b.value.CompareTo(a.value));

        StartCoroutine(SpawnRoutine(amount));
    }

    private IEnumerator SpawnRoutine(int amount)
    {
        Vector3 spawnPos = spawnParent ? spawnParent.position : transform.position;

        foreach (Chip chip in chipTypes)
        {
            while (amount >= chip.value)
            {
                amount -= (int)chip.value;
                
                Instantiate(chip, spawnPos, Quaternion.identity);
                
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}