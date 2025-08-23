using UnityEngine;

public class PlinkoSlot : MonoBehaviour
{
    public float multiplier = 1.0f;

    void OnTriggerEnter(Collider other)
    {
        PlinkoBall plinkoBall = other.GetComponent<PlinkoBall>();
        if (plinkoBall != null)
        {
            plinkoBall.baseChipValue *= multiplier;
            PlinkoMachine.plinkoMachine.spawnChips((int)plinkoBall.baseChipValue);
            Destroy(other.gameObject);
        }
    }
}
