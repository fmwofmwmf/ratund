using UnityEngine;
using System.Collections.Generic;

public class Roulette : MonoBehaviour
{
    private Dictionary<int, RouletteSlot> slots = new Dictionary<int, RouletteSlot>();
    public float payoutCap;
    [Header("Collapsed reward prefab")]
    public GameObject collapsedPrefab;

    void Awake()
    {
        RouletteSlot[] foundSlots = GetComponentsInChildren<RouletteSlot>();
        foreach (var slot in foundSlots)
        {
            if (!slots.ContainsKey(slot.number))
                slots.Add(slot.number, slot);
        }
    }

    public void Payout(int number)
    {
        if (slots.TryGetValue(number, out RouletteSlot slot))
        {
            List<Chip> chips = slot.GetChips();
            float multiplier = slot.multiplier;
            float pay = 0;
            foreach (Chip chip in chips)
            {
                if (multiplier >= 1f)
                {
                    int extra = Mathf.FloorToInt(multiplier) - 1;
                    for (int i = 0; i < extra; i++)
                    {
                        if (pay >= payoutCap) break;
                        Vector3 pos = chip.transform.position + Vector3.up * (0.02f * (i + 1));
                        Instantiate(chip, pos, Quaternion.identity);
                        pay += chip.value;
                    }
                }

                float fractional = multiplier - Mathf.Floor(multiplier);
                if (fractional > 0f && Random.value < fractional)
                {
                    if (pay >= payoutCap) break;
                    Vector3 pos = chip.transform.position + Vector3.up * 0.05f;
                    Instantiate(chip, pos, Quaternion.identity);
                    pay += chip.value;
                }
            }

            Debug.Log($"Payout on slot {number} with multiplier {multiplier}. Chips: {chips.Count}");
        }
        else
        {
            Debug.LogWarning($"No slot for number {number}");
        }
    }

    /// <summary>
    /// Deletes all chips in the given slot and replaces them with one prefab.
    /// </summary>
    public void CollapseSlot(int number)
    {
        if (slots.TryGetValue(number, out RouletteSlot slot))
        {
            List<Chip> chips = slot.GetChips();

            if (chips.Count > 0)
            {
                // Find average position of chips to place the replacement prefab
                Vector3 center = Vector3.zero;
                foreach (Chip chip in chips)
                {
                    center += chip.transform.position;
                    Destroy(chip.gameObject);
                }
                center /= chips.Count;

                // Spawn collapsed prefab
                if (collapsedPrefab != null)
                {
                    Instantiate(collapsedPrefab, center, Quaternion.Euler(-90, 0, 0));
                }

                Debug.Log($"Collapsed {chips.Count} chips in slot {number} into 1 prefab.");
            }
        }
        else
        {
            Debug.LogWarning($"No slot for number {number}");
        }
    }
}
