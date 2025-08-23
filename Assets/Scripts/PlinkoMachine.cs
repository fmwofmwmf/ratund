using UnityEngine;

public class PlinkoMachine : MonoBehaviour
{
    static public PlinkoMachine plinkoMachine;
    public Transform plinkoBallSpawnPoint;
    public Transform chipSpawnPoint;
    public PlinkoBall plinkoBallPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (plinkoMachine == null)
        {
            plinkoMachine = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void spawnPlinkoBall()
    {
        Instantiate(plinkoBallPrefab, plinkoBallSpawnPoint.position, plinkoBallSpawnPoint.rotation);
    }

    public void spawnChips(int amount = 0)
    { 
        for (int i = 0; i < amount; i++)
        {
            Instantiate(Player.player.chipPrefab, chipSpawnPoint.position, chipSpawnPoint.rotation);
        }
    }
}
