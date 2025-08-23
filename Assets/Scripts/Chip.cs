using UnityEngine;

public class Chip : MonoBehaviour
{
    public void PickUpChip()
    {
        Player.player.PickUpChip();
        Destroy(gameObject);
    }
}
