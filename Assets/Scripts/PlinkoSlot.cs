using UnityEngine;

public class PlinkoSlot : MonoBehaviour
{
    public float multiplier = 1.0f;

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            float spawnValue = player.GetPlinkoValue() * multiplier;
            PlinkoMachine.plinkoMachine.spawnChips((int)spawnValue);
            PlinkoMachine.plinkoMachine.ejectPlayerFromMachine();
        }
    }
}
