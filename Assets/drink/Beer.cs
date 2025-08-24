using UnityEngine;

public class Beer : MonoBehaviour
{
    public float beerAmount;

    public void Drink()
    {
        Player.player.drunkness += beerAmount;
        Destroy(gameObject);
    }
}
