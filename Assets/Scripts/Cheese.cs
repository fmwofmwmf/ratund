using UnityEngine;

public class Cheese : MonoBehaviour
{
    public float value;
    public void Eat()
    {
        Player.player.heft += value;
        Destroy(gameObject);
    }
}
