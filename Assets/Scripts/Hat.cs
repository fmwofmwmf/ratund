using UnityEngine;

public class Hat : MonoBehaviour
{
    public Rigidbody _rb;
    public GameObject colliders;
    
    public void DropHat()
    {
        _rb.isKinematic = false;
        colliders.SetActive(true);
        transform.parent = null;
    }

    public void WearHat()
    {
        _rb.isKinematic = true;
        colliders.SetActive(false);
        Player.player.WearHat(this);
    }
}
