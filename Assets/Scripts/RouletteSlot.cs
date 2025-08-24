using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class RouletteSlot : MonoBehaviour
{
    public int number;
    public float multiplier = 2f; // Example: 2 = double, 0.5 = 50% chance

    private List<Chip> chipsInSlot = new List<Chip>();

    void OnTriggerEnter(Collider other)
    {
        Chip chip = other.GetComponent<Chip>();
        if (chip != null && !chipsInSlot.Contains(chip))
        {
            chipsInSlot.Add(chip);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Chip chip = other.GetComponent<Chip>();
        if (chip != null)
        {
            chipsInSlot.Remove(chip);
        }
    }

    public List<Chip> GetChips()
    {
        return new List<Chip>(chipsInSlot); // return copy
    }
}