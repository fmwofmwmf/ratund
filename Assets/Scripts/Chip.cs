using UnityEngine;

public class Chip : MonoBehaviour
{
    public float value;
    public void PickUpChip()
    {
        Player.player.PickUpChip();
        Destroy(gameObject);
    }
}
