using UnityEngine;

public class Cheese : MonoBehaviour
{
    public float value;
    public void Eat()
    {
        Player.player.modifyHeft(value);
        Destroy(gameObject);
    }
}
