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
        transform.position += Player.player.transform.forward * 0.3f + Vector3.up * 0.1f;
    }

    public void WearHat()
    {
        _rb.isKinematic = true;
        colliders.SetActive(false);
        Player.player.WearHat(this);
    }
}
