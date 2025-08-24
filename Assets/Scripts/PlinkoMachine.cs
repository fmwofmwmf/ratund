using UnityEngine;

public class PlinkoMachine : MonoBehaviour
{
    public static PlinkoMachine plinkoMachine;

    public Camera plinkoCamera;
    private Camera previousCamera;
    public Transform plinkoBallSpawnPoint;
    public Transform ratEjectPoint;
    public Transform chipSpawnPoint;
    public Chip chip1Prefab;
    public Chip chip5Prefab;
    public Chip chip20Prefab;

    void Start()
    {
        if (plinkoCamera != null)
        {
            plinkoCamera.enabled = false;
        }

        if (plinkoMachine == null)
        {
            plinkoMachine = this;

        }
        else
        {
            Destroy(gameObject);
        }
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
    
    public void startPlinko()
    {
        // store old camera
        previousCamera = Camera.main;

        if (previousCamera != null)
            previousCamera.enabled = false;

        if (plinkoCamera != null)
            plinkoCamera.enabled = true;

        // move player to plinko start
        Player.player.transform.position = plinkoBallSpawnPoint.position;
        Player.player.transform.rotation = plinkoBallSpawnPoint.rotation;

        Rigidbody rb = Player.player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void ejectPlayerFromMachine()
    {
        // move player to eject point
        Player.player.transform.position = ratEjectPoint.position;
        Player.player.transform.rotation = ratEjectPoint.rotation;

        // switch camera back
        if (plinkoCamera != null)
            plinkoCamera.enabled = false;

        if (previousCamera != null)
            previousCamera.enabled = true;
    }
}