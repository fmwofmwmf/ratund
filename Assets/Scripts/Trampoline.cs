
using System;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float baseForce;
    public float maxForce;
    public AnimationCurve velocityScaling;
    public float sizeScaling;
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody && other.rigidbody.TryGetComponent(out Player p))
        {
            if (Vector3.Dot(other.GetContact(0).normal.normalized, Vector3.up) > 0) return;
            Debug.Log(other.relativeVelocity.magnitude);
            float m = baseForce + p.EffectiveHeft * sizeScaling + velocityScaling.Evaluate(other.relativeVelocity.magnitude);
            m = Mathf.Min(m, maxForce);
            other.rigidbody.AddForce(m/10f * Vector3.up, ForceMode.Impulse);
        }
    }
}
