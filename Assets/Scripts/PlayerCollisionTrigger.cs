using UnityEngine;
using UnityEngine.Events;

public class PlayerCollisionTrigger : MonoBehaviour
{
    public UnityEvent onEnter;

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            onEnter.Invoke();
        }
    }
}
