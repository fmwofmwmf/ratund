using UnityEngine;

public class PlinkoMachine : MonoBehaviour
{
    static public PlinkoMachine plinkoMachine;
    public Transform plinkoBallSpawnPoint;
    public Transform chipSpawnPoint;
    public PlinkoBall plinkoBallPrefab;
    public Chip chip1Prefab;
    public Chip chip5Prefab;
    public Chip chip20Prefab; 

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

    public void spawnChips(int value = 0)
    {
        while (true)
        {
            if (value >= 20)
            {
                value -= 20;
                Instantiate(chip20Prefab, chipSpawnPoint.position, chipSpawnPoint.rotation);
            }
            else if (value >= 5)
            {
                value -= 5;
                Instantiate(chip5Prefab, chipSpawnPoint.position, chipSpawnPoint.rotation);
            }
            else if (value >= 1)
            {
                value -= 1;
                Instantiate(chip1Prefab, chipSpawnPoint.position, chipSpawnPoint.rotation);
            }
            else
            {
                break;
            }
        }
    }
}
