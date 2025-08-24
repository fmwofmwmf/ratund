using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class PlayerCollisionVelocityTrigger : MonoBehaviour
    {
        public UnityEvent onEnter;
        public float minVel;

        private void OnCollisionEnter(Collision other)
        {
            if (other.relativeVelocity.magnitude > minVel || Player.player.HeftStage > 0)
            {
                Player player = other.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    onEnter.Invoke();
                }
            }
        }
        

    }
}