using System;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody && other.rigidbody.gameObject.TryGetComponent(out Player p) && p.heft >= 1000)
        {
            PlinkoMachine.plinkoMachine.playKnobBreakSound();
            Destroy(gameObject);
        }
    }
}
